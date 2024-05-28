using System.Collections.Generic;
using System.Threading.Tasks;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.Interfaces;

public interface IDashboardService
{
    Dashboard GetDashboard(ITuxboardConfig config);
    Task<Dashboard> GetDashboardAsync(ITuxboardConfig config);

    Dashboard GetDashboardFor(ITuxboardConfig config, string userId);
    Task<Dashboard> GetDashboardForAsync(ITuxboardConfig config, string userId);

    Dashboard CreateDashboardFrom(DashboardDefault template, string userId = "");
    Task<Dashboard> CreateDashboardFromAsync(DashboardDefault template, string userId = "");

    Layout GetLayoutFromTab(string tabId);
    Task<Layout> GetLayoutFromTabAsync(string tabId);

    List<WidgetPlacement> GetWidgetsForTab(DashboardTab tab);
    Task<List<WidgetPlacement>> GetWidgetsForTabAsync(DashboardTab tab);

    List<LayoutType> GetLayoutTypes();
    Task<List<LayoutType>> GetLayoutTypesAsync();

    List<Widget> GetWidgetsFor(int planId=0);
    Task<List<Widget>> GetWidgetsForAsync(int planId=0);

    Widget GetWidget(string id);
    Task<Widget> GetWidgetAsync(string id);

    List<Widget> GetWidgets();
    Task<List<Widget>> GetWidgetsAsync();

    WidgetPlacement GetWidgetPlacement(string id);
    Task<WidgetPlacement> GetWidgetPlacementAsync(string id);

    bool AddLayoutRow(Layout layout, string layoutTypeId);
    Task<bool> AddLayoutRowAsync(Layout layout, string layoutTypeId);

    bool RemoveLayoutRow(LayoutRow row);
    Task<bool> RemoveLayoutRowAsync(LayoutRow row);

    bool SaveLayout(string tabId, List<LayoutOrder> layoutList);
    Task<bool> SaveLayoutAsync(string tabId, List<LayoutOrder> layoutList);

    AddWidgetResponse AddWidgetToTab(string tabId, string widgetId);
    Task<AddWidgetResponse> AddWidgetToTabAsync(string tabId, string widgetId);

    bool RemoveWidget(string placementId);
    Task<bool> RemoveWidgetAsync(string placementId);

    WidgetPlacement SaveWidgetPlacement(PlacementParameter param);
    Task<WidgetPlacement> SaveWidgetPlacementAsync(PlacementParameter param);

    WidgetPlacement UpdateCollapsed(string id, bool collapsed);
    Task<WidgetPlacement> UpdateCollapsedAsync(string id, bool collapsed);

    List<WidgetSettingDto> SaveWidgetSettings(List<WidgetSetting> settings);
    Task<List<WidgetSettingDto>> SaveWidgetSettingsAsync(List<WidgetSetting> settings);

}