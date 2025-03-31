using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Extensions;

/// <summary>
/// These extension methods are used when the user ID is an integer in the Dashboard table.
/// If the user ID is a GUID, use the TuxDbContextWithGuidExtensions.
/// </summary>
public static class TuxDbContextWithIntExtensions
{
    #region Synchronous

    /// <summary>
    /// Return the first Dashboard synchronously
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Dashboard<int> GetDashboard(this ITuxDbContext<int> context, ITuxboardConfig config)
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
            foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows).OrderBy(t => t.RowIndex))
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                row.WidgetPlacements = context.GetPlacementsByLayoutRow(row.LayoutRowId);
            }
        }

        dashboard.Settings = config;

        return dashboard;
    }

    /// <summary>
    /// Return a Dashboard for a user synchronously.
    /// If a User Dashboard doesn't exist, it'll create one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static Dashboard<int> GetDashboardFor(this ITuxDbContext<int> context, 
        ITuxboardConfig config, int userId)
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

    /// <summary>
    /// Returns whether a dashboard for a user exists. This call is synchronous.
    /// </summary>
    /// <param name="context"><see cref="ITuxDbContext"/></param>
    /// <param name="userId">User ID</param>
    /// <returns>true if the dashboard exists, false if a user doesn't have one yet.</returns>
    public static bool DashboardExistsFor(this ITuxDbContext<int> context, int userId) => 
        context.Dashboards.FirstOrDefault(e => e.UserId == userId) != null;

    /// <summary>
    /// Return whether a first Dashboard exists or not synchronously
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool DashboardExists(this ITuxDbContext<int> context)
        => context.Dashboards.FirstOrDefault() != null;

    #endregion

    #region Asynchronous

    /// <summary>
    /// Return whether a Dashboard exists or not for a user asynchronously
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<bool> DashboardExistsForAsync(this ITuxDbContext<int> context, 
        int userId, CancellationToken token = default)
    {
        var item = await context.Dashboards.FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken: token);
        return item != null;
    }

    /// <summary>
    /// Return whether a Dashboard exists or not asynchronously
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<bool> DashboardExistsAsync(this ITuxDbContext<int> context, 
        CancellationToken token = default)
    {
        var item = await context.Dashboards.FirstOrDefaultAsync(cancellationToken: token);
        return item != null;
    }

    /// <summary>
    /// Returns the first dashboard asynchronously 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<Dashboard<int>> GetDashboardAsync(
        this ITuxDbContext<int> context, ITuxboardConfig config, 
        CancellationToken token = default)
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
            foreach (var row in tab.GetLayouts().SelectMany(layout => layout.LayoutRows).OrderBy(t => t.RowIndex))
            {
                row.LayoutType = layoutTypes.FirstOrDefault(e => e.LayoutTypeId == row.LayoutTypeId);
                row.WidgetPlacements = await context.GetPlacementsByLayoutRowAsync(row.LayoutRowId, token: token);
            }
        }

        dashboard.Settings = config;

        return dashboard;
    }

    /// <summary>
    /// Returns a dashboard asynchronously based on a User ID
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<Dashboard<int>> GetDashboardForAsync(this ITuxDbContext<int> context,
        ITuxboardConfig config, int userId, CancellationToken token = default)
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

    #endregion
}