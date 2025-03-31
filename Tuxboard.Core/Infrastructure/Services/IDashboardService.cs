using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.Services;

/// <summary>
/// Primary service for retrieving and managing Tuxboard dashboards.
/// </summary>
public interface IDashboardService<TUserId> where TUserId: struct
{
    /// <summary>
    /// Create a dashboard from a default dashboard synchronously.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    Dashboard<TUserId> CreateDashboardFrom(DashboardDefault template);
    /// <summary>
    /// Create a dashboard from a default dashboard asynchronously.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Dashboard<TUserId>> CreateDashboardFromAsync(DashboardDefault template, 
        CancellationToken token = default);

    /// <summary>
    /// Create a dashboard from a default dashboard and, optionally,
    /// assign a user id to the dashboard synchronously.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Dashboard<TUserId> CreateFromTemplate(DashboardDefault template, TUserId? userId = null);
    /// <summary>
    /// Create a dashboard from a default dashboard and, optionally,
    /// assign a user id to the dashboard asynchronously.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Dashboard<TUserId>> CreateFromTemplateAsync(DashboardDefault template, TUserId? userId = null,
        CancellationToken token = default);

    /// <summary>
    /// Retrieve a <see cref="Dashboard"/> for a user synchronously.
    /// If a dashboard doesn't exist for the user, it'll create one based on a default dashboard.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Dashboard<TUserId> GetDashboardFor(ITuxboardConfig config, TUserId userId);
    /// <summary>
    /// Retrieve a <see cref="Dashboard"/> for a user asynchronously.
    /// If a dashboard doesn't exist for the user, it'll create one based on a default dashboard.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Dashboard<TUserId>> GetDashboardForAsync(ITuxboardConfig config, TUserId userId, 
        CancellationToken token = default);
    
    /// <summary>
    /// Retrieve a static <see cref="Dashboard"/> synchronously
    /// If a dashboard doesn't exist, it'll create one based on an existing default dashboard.
    /// </summary>
    /// <param name="config"><see cref="ITuxboardConfig"/></param>
    /// <returns><see cref="Dashboard"/></returns>
    Dashboard<TUserId> GetDashboard(ITuxboardConfig config);
    /// <summary>
    /// Retrieve a static <see cref="Dashboard"/> asynchronously
    /// If a dashboard doesn't exist, it'll create one based on an existing default dashboard.
    /// </summary>
    /// <param name="config"><see cref="ITuxboardConfig"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Dashboard"/></returns>
    Task<Dashboard<TUserId>> GetDashboardAsync(ITuxboardConfig config, CancellationToken token = default);

    /// <summary>
    /// Retrieve a <see cref="Layout"/> from a <see cref="DashboardTab"/> Id synchronously.
    /// If a dashboard doesn't exist, it'll create one based on an existing default dashboard.
    /// </summary>
    /// <param name="tabId">Tab Id</param>
    /// <returns><see cref="Layout"/></returns>
    Layout GetLayoutFromTab(Guid tabId);
    /// <summary>
    /// Retrieve a <see cref="Layout"/> from a <see cref="DashboardTab"/> Id asynchronously.
    /// If a dashboard doesn't exist, it'll create one based on an existing default dashboard.
    /// </summary>
    /// <param name="tabId">Tab Id</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Layout"/></returns>
    Task<Layout> GetLayoutFromTabAsync(Guid tabId, CancellationToken token = default);

    /// <summary>
    /// Retrieve a <see cref="List{WidgetPlacement}"/> from a <see cref="DashboardTab"/> synchronously.
    /// </summary>
    /// <param name="tab"><see cref="DashboardTab"/></param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
    List<WidgetPlacement> GetWidgetsForTab(DashboardTab tab);
    /// <summary>
    /// Retrieve a <see cref="List{WidgetPlacement}"/> from a <see cref="DashboardTab"/> asynchronously.
    /// </summary>
    /// <param name="tab"><see cref="DashboardTab"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
    Task<List<WidgetPlacement>> GetWidgetsForTabAsync(DashboardTab tab, CancellationToken token = default);

    /// <summary>
    /// Create a <see cref="Dashboard"/> from a <see cref="DashboardDefault"/>. There is an option to assign a user ID to the dashboard as well. This is a synchronous call.
    /// </summary>
    /// <param name="template"><see cref="DashboardDefault"/></param>
    /// <param name="userId">UserID - <see cref="Guid"/></param>
    /// <returns><see cref="Dashboard"/></returns>
    Dashboard<TUserId> CreateDashboardFrom(DashboardDefault template, TUserId? userId);
    /// <summary>
    /// Create a <see cref="Dashboard"/> from a <see cref="DashboardDefault"/>. There is an option to assign a user ID to the dashboard as well. This is an asynchronous call.
    /// </summary>
    /// <param name="template"><see cref="DashboardDefault"/></param>
    /// <param name="userId">UserID - <see cref="Guid"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Dashboard"/></returns>
    Task<Dashboard<TUserId>> CreateDashboardFromAsync(DashboardDefault template, TUserId? userId,
        CancellationToken token = default);

    /// <summary>
    /// Return a list of <see cref="LayoutType"/> synchronously.
    /// </summary>
    /// <returns><see cref="List{LayoutType}"/></returns>
    List<LayoutType> GetLayoutTypes();
    /// <summary>
    /// Return a list of <see cref="LayoutType"/> asynchronously.
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{LayoutType}"/></returns>
    Task<List<LayoutType>> GetLayoutTypesAsync(CancellationToken token = default);

    /// <summary>
    /// Return a list of <see cref="Widget"/>s available for a subscriber plan synchronously. If PlanId is zero, all widgets will load.
    /// </summary>
    /// <param name="planId">Subscriber Plan ID (optional)</param>
    /// <returns><see cref="List{Widget}"/></returns>
    List<Widget> GetWidgetsFor(int planId=0);
    /// <summary>
    /// Return a list of <see cref="Widget"/>s available for a subscriber plan asynchronously. If PlanId is zero, all widgets will load.
    /// </summary>
    /// <param name="planId">Subscriber Plan ID (optional)</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{Widget}"/></returns>
    Task<List<Widget>> GetWidgetsForAsync(int planId = 0, CancellationToken token = default);

    /// <summary>
    /// Return a <see cref="Widget"/> by ID synchronously.
    /// </summary>
    /// <param name="id">Widget ID</param>
    /// <returns><see cref="Widget"/></returns>
    Widget GetWidget(Guid id);
    /// <summary>
    /// Return a <see cref="Widget"/> by ID asynchronously.
    /// </summary>
    /// <param name="id">Widget ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="Widget"/></returns>
    Task<Widget> GetWidgetAsync(Guid id, CancellationToken token = default);

    /// <summary>
    /// Return a <see cref="List{Widget}"/> synchronously.
    /// </summary>
    /// <returns><see cref="List{Widget}"/></returns>
    List<Widget> GetWidgets();
    /// <summary>
    /// Return a <see cref="List{Widget}"/> asynchronously.
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{Widget}"/></returns>
    Task<List<Widget>> GetWidgetsAsync(CancellationToken token = default);

    /// <summary>
    /// Retrieve a <see cref="WidgetPlacement"/> by ID synchronously.
    /// </summary>
    /// <param name="id">Widget Placement ID</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    WidgetPlacement GetWidgetPlacement(Guid id);
    /// <summary>
    /// Retrieve a <see cref="WidgetPlacement"/> by ID asynchronously.
    /// </summary>
    /// <param name="id">Widget Placement ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    Task<WidgetPlacement> GetWidgetPlacementAsync(Guid id, CancellationToken token = default);

    /// <summary>
    /// Remove a <see cref="LayoutRow"/> by ID synchronously. If a <see cref="LayoutRow"/> contains widgets, the layout row will NOT be removed and fail every time.
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <returns>true if successfully removed, false if not.</returns>
    bool RemoveLayoutRow(LayoutRow row);
    /// <summary>
    /// Remove a <see cref="LayoutRow"/> by ID asynchronously. If a <see cref="LayoutRow"/> contains widgets, the layout row will NOT be removed and fail every time.
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>true if successfully removed, false if not.</returns>
    Task<bool> RemoveLayoutRowAsync(LayoutRow row, CancellationToken token = default);

    /// <summary>
    /// Save a Dashboard's Layout by comparing a user's requested layout with the existing layout.
    /// To update a dashboard's layout, this performs a simple synchronization process of adding, removing, and updating Layout Rows.
    /// This is a synchronous call.
    /// </summary>
    /// <param name="tabId">Dashboard Tab ID</param>
    /// <param name="layoutList"><see cref="List{LayoutOrder}"/></param>
    /// <returns>true if successfully updates, false if not.</returns>
    bool SaveLayout(Guid tabId, List<LayoutOrder> layoutList);
    /// <summary>
    /// Save a Dashboard's Layout by comparing a user's requested layout with the existing layout.
    /// To update a dashboard's layout, this performs a simple synchronization process of adding, removing, and updating Layout Rows.
    /// This is an asynchronous call.
    /// </summary>
    /// <param name="tabId">Dashboard Tab ID</param>
    /// <param name="layoutList"><see cref="List{LayoutOrder}"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>true if successfully updates, false if not.</returns>
    Task<bool> SaveLayoutAsync(Guid tabId, List<LayoutOrder> layoutList, CancellationToken token = default);

    /// <summary>
    /// Adds a <see cref="LayoutRow"/> with a <see cref="LayoutType"/> ID to an existing <see cref="Layout"/> synchronously.
    /// </summary>
    /// <param name="layout"><see cref="Layout"/></param>
    /// <param name="layoutTypeId">Layout Type ID</param>
    /// <returns>true if successfully added, false if not.</returns>
    bool AddLayoutRow(Layout layout, int layoutTypeId);
    /// <summary>
    /// Adds a <see cref="LayoutRow"/> with a <see cref="LayoutType"/> ID to an existing <see cref="Layout"/> asynchronously.
    /// </summary>
    /// <param name="layout"><see cref="Layout"/></param>
    /// <param name="layoutTypeId">Layout Type ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>true if successfully added, false if not.</returns>
    Task<bool> AddLayoutRowAsync(Layout layout, int layoutTypeId, CancellationToken token = default);

    /// <summary>
    /// Adds a <see cref="Widget"/> to a tab synchronously using the <see cref="DashboardTab"/> ID.
    /// By default, the widget is added to the first <see cref="LayoutRow"/> at the bottom of the first column.
    /// </summary>
    /// <param name="tabId"><see cref="DashboardTab"/> ID</param>
    /// <param name="widgetId"><see cref="Widget"/> ID</param>
    /// <returns><see cref="AddWidgetResponse"/></returns>
    AddWidgetResponse AddWidgetToTab(Guid tabId, Guid widgetId);
    /// <summary>
    /// Adds a <see cref="Widget"/> to a tab asynchronously using the <see cref="DashboardTab"/> ID.
    /// By default, the widget is added to the first <see cref="LayoutRow"/> at the bottom of the first column.
    /// </summary>
    /// <param name="tabId"><see cref="DashboardTab"/> ID</param>
    /// <param name="widgetId"><see cref="Widget"/> ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="AddWidgetResponse"/></returns>
    Task<AddWidgetResponse> AddWidgetToTabAsync(Guid tabId, Guid widgetId, CancellationToken token = default);

    /// <summary>
    /// Remove a <see cref="WidgetPlacement"/> from a dashboard synchronously.
    /// </summary>
    /// <param name="placementId"><see cref="WidgetPlacement"/> ID</param>
    /// <returns>true if successfully removed, false if not.</returns>
    bool RemoveWidget(Guid placementId);
    /// <summary>
    /// Remove a <see cref="WidgetPlacement"/> from a dashboard asynchronously.
    /// </summary>
    /// <param name="placementId"><see cref="WidgetPlacement"/> ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>true if successfully removed, false if not.</returns>
    Task<bool> RemoveWidgetAsync(Guid placementId, CancellationToken token = default);

    /// <summary>
    /// When a <see cref="WidgetPlacement"/> is moved on a dashboard from one <see cref="LayoutRow"/> to another,
    /// location data is required to persist the data.
    /// The <see cref="PlacementParameter"/> contains location data of a WidgetPlacement including the previous
    /// and current layout row id and column.
    /// This is a synchronous call.
    /// </summary>
    /// <param name="param"><see cref="PlacementParameter"/></param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    WidgetPlacement SaveWidgetPlacement(PlacementParameter param);
    /// <summary>
    /// When a <see cref="WidgetPlacement"/> is moved on a dashboard from one <see cref="LayoutRow"/> to another,
    /// location data is required to persist the data.
    /// The <see cref="PlacementParameter"/> contains location data of a WidgetPlacement including the previous
    /// and current layout row id and column.
    /// This is an asynchronous call.
    /// </summary>
    /// <param name="param"><see cref="PlacementParameter"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    Task<WidgetPlacement> SaveWidgetPlacementAsync(PlacementParameter param, CancellationToken token = default);

    /// <summary>
    /// Updates the <see cref="WidgetPlacement.Collapsed"/> synchronously.
    /// </summary>
    /// <param name="id"><see cref="WidgetPlacement"/> ID</param>
    /// <param name="collapsed">Get or set to collapse the body. True to collapse the body and only display the header, false to expand and display the body of the widget.</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    WidgetPlacement UpdateCollapsed(Guid id, bool collapsed);
    /// <summary>
    /// Updates the <see cref="WidgetPlacement.Collapsed"/> asynchronously.
    /// </summary>
    /// <param name="id"><see cref="WidgetPlacement"/> ID</param>
    /// <param name="collapsed">Get or set to collapse the body. True to collapse the body and only display the header, false to expand and display the body of the widget.</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    Task<WidgetPlacement> UpdateCollapsedAsync(Guid id, bool collapsed, CancellationToken token = default);

    /// <summary>
    /// Saves a widget's settings synchronously.
    /// </summary>
    /// <param name="settings">Updated widget settings for a <see cref="WidgetPlacement"/></param>
    /// <returns><see cref="List{WidgetSettingDto}"/></returns>
    List<WidgetSettingDto> SaveWidgetSettings(List<WidgetSetting> settings);
    /// <summary>
    /// Saves a widget's settings asynchronously.
    /// </summary>
    /// <param name="settings">Updated widget settings for a <see cref="WidgetPlacement"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns><see cref="List{WidgetSettingDto}"/></returns>
    Task<List<WidgetSettingDto>> SaveWidgetSettingsAsync(List<WidgetSetting> settings,
        CancellationToken token = default);

    /// <summary>
    /// Change a <see cref="LayoutRow"/> to a different <see cref="LayoutType"/> synchronously.
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <param name="layoutType"><see cref="LayoutType"/></param>
    /// <returns>Number of records affected.</returns>
    int ChangeLayoutRowTo(LayoutRow row, LayoutType layoutType);
    /// <summary>
    /// Change a <see cref="LayoutRow"/> to a different <see cref="LayoutType"/> asynchronously.
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <param name="layoutType"><see cref="LayoutType"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>Number of records affected.</returns>
    Task<int> ChangeLayoutRowToAsync(LayoutRow row, LayoutType layoutType, CancellationToken token = default);

    /// <summary>
    /// Change a <see cref="LayoutRow"/> to a different <see cref="LayoutType"/> synchronously.
    /// (overloaded method, see <seealso cref="ChangeLayoutRowTo(LayoutRow, LayoutType)"/> )
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <param name="layoutTypeId"></param>
    /// <returns>Number of records affected.</returns>
    int ChangeLayoutRowTo(LayoutRow row, int layoutTypeId);
    /// <summary>
    /// Change a <see cref="LayoutRow"/> to a different <see cref="LayoutType"/> asynchronously.
    /// (overloaded method, see <seealso cref="ChangeLayoutRowToAsync(LayoutRow, LayoutType, CancellationToken)"/> )
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <param name="layoutType">Layout Type ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>Number of records affected.</returns>
    Task<int> ChangeLayoutRowToAsync(LayoutRow row, int layoutType, CancellationToken token = default);

    /// <summary>
    /// Determine whether a <see cref="LayoutRow"/> can be deleted or not. This call is made synchronously.
    /// If a <see cref="LayoutRow"/> contains widgets, it will always return false. Only empty layout rows can be deleted.
    /// </summary>
    /// <param name="tabId"><see cref="DashboardTab"/> ID</param>
    /// <param name="layoutRowId"><see cref="LayoutRow"/> ID</param>
    /// <returns>true if the layout row can be deleted, false if it can't be deleted.</returns>
    bool CanDeleteLayoutRow(Guid tabId, Guid layoutRowId);
    /// <summary>
    /// Determine whether a <see cref="LayoutRow"/> can be deleted or not. This call is made asynchronously.
    /// If a <see cref="LayoutRow"/> contains widgets, it will always return false. Only empty layout rows can be deleted.
    /// </summary>
    /// <param name="tabId"><see cref="DashboardTab"/> ID</param>
    /// <param name="layoutRowId"><see cref="LayoutRow"/> ID</param>
    /// <returns>true if the layout row can be deleted, false if it can't be deleted.</returns>
    Task<bool> CanDeleteLayoutRowAsync(Guid tabId, Guid layoutRowId);

    /// <summary>
    /// Adds a new <see cref="WidgetPlacement"/> synchronously. 
    /// </summary>
    /// <param name="placement"><see cref="WidgetPlacement"/></param>
    /// <returns>Number of records affected.</returns>
    int AddWidgetPlacement(WidgetPlacement placement);
    /// <summary>
    /// Adds a new <see cref="WidgetPlacement"/> asynchronously. 
    /// </summary>
    /// <param name="placement"><see cref="WidgetPlacement"/></param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>Number of records affected.</returns>
    Task<int> AddWidgetPlacementAsync(WidgetPlacement placement, CancellationToken token = default);

    /// <summary>
    /// Determine whether a <see cref="Dashboard"/> exists for a user by their UserID (Guid). This is a synchronous call.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>true if a dashboard does exist for a user, false if the dashboard doesn't exist.</returns>
    bool DashboardExistsFor(Guid id);
    /// <summary>
    /// Determine whether a <see cref="Dashboard"/> exists for a user by their UserID (Guid). This is an asynchronous call.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="token"><see cref="CancellationToken"/> (optional)</param>
    /// <returns>true if a dashboard does exist for a user, false if the dashboard doesn't exist.</returns>
    Task<bool> DashboardExistsForAsync(Guid id, CancellationToken token = default);

}