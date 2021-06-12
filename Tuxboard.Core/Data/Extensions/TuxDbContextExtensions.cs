using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Extensions
{
    public static class TuxDbContextExtensions
    {
        #region Synchronous

        public static Widget GetWidget(this ITuxDbContext context, string widgetId)
        {
            return context.Widget
                .Include(w => w.WidgetDefaults)
                .FirstOrDefault(r => r.WidgetId == widgetId);
        }

        public static Layout GetLayoutForTab(this ITuxDbContext context, string tabId)
        {
            return context.Layout
                .Include(e => e.LayoutRows)
                    .ThenInclude(e => e.LayoutType)
                .AsNoTracking()
                .FirstOrDefault(r => r.TabId == tabId);
        }

        public static List<WidgetPlacement> GetPlacementsByLayout(this ITuxDbContext context, string layoutId)
        {
            return context.WidgetPlacement.Where(r => r.LayoutRowId == layoutId)
                .ToList();
        }

        public static bool DashboardExistsFor(this ITuxDbContext context, string userId)
        {
            return context.Dashboard.FirstOrDefault(e => e.UserId == userId) != null;
        }

        public static Dashboard GetDashboardFor(this ITuxDbContext context, ITuxboardConfig config, string userId)
        {
            var layoutTypes = context.LayoutType.ToList();

            var dashboard = context.Dashboard
                .Include(db => db.Tabs)
                    .ThenInclude(tab => tab.Layouts)
                    .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking()
                .FirstOrDefault(t => t.UserId == userId);

            if (dashboard == null)
                return dashboard;

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

        public static DashboardDefault GetDashboardTemplateFor(this ITuxDbContext context, int planId=0)
        {
            var layoutTypes = context.LayoutType.ToList();

            var query = context.DashboardDefault
                .Include(dt => dt.DashboardDefaultWidgets)
                    .ThenInclude(ddw => ddw.Widget)
                .Include(tab => tab.Layout)
                    .ThenInclude(layout => layout.LayoutRows)
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
            string layoutRowId)
        {
            var placements = context.WidgetPlacement
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
                var settings = placement.WidgetSettings.Where(e => string.IsNullOrEmpty(e.WidgetSettingId));
                foreach (var setting in settings)
                {
                    context.WidgetSetting.Add(setting);
                    context.SaveChanges();
                }
            }

            return placements;
        }

        public static Layout GetLayout(this ITuxDbContext context, string layoutId)
        {
            var layoutTypes = context.LayoutType.ToList();

            var layout = context.Layout
                .Include(layout => layout.LayoutRows)
                    .ThenInclude(row=> row.WidgetPlacements)
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
            return GetWidgetsForTab(context, (string) tab.TabId);
        }

        public static List<WidgetPlacement> GetWidgetsForTab(this ITuxDbContext context, string tabId)
        {
            var placements = context.WidgetPlacement
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking().Where(r => r.LayoutRow.Layout.TabId == tabId)
                .ToList();

            foreach (var placement in placements)
            {
                placement.UpdateWidgetSettings();
                var settings = placement.WidgetSettings.Where(e => string.IsNullOrEmpty(e.WidgetSettingId));
                foreach (var setting in settings)
                {
                    context.WidgetSetting.Add(setting);
                    context.SaveChanges();
                }
            }

            return placements;
        }

        public static WidgetPlacement GetWidgetPlacement(this ITuxDbContext context, string widgetPlacementId)
        {
            return context.WidgetPlacement
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .FirstOrDefault(r => r.WidgetPlacementId == widgetPlacementId);
        }

        #endregion

        #region Asynchronous

        public static Task<Widget> GetWidgetAsync(this ITuxDbContext context, string widgetId) =>
            context.Widget
                .Include(w => w.WidgetDefaults)
                .FirstOrDefaultAsync(r => r.WidgetId == widgetId);

        public static Task<List<WidgetPlacement>> GetPlacementsByLayoutAsync(this ITuxDbContext context, 
            string layoutId) =>
            context.WidgetPlacement.Where(r => r.LayoutRowId == layoutId)
                .ToListAsync();

        public static async Task<bool> DashboardExistsForAsync(this ITuxDbContext context, string userId)
        {
            var item = await context.Dashboard.FirstOrDefaultAsync(t => t.UserId == userId);
            return item != null;
        }

        public static async Task<Dashboard> GetDashboardForAsync(this ITuxDbContext context,
            ITuxboardConfig config, string userId)
        {
            var layoutTypes = await context.LayoutType.ToListAsync();

            var dashboard = await context.Dashboard
                .Include(db => db.Tabs)
                    .ThenInclude(tab => tab.Layouts)
                        .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (dashboard == null)
                return dashboard;

            foreach (var tab in dashboard.Tabs)
            {
                foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows))
                {
                    row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                    row.WidgetPlacements = await context.GetPlacementsByLayoutRowAsync(row.LayoutRowId);
                }
            }

            dashboard.Settings = config;

            return dashboard;
        }

        public static async Task<DashboardDefault> GetDashboardTemplateForAsync(this ITuxDbContext context, int planId=0)
        {
            var layoutTypes = await context.LayoutType.ToListAsync();

            var query = context.DashboardDefault
                .Include(dt => dt.DashboardDefaultWidgets)
                    .ThenInclude(ddw => ddw.Widget)
                .Include(tab => tab.Layout)
                    .ThenInclude(layout => layout.LayoutRows)
                .AsNoTracking();

            var result = planId > 0
                ? await query.FirstOrDefaultAsync(e => e.PlanId == planId)
                : await query.FirstOrDefaultAsync();

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

        public static Task<Layout> GetLayoutForTabAsync(this ITuxDbContext context, string tabId) =>
            context.Layout
                .Include(e => e.LayoutRows)
                .ThenInclude(e => e.LayoutType)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TabId == tabId);

        public static async Task<List<WidgetPlacement>> GetPlacementsByLayoutRowAsync(this ITuxDbContext context,
            string layoutRowId)
        {
            var placements = await context.WidgetPlacement
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking()
                .Where(r => r.LayoutRowId == layoutRowId)
                .ToListAsync();

            return await UpdateMissingSettingsAsync(context, placements);
        }

        public static async Task<Layout> GetLayoutAsync(this ITuxDbContext context, string layoutId)
        {
            var layoutTypes = await context.LayoutType.ToListAsync();

            var layout = await context.Layout
                    .Include(layout => layout.LayoutRows)
                        .ThenInclude(row=> row.WidgetPlacements)
                            .ThenInclude(wp => wp.Widget)
                                .ThenInclude(w => w.WidgetDefaults)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.LayoutId == layoutId);

            foreach (var row in layout.LayoutRows)
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
            }

            return layout;
        }

        public static Task<List<WidgetPlacement>> GetWidgetsForTabAsync(this ITuxDbContext context, DashboardTab tab) => 
            GetWidgetsForTabAsync(context, (string) tab.TabId);

        public static async Task<List<WidgetPlacement>> GetWidgetsForTabAsync(this ITuxDbContext context, string tabId)
        {
            var placements = await context.WidgetPlacement
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .AsNoTracking().Where(r => r.LayoutRow.Layout.TabId == tabId)
                .ToListAsync();

            return await context.UpdateMissingSettingsAsync(placements);
        }

        private static async Task<List<WidgetPlacement>> UpdateMissingSettingsAsync(this ITuxDbContext context, List<WidgetPlacement> placements)
        {
            foreach (var placement in placements)
            {
                // Add the new settings if necessary.
                placement.UpdateWidgetSettings();
                
                // Save the missing settings to the table.
                var settings = placement.WidgetSettings.Where(e => string.IsNullOrEmpty(e.WidgetSettingId));
                foreach (var setting in settings)
                {
                    await context.WidgetSetting.AddAsync(setting);
                    await context.SaveChangesAsync(new CancellationToken());
                }
            }

            return placements;
        }

        public static Task<WidgetPlacement> GetWidgetPlacementAsync(this ITuxDbContext context, string widgetPlacementId) =>
            context.WidgetPlacement
                .Include(e => e.WidgetSettings)
                .Include(e => e.Widget)
                    .ThenInclude(w => w.WidgetDefaults)
                .FirstOrDefaultAsync(r => r.WidgetPlacementId == widgetPlacementId);

        #endregion
    }
}
