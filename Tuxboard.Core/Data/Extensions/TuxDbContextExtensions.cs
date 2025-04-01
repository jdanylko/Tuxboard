using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Extensions;

/// <summary>
/// Extension methods for the Tuxboard DbContext
/// </summary>
public static class TuxDbContextExtensions
{
    #region Synchronous

    /// <summary>
    /// Get a widget by ID synchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="widgetId">Widget ID</param>
    /// <returns><see cref="Widget"/></returns>
    public static Widget GetWidget(this ITuxDbContext context, Guid widgetId)
    {
        return context.Widgets
            .Include(w => w.WidgetDefaults)
            .FirstOrDefault(r => r.WidgetId == widgetId);
    }

    /// <summary>
    /// Get a Layout by tab ID synchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="tabId">Dashboard Tab ID</param>
    /// <returns><see cref="Layout"/></returns>
    public static Layout GetLayoutForTab(this ITuxDbContext context, Guid tabId)
    {
        return context.Layouts
            .Include(e => e.LayoutRows)
                .ThenInclude(e => e.LayoutType)
            .Include(e => e.LayoutRows)
                .ThenInclude(e => e.WidgetPlacements)
            .AsNoTracking()
            .FirstOrDefault(r => r.TabId == tabId);
    }

    /// <summary>
    /// Get Widget Placements by layout synchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="layoutId">Layout ID</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
    public static List<WidgetPlacement> GetPlacementsByLayout(this ITuxDbContext context, Guid layoutId)
    {
        return context.WidgetPlacements.Where(r => r.LayoutRowId == layoutId)
            .ToList();
    }

    /// <summary>
    /// Get a default dashboard by ID synchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="id">Dashboard Default ID</param>
    /// <returns><see cref="DashboardDefault"/></returns>
    public static DashboardDefault GetDashboardDefault(this ITuxDbContext context, Guid id)
    {
        return context.DashboardDefaults
            .Include(e => e.DashboardDefaultWidgets)
                .ThenInclude(e => e.Widget)
            .Include(e => e.Layout)
                .ThenInclude(f => f.LayoutRows)
            .FirstOrDefault(e => e.DefaultId == id);
    }

    /// <summary>
    /// Get a default dashboard by plan ID synchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="planId">Plan ID</param>
    /// <returns><see cref="DashboardDefault"/></returns>
    public static DashboardDefault GetDashboardTemplateFor(this ITuxDbContext context, int planId = 0)
    {
        var layoutTypes = context.LayoutTypes.ToList();

        var query = context.DashboardDefaults
            .Include(dt => dt.DashboardDefaultWidgets)
                .ThenInclude(ddw => ddw.Widget)
            .Include(tab => tab.Layout)
                .ThenInclude(lo => lo.LayoutRows)
            .Where(y => !y.Layout.TabId.HasValue)
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

    /// <summary>
    /// Get the Widget Placements by a LayoutRow ID synchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="layoutRowId">Layout Row ID</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
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

    /// <summary>
    /// Return a Layout by ID synchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="layoutId">Layout ID</param>
    /// <returns><see cref="Layout"/></returns>
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

        foreach (var row in layout.LayoutRows)
        {
            row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
        }

        return layout;
    }

    /// <summary>
    /// Get all widget placements by a dashboard tab synchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="tab"><see cref="DashboardTab"/></param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
    public static List<WidgetPlacement> GetWidgetsForTab(this ITuxDbContext context, 
        DashboardTab tab) 
        => GetWidgetsForTab(context, tab.TabId);

    /// <summary>
    /// Get all widget placements by a dashboard tab ID synchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="tabId">Dashboard Tab ID</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
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

    /// <summary>
    /// Get a Widget Placement by ID synchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="widgetPlacementId">Widget Placement ID</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    public static WidgetPlacement GetWidgetPlacement(this ITuxDbContext context, Guid widgetPlacementId)
    {
        return context.WidgetPlacements
            .Include(e => e.WidgetSettings)
            .Include(e => e.Widget)
                .ThenInclude(w => w.WidgetDefaults)
            .FirstOrDefault(r => r.WidgetPlacementId == widgetPlacementId);
    }

    private static List<WidgetPlacement> UpdateMissingSettings(this ITuxDbContext context, List<WidgetPlacement> placements)
    {
        foreach (var placement in placements)
        {
            // Add the new settings if necessary.
            placement.UpdateWidgetSettings();

            // Save the missing settings to the table.
            var settings = placement.WidgetSettings.Where(e => e.WidgetSettingId == Guid.Empty);
            foreach (var setting in settings)
            {
                context.WidgetSettings.Add(setting);
                context.SaveChanges();
            }
        }

        return placements;
    }

    #endregion

    #region Asynchronous

    /// <summary>
    /// Get a widget by ID asynchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="widgetId">Widget ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Widget"/></returns>
    public static Task<Widget> GetWidgetAsync(this ITuxDbContext context, Guid widgetId, CancellationToken token = default) =>
        context.Widgets
            .Include(w => w.WidgetDefaults)
            .FirstOrDefaultAsync(r => r.WidgetId == widgetId, cancellationToken: token);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="layoutId"></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns></returns>
    public static Task<List<WidgetPlacement>> GetPlacementsByLayoutAsync(this ITuxDbContext context,
        Guid layoutId, CancellationToken token = default) =>
        context.WidgetPlacements.Where(r => r.LayoutRowId == layoutId)
            .ToListAsync(cancellationToken: token);

    /// <summary>
    /// Get a default dashboard by ID asynchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="id">Dashboard Default ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="DashboardDefault"/></returns>
    public static async Task<DashboardDefault> GetDashboardDefaultAsync(this ITuxDbContext context, Guid id,
        CancellationToken token = default)
    {
        return await context.DashboardDefaults
            .Include(e => e.DashboardDefaultWidgets)
                .ThenInclude(e => e.Widget)
            .Include(e => e.Layout)
                .ThenInclude(f => f.LayoutRows)
            .FirstOrDefaultAsync(e => e.DefaultId == id, cancellationToken: token);
    }

    /// <summary>
    /// Get a default dashboard by plan ID asynchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="planId">Plan ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="DashboardDefault"/></returns>
    public static async Task<DashboardDefault> GetDashboardTemplateForAsync(this ITuxDbContext context,
        int planId = 0, CancellationToken token = default)
    {
        var layoutTypes = await context.LayoutTypes.ToListAsync(cancellationToken: token);

        var query = context.DashboardDefaults
            .Include(dt => dt.DashboardDefaultWidgets)
                .ThenInclude(ddw => ddw.Widget)
            .Include(tab => tab.Layout)
                .ThenInclude(lo => lo.LayoutRows)
            .Where(y=> !y.Layout.TabId.HasValue)
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
        }

        return result;
    }

    /// <summary>
    /// Get a Layout by tab ID asynchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="tabId">Dashboard Tab ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Layout"/></returns>
    public static Task<Layout> GetLayoutForTabAsync(this ITuxDbContext context, Guid tabId, CancellationToken token = default) =>
        context.Layouts
            .Include(e => e.LayoutRows)
                .ThenInclude(e => e.LayoutType)
            .Include(e => e.LayoutRows)
                .ThenInclude(e => e.WidgetPlacements)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.TabId == tabId, cancellationToken: token);

    /// <summary>
    /// Get the Widget Placements by a LayoutRow ID asynchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="layoutRowId">Layout Row ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
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

    /// <summary>
    /// Return a Layout by ID asynchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="layoutId">Layout ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Layout"/></returns>
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

    /// <summary>
    /// Get all widget placements for a dashboard tab asynchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="tab"><see cref="DashboardTab"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
    public static Task<List<WidgetPlacement>> GetWidgetsForTabAsync(this ITuxDbContext context, DashboardTab tab, CancellationToken token = default) =>
        GetWidgetsForTabAsync(context, tab.TabId, token);

    /// <summary>
    /// Get all widget placements by a dashboard tab ID asynchronously
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="tabId">Dashboard Tab ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
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

    /// <summary>
    /// Get a Widget Placement by ID asynchronously.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="widgetPlacementId">Widget Placement ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    public static Task<WidgetPlacement> GetWidgetPlacementAsync(this ITuxDbContext context,
        Guid widgetPlacementId, CancellationToken token = default) =>

        context.WidgetPlacements
            .Include(e => e.WidgetSettings)
            .Include(e => e.Widget)
            .ThenInclude(w => w.WidgetDefaults)
            .FirstOrDefaultAsync(r => r.WidgetPlacementId == widgetPlacementId, cancellationToken: token);

    private static async Task<List<WidgetPlacement>> UpdateMissingSettingsAsync(this ITuxDbContext context,
        List<WidgetPlacement> placements, CancellationToken token = default)
    {
        foreach (var placement in placements)
        {
            // Add the new settings if necessary.
            placement.UpdateWidgetSettings();

            // Save the missing settings to the table.
            var settings = placement.WidgetSettings.Where(e => e.WidgetSettingId == Guid.Empty);
            foreach (var setting in settings)
            {
                setting.WidgetSettingId = new Guid();
                await context.WidgetSettings.AddAsync(setting, token);
                await context.SaveChangesAsync(new CancellationToken());
            }
        }

        return placements;
    }

    #endregion
}