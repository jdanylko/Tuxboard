using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.Interfaces;

public interface IDashboardService
{
    Dashboard GetDashboardFor(ITuxboardConfig config, Guid userId);
    Task<Dashboard> GetDashboardForAsync(ITuxboardConfig config, Guid userId, CancellationToken token = default);

    Dashboard GetDashboard(ITuxboardConfig config);
    Task<Dashboard> GetDashboardAsync(ITuxboardConfig config, CancellationToken token = default);
        
    Layout GetLayoutFromTab(Guid tabId);
    Task<Layout> GetLayoutFromTabAsync(Guid tabId, CancellationToken token = default);

    List<WidgetPlacement> GetWidgetsForTab(DashboardTab tab);
    Task<List<WidgetPlacement>> GetWidgetsForTabAsync(DashboardTab tab, CancellationToken token = default);

    Dashboard CreateDashboardFrom(DashboardDefault template, Guid? userId = null);
    Task<Dashboard> CreateDashboardFromAsync(DashboardDefault template, Guid? userId = null,
        CancellationToken token = default);

    List<LayoutType> GetLayoutTypes();
    Task<List<LayoutType>> GetLayoutTypesAsync(CancellationToken token = default);

    List<Widget> GetWidgetsFor(int planId=0);
    Task<List<Widget>> GetWidgetsForAsync(int planId = 0, CancellationToken token = default);

    Widget GetWidget(Guid id);
    Task<Widget> GetWidgetAsync(Guid id, CancellationToken token = default);

    List<Widget> GetWidgets();
    Task<List<Widget>> GetWidgetsAsync(CancellationToken token = default);

    WidgetPlacement GetWidgetPlacement(Guid id);
    Task<WidgetPlacement> GetWidgetPlacementAsync(Guid id, CancellationToken token = default);

    bool RemoveLayoutRow(LayoutRow row);
    Task<bool> RemoveLayoutRowAsync(LayoutRow row, CancellationToken token = default);

    bool SaveLayout(Guid tabId, List<LayoutOrder> layoutList);
    Task<bool> SaveLayoutAsync(Guid tabId, List<LayoutOrder> layoutList, CancellationToken token = default);

    bool AddLayoutRow(Layout layout, int layoutTypeId);
    Task<bool> AddLayoutRowAsync(Layout layout, int layoutTypeId, CancellationToken token = default);

    AddWidgetResponse AddWidgetToTab(Guid tabId, Guid widgetId);
    Task<AddWidgetResponse> AddWidgetToTabAsync(Guid tabId, Guid widgetId, CancellationToken token = default);

    bool RemoveWidget(Guid placementId);
    Task<bool> RemoveWidgetAsync(Guid placementId, CancellationToken token = default);

    WidgetPlacement SaveWidgetPlacement(PlacementParameter param);
    Task<WidgetPlacement> SaveWidgetPlacementAsync(PlacementParameter param, CancellationToken token = default);

    WidgetPlacement UpdateCollapsed(Guid id, bool collapsed);
    Task<WidgetPlacement> UpdateCollapsedAsync(Guid id, bool collapsed, CancellationToken token = default);

    List<WidgetSettingDto> SaveWidgetSettings(List<WidgetSetting> settings);
    Task<List<WidgetSettingDto>> SaveWidgetSettingsAsync(List<WidgetSetting> settings,
        CancellationToken token = default);

    int ChangeLayoutRowTo(LayoutRow row, LayoutType layoutType);
    Task<int> ChangeLayoutRowToAsync(LayoutRow row, LayoutType layoutType, CancellationToken token = default);

}