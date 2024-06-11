using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Extensions;

public static class TuxDbContextExtensions
{
    #region Synchronous

    public static Widget GetWidget(this ITuxDbContext context, Guid widgetId)
    {
            return context.Widgets
                .Include(w => w.WidgetDefaults)
                .FirstOrDefault(r => r.WidgetId == widgetId);
        }

    public static Layout GetLayoutForTab(this ITuxDbContext context, Guid tabId)
    {
            return context.Layouts
                .Include(e => e.LayoutRows)
                    .ThenInclude(e => e.LayoutType)
                .AsNoTracking()
                .FirstOrDefault(r => r.TabId == tabId);
        }

    public static List<WidgetPlacement> GetPlacementsByLayout(this ITuxDbContext context, Guid layoutId)
    {
            return context.WidgetPlacements.Where(r => r.LayoutRowId == layoutId)
                .ToList();
        }

    public static bool DashboardExistsFor(this ITuxDbContext context, Guid userId)
    {
            return context.Dashboards.FirstOrDefault(e => e.UserId == userId) != null;
        }

    public static bool DashboardExists(this ITuxDbContext context)
    {
            return context.Dashboards.FirstOrDefault() != null;
        }

    public static Dashboard GetDashboard(this ITuxDbContext context, ITuxboardConfig config)
    {
            var layoutTypes = context.LayoutTypes.ToList();

            var dashboard = context.Dashboards
                .Include(db => db.Tabs)
                    .ThenInclude(tab => tab.Layouts)
                        .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking()
                .FirstOrDefault();

            if (dashboard == null)
                return null;

            // Assign the LayoutTypes to each row; get the settings for the WidgetPlacements.
            foreach (var tab in dashboard.Tabs)
            {
                foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows))
                {
                    row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                    row.WidgetPlacements = context.GetPlacementsByLayoutRow(row.LayoutRowId);
                }
            }

            dashboard.Settings = config;

            return dashboard;
        }

    public static Dashboard GetDashboardFor(this ITuxDbContext context, ITuxboardConfig config, Guid userId)
    {
            var layoutTypes = context.LayoutTypes.ToList();

            var dashboard = context.Dashboards
                .Include(db => db.Tabs)
                    .ThenInclude(tab => tab.Layouts)
                        .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking()
                .FirstOrDefault(t => t.UserId == userId);

            if (dashboard == null)
                return null;

            // Assign the LayoutTypes to each row; get the settings for the WidgetPlacements.
            foreach (var tab in dashboard.Tabs)
            {
                foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows))
                {
                    row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                    row.WidgetPlacements = context.GetPlacementsByLayoutRow(row.LayoutRowId);
                }
            }

            dashboard.Settings = config;

            return dashboard;
        }

    public static DashboardDefault GetDashboardTemplateFor(this ITuxDbContext context, int planId = 0)
    {
            var layoutTypes = context.LayoutTypes.ToList();

            var query = context.DashboardDefaults
                .Include(dt => dt.DashboardDefaultWidgets)
                    .ThenInclude(ddw => ddw.Widget)
                .Include(tab => tab.Layout)
                    .ThenInclude(lo => lo.LayoutRows)
                .AsNoTracking();

            var result = planId > 0
                ? query.FirstOrDefault(e => e.PlanId == planId)
                : query.FirstOrDefault();

            if (result == null)
                return null;

            var layout = result.Layout;
            foreach (var row in layout.LayoutRows)
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                //row.WidgetPlacements = context.GetPlacementsByLayoutRow(row.LayoutRowId);
            }

            return result;
        }

    public static List<WidgetPlacement> GetPlacementsByLayoutRow(this ITuxDbContext context,
        Guid layoutRowId)
    {
            var placements = context.WidgetPlacements
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking()
                .Where(r => r.LayoutRowId == layoutRowId)
                .ToList();

            return context.UpdateMissingSettings(placements);
        }


    private static List<WidgetPlacement> UpdateMissingSettings(this ITuxDbContext context, List<WidgetPlacement> placements)
    {
            foreach (var placement in placements)
            {
                // Add the new settings if necessary.
                placement.UpdateWidgetSettings();

                // Save the missing settings to the table.
                var settings = placement.WidgetSettings.Where(e => e.WidgetSettingId==Guid.Empty);
                foreach (var setting in settings)
                {
                    context.WidgetSettings.Add(setting);
                    context.SaveChanges();
                }
            }

            return placements;
        }

    public static Layout GetLayout(this ITuxDbContext context, Guid layoutId)
    {
            var layoutTypes = context.LayoutTypes.ToList();

            var layout = context.Layouts
                .Include(lo => lo.LayoutRows)
                    .ThenInclude(row => row.WidgetPlacements)
                        .ThenInclude(wp => wp.Widget)
                            .ThenInclude(w => w.WidgetDefaults)

                .AsNoTracking()
                .FirstOrDefault(e => e.LayoutId == layoutId);

            foreach (LayoutRow row in layout.LayoutRows)
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
            }

            return layout;
        }

    public static List<WidgetPlacement> GetWidgetsForTab(this ITuxDbContext context, DashboardTab tab)
    {
            return GetWidgetsForTab(context, tab.TabId);
        }

    public static List<WidgetPlacement> GetWidgetsForTab(this ITuxDbContext context, Guid tabId)
    {
            var placements = context.WidgetPlacements
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking().Where(r => r.LayoutRow.Layout.TabId == tabId)
                .ToList();

            foreach (var placement in placements)
            {
                placement.UpdateWidgetSettings();
                var settings = placement.WidgetSettings.Where(e => e.WidgetSettingId == Guid.Empty);
                foreach (var setting in settings)
                {
                    setting.WidgetSettingId = new Guid();
                    context.WidgetSettings.Add(setting);
                    context.SaveChanges();
                }
            }

            return placements;
        }

    public static WidgetPlacement GetWidgetPlacement(this ITuxDbContext context, Guid widgetPlacementId)
    {
            return context.WidgetPlacements
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .FirstOrDefault(r => r.WidgetPlacementId == widgetPlacementId);
        }

    #endregion

    #region Asynchronous

    public static Task<Widget> GetWidgetAsync(this ITuxDbContext context, Guid widgetId, CancellationToken token = default) =>
        context.Widgets
            .Include(w => w.WidgetDefaults)
            .FirstOrDefaultAsync(r => r.WidgetId == widgetId, cancellationToken: token);

    public static Task<List<WidgetPlacement>> GetPlacementsByLayoutAsync(this ITuxDbContext context,
        Guid layoutId, CancellationToken token = default) =>
        context.WidgetPlacements.Where(r => r.LayoutRowId == layoutId)
            .ToListAsync(cancellationToken: token);

    public static async Task<bool> DashboardExistsForAsync(this ITuxDbContext context, Guid userId, CancellationToken token = default)
    {
            var item = await context.Dashboards.FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken: token);
            return item != null;
        }

    public static async Task<bool> DashboardExistsAsync(this ITuxDbContext context, CancellationToken token = default)
    {
            var item = await context.Dashboards.FirstOrDefaultAsync(cancellationToken: token);
            return item != null;
        }

    public static async Task<Dashboard> GetDashboardAsync(this ITuxDbContext context, ITuxboardConfig config, CancellationToken token = default)
    {
            var layoutTypes = await context.LayoutTypes.ToListAsync(cancellationToken: token);

            var dashboard = await context.Dashboards
                .Include(db => db.Tabs)
                    .ThenInclude(tab => tab.Layouts)
                        .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: token);

            if (dashboard == null)
                return null;

            // Assign the LayoutTypes to each row; get the settings for the WidgetPlacements.
            foreach (var tab in dashboard.Tabs)
            {
                foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows))
                {
                    row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                    row.WidgetPlacements = await context.GetPlacementsByLayoutRowAsync(row.LayoutRowId, token: token);
                }
            }

            dashboard.Settings = config;

            return dashboard;
        }

    public static async Task<Dashboard> GetDashboardForAsync(this ITuxDbContext context,
        ITuxboardConfig config, Guid userId, CancellationToken token = default)
    {
            var layoutTypes = await context.LayoutTypes.ToListAsync(cancellationToken: token);

            var dashboard = await context.Dashboards
                .Include(db => db.Tabs)
                    .ThenInclude(tab => tab.Layouts)
                        .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken: token);

            if (dashboard == null)
                return null;

            foreach (var tab in dashboard.Tabs)
            {
                foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows))
                {
                    row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                    row.WidgetPlacements = await context.GetPlacementsByLayoutRowAsync(row.LayoutRowId, token: token);
                }
            }

            dashboard.Settings = config;

            return dashboard;
        }

    public static async Task<DashboardDefault> GetDashboardTemplateForAsync(this ITuxDbContext context, 
        int planId = 0, CancellationToken token = default)
    {
            var layoutTypes = await context.LayoutTypes.ToListAsync(cancellationToken: token);

            var query = context.DashboardDefaults
                .Include(dt => dt.DashboardDefaultWidgets)
                    .ThenInclude(ddw => ddw.Widget)
                .Include(tab => tab.Layout)
                    .ThenInclude(lo => lo.LayoutRows)
                .AsNoTracking();

            var result = planId > 0
                ? await query.FirstOrDefaultAsync(e => e.PlanId == planId, cancellationToken: token)
                : await query.FirstOrDefaultAsync(cancellationToken: token);

            if (result == null)
                return null;

            var layout = result.Layout;
            foreach (var row in layout.LayoutRows)
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                // row.WidgetPlacements = await context.GetPlacementsByLayoutRowAsync(row.LayoutRowId);
            }

            return result;
        }

    public static Task<Layout> GetLayoutForTabAsync(this ITuxDbContext context, Guid tabId, CancellationToken token = default) =>
        context.Layouts
            .Include(e => e.LayoutRows)
            .ThenInclude(e => e.LayoutType)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.TabId == tabId, cancellationToken: token);

    public static async Task<List<WidgetPlacement>> GetPlacementsByLayoutRowAsync(this ITuxDbContext context,
        Guid layoutRowId, CancellationToken token = default)
    {
            var placements = await context.WidgetPlacements
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking()
                .Where(r => r.LayoutRowId == layoutRowId)
                .ToListAsync(cancellationToken: token);

            return await UpdateMissingSettingsAsync(context, placements, token);
        }

    public static async Task<Layout> GetLayoutAsync(this ITuxDbContext context, Guid layoutId, CancellationToken token = default)
    {
            var layoutTypes = await context.LayoutTypes.ToListAsync(cancellationToken: token);

            var layout = await context.Layouts
                    .Include(lo => lo.LayoutRows)
                        .ThenInclude(row => row.WidgetPlacements)
                            .ThenInclude(wp => wp.Widget)
                                .ThenInclude(w => w.WidgetDefaults)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.LayoutId == layoutId, cancellationToken: token);

            foreach (var row in layout.LayoutRows)
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
            }

            return layout;
        }

    public static Task<List<WidgetPlacement>> GetWidgetsForTabAsync(this ITuxDbContext context, DashboardTab tab, CancellationToken token = default) =>
        GetWidgetsForTabAsync(context, tab.TabId, token);

    public static async Task<List<WidgetPlacement>> GetWidgetsForTabAsync(this ITuxDbContext context, Guid tabId, CancellationToken token = default)
    {
            var placements = await context.WidgetPlacements
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking().Where(r => r.LayoutRow.Layout.TabId == tabId)
                .ToListAsync(cancellationToken: token);

            return await context.UpdateMissingSettingsAsync(placements, token: token);
        }

    private static async Task<List<WidgetPlacement>> UpdateMissingSettingsAsync(this ITuxDbContext context, 
        List<WidgetPlacement> placements, CancellationToken token = default)
    {
            foreach (var placement in placements)
            {
                // Add the new settings if necessary.
                placement.UpdateWidgetSettings();

                // Save the missing settings to the table.
                var settings = placement.WidgetSettings.Where(e => e.WidgetSettingId==Guid.Empty);
                foreach (var setting in settings)
                {
                    setting.WidgetSettingId = new Guid();
                    await context.WidgetSettings.AddAsync(setting, token);
                    await context.SaveChangesAsync(new CancellationToken());
                }
            }

            return placements;
        }

    public static Task<WidgetPlacement> GetWidgetPlacementAsync(this ITuxDbContext context, 
        Guid widgetPlacementId, CancellationToken token = default) =>
            
        context.WidgetPlacements
            .Include(e => e.WidgetSettings)
            .Include(e => e.Widget)
            .ThenInclude(w => w.WidgetDefaults)
            .FirstOrDefaultAsync(r => r.WidgetPlacementId == widgetPlacementId, cancellationToken: token);

    #endregion
}