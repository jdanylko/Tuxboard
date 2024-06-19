using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Data.Extensions;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ITuxDbContext _context;

    public DashboardService(ITuxDbContext context)
    {
        _context = context;
    }

    #region Sync

    public Dashboard GetDashboard(ITuxboardConfig config) => _context.GetDashboard(config);
    public Layout GetLayoutFromTab(Guid tabId) => _context.GetLayoutForTab(tabId);
    public Dashboard CreateDashboardFrom(DashboardDefault template, Guid? userId) => CreateFromTemplate(template, userId);
    public Widget GetWidget(Guid id) => _context.Widgets.FirstOrDefault(e => e.WidgetId == id);
    public WidgetPlacement GetWidgetPlacement(Guid widgetPlacementId) => _context.GetWidgetPlacement(widgetPlacementId);
    public List<WidgetPlacement> GetWidgetsForTab(DashboardTab tab) => _context.GetWidgetsForTab(tab);
    public List<LayoutType> GetLayoutTypes() => _context.LayoutTypes.ToList();
    public List<Widget> GetWidgets() => _context.Widgets.ToList();

    public Dashboard GetDashboardFor(ITuxboardConfig config, Guid userId)
    {
        if (!_context.DashboardExistsFor(userId))
        {
            // Pass in a planid (int) to pull back specific dashboards.
            // If nothing passed, it'll grab the first Dashboard Template.
            var template = _context.GetDashboardTemplateFor();

            CreateDashboardFrom(template, userId);

            _context.SaveChanges();
        }

        var dashboard = _context.GetDashboardFor(config, userId);
        dashboard.Settings = config;

        return dashboard;
    }

    public bool RemoveLayoutRow(LayoutRow row)
    {
        var item = _context.LayoutRows.FirstOrDefault(t => t.LayoutRowId == row.LayoutRowId);
        if (item != null)
        {
            _context.LayoutRows.Remove(item);
        }
        return _context.SaveChanges() > 0;
    }

    public Dashboard CreateDashboardFrom(DashboardDefault template)
    {
        return CreateFromTemplate(template, null);
    }

    public Dashboard CreateFromTemplate(DashboardDefault template, Guid? userId)
    {
        var dashboard = Dashboard.Create(userId);
        _context.Dashboards.Add(dashboard);
        _context.SaveChanges();

        var currentTab = dashboard.GetCurrentTab();
        var tabId = currentTab.TabId;

        currentTab.Layouts = Layout.CreateDefaultLayouts(tabId, template);

        return dashboard;

    }

    public List<Widget> GetWidgetsFor(int planId = 0)
    {
        if (planId > 0)
        {
            return _context.Widgets.Where(u => u.Plans.Any(i => i.PlanId == planId))
                .ToList();
        }
        return GetWidgets();
    }
    
    public bool SaveLayout(Guid tabId, List<LayoutOrder> newList)
    {
        var oldLayout = _context.GetLayoutForTab(tabId);

        var success = true;

        // Add
        foreach (var item in newList.Where(
                     e => e.LayoutRowId.ToString() == string.Empty
                          || e.LayoutRowId.Equals(Guid.Empty)))
        {
            _context.LayoutRows.Add(new LayoutRow
            {
                LayoutId = oldLayout.LayoutId,
                LayoutTypeId = item.TypeId,
                RowIndex = item.Index
            });
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                success = false;
            }
        }

        // Delete
        foreach (var layoutRow in oldLayout.LayoutRows.Where(e => newList.All(y => y.LayoutRowId != e.LayoutRowId)))
        {
            var loadedRow = _context.LayoutRows
                .Include(r=> r.WidgetPlacements)
                .FirstOrDefault(e => e.LayoutRowId == layoutRow.LayoutRowId);
            if (loadedRow != null && !loadedRow.RowContainsWidgets())
            {
                _context.LayoutRows.Remove(loadedRow);
            }
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                success = false;
            }
        }

        // Update
        foreach (var item in newList)
        {
            var row = _context.LayoutRows.FirstOrDefault(y => y.LayoutRowId == item.LayoutRowId);
            if (row != null)
            {
                row.RowIndex = item.Index;
                row.LayoutTypeId = item.TypeId;
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    success = false;
                }
            }

            if (!success)
                break;
        }

        return success;
    }

    public bool AddLayoutRow(Layout layout, int layoutTypeId)
    {
        var layoutRow = new LayoutRow
        {
            LayoutTypeId = layoutTypeId,
            LayoutId = layout.LayoutId,
            RowIndex = layout.LayoutRows.Count
        };
        _context.LayoutRows.Add(layoutRow);
        return _context.SaveChanges() > 0;
    }

    public AddWidgetResponse AddWidgetToTab(Guid tabId, Guid widgetId)
    {
        var result = new AddWidgetResponse { Success = false };

        var layout = _context.GetLayoutForTab(tabId);
        if (layout == null)
            return result;

        var widget = _context.GetWidget(widgetId);
        var firstLayoutRow = layout.LayoutRows.OrderBy(e => e.RowIndex).FirstOrDefault();
        var placement = new WidgetPlacement
        {
            Collapsed = false,
            LayoutRowId = firstLayoutRow.LayoutRowId,
            ColumnIndex = 0,
            WidgetId = widgetId,
            Widget = widget,
            UseTemplate = widget.UseTemplate,
            UseSettings = widget.UseSettings,
            WidgetIndex = firstLayoutRow.WidgetPlacements.Count + 1,
            WidgetSettings = widget.WidgetDefaults.Select(def => new WidgetSetting
            {
                Value = def.DefaultValue,
                WidgetDefaultId = def.WidgetDefaultId,
                WidgetDefault = def
            }).ToList()
        };

        _context.WidgetPlacements.Add(placement);

        result.Success = _context.SaveChanges() > 0;
        result.PlacementId = placement.WidgetPlacementId;

        return result;
    }

    public bool RemoveWidget(Guid placementId)
    {
        var placement = _context.GetWidgetPlacement(placementId);
        foreach (var setting in placement.WidgetSettings)
        {
            _context.WidgetSettings.Remove(setting);
        }

        _context.WidgetPlacements.Remove(placement);

        return _context.SaveChanges() > 0;
    }

    public WidgetPlacement SaveWidgetPlacement(PlacementParameter param)
    {
        var wp = UpdateNewLayoutOrder(param);

        UpdatePreviousLayoutOrder(param);

        return wp;
    }

    private WidgetPlacement UpdateNewLayoutOrder(PlacementParameter param)
    {
        foreach (var plItem in param.PlacementList)
        {
            var wp = _context.GetWidgetPlacement(plItem.PlacementId);
            if (wp == null) continue;

            if (wp.WidgetPlacementId == param.PlacementId)
            {
                wp.LayoutRowId = param.CurrentLayoutRowId;
                wp.ColumnIndex = param.CurrentColumn;
            }
            wp.WidgetIndex = plItem.Index;
        }

        _context.SaveChanges();

        return _context.GetWidgetPlacement(param.PlacementId);
    }

    private void UpdatePreviousLayoutOrder(PlacementParameter param)
    {
        // update placements from previous layout.
        var placements = _context.GetPlacementsByLayout(param.PreviousLayoutRowId);
        var index = 0;
        foreach (var wp in placements.Where(e => e.ColumnIndex == param.CurrentColumn).OrderBy(e => e.WidgetIndex))
        {
            wp.WidgetIndex = index++;
        }

        _context.SaveChanges();
    }

    public WidgetPlacement UpdateCollapsed(Guid id, bool collapsed)
    {
        var item = _context.WidgetPlacements.FirstOrDefault(e => e.WidgetPlacementId == id);
        if (item == null)
            return null;

        item.Collapsed = collapsed;
        _context.SaveChanges();

        return item;
    }

    public List<WidgetSettingDto> SaveWidgetSettings(List<WidgetSetting> settings)
    {
        var result = new List<WidgetSetting>();

        foreach (var widgetSetting in settings)
        {
            var setting = _context.WidgetSettings.FirstOrDefault(e =>
                e.WidgetSettingId == widgetSetting.WidgetSettingId);
            if (setting == null) continue;

            setting.Value = widgetSetting.Value;
            _context.SaveChanges();
            result.Add(setting);
        }

        if (!result.Any()) return new List<WidgetSettingDto>();

        var placementId = result.FirstOrDefault().WidgetPlacementId;
        var placement = _context.GetWidgetPlacement(placementId);

        return placement.ToSettingsDto();
    }

    public int ChangeLayoutRowTo(LayoutRow row, LayoutType layoutType) 
        => ChangeLayoutRowTo(row, layoutType.LayoutTypeId);

    public int ChangeLayoutRowTo(LayoutRow row, int layoutTypeId)
    {
        var layoutRow = _context.LayoutRows
            .FirstOrDefault(e => e.LayoutRowId.Equals(row.LayoutRowId));
        if (layoutRow != null)
        {
            layoutRow.LayoutTypeId = layoutTypeId;
        }

        return _context.SaveChanges();
    }

    public bool CanDeleteLayoutRow(Guid tabId, Guid layoutRowId)
    {
        const bool result = false;
        if (layoutRowId.Equals(Guid.Empty)) return true;
        var layout = GetLayoutFromTab(tabId);
        if (layout == null) return result;
        var row = layout.LayoutRows.FirstOrDefault(e => e.LayoutRowId.Equals(layoutRowId));
        
        return !row?.RowContainsWidgets() ?? result;
    }

    public int AddWidgetPlacement(WidgetPlacement placement)
    {
        _context.WidgetPlacements.Add(placement);
        return _context.SaveChanges();
    }

    #endregion

    #region Async

    public async Task<Widget> GetWidgetAsync(Guid id, CancellationToken token = default) => await _context.Widgets.FirstOrDefaultAsync(e => e.WidgetId == id, cancellationToken: token);
    public async Task<WidgetPlacement> GetWidgetPlacementAsync(Guid id, CancellationToken token = default) => await _context.GetWidgetPlacementAsync(id, token);
    public async Task<Layout> GetLayoutFromTabAsync(Guid tabId, CancellationToken token = default) => await _context.GetLayoutForTabAsync(tabId, token);
    public async Task<Dashboard> CreateDashboardFromAsync(DashboardDefault template, Guid? userId, CancellationToken token = default) => await CreateFromTemplateAsync(template, userId, token);
    public async Task<Dashboard> CreateDashboardFromAsync(DashboardDefault template, CancellationToken token = default) => await CreateFromTemplateAsync(template, token: token);
    public async Task<List<WidgetPlacement>> GetWidgetsForTabAsync(DashboardTab tab, CancellationToken token = default) => await _context.GetWidgetsForTabAsync(tab, token: token);
    public async Task<List<LayoutType>> GetLayoutTypesAsync(CancellationToken token = default) => await _context.LayoutTypes.ToListAsync(cancellationToken: token);
    public async Task<List<Widget>> GetWidgetsAsync(CancellationToken token = default) => await _context.Widgets.ToListAsync(cancellationToken: token);
    public async Task<List<Widget>> GetWidgetsForAsync(int planId = 0, CancellationToken token = default) =>
        planId > 0
            ? await _context.Widgets.Where(u => u.Plans.Any(i => i.PlanId == planId))
                .ToListAsync(cancellationToken: token)
            : await GetWidgetsAsync(token);

    public async Task<Dashboard> GetDashboardAsync(ITuxboardConfig config, CancellationToken token = default)
    {
        if (!await _context.DashboardExistsAsync(token: token))
        {
            // Pass in a planid (int) to pull back specific dashboards.
            // If nothing passed, it'll grab the first Dashboard Template.
            var template = await _context.GetDashboardTemplateForAsync(token: token);

            await CreateDashboardFromAsync(template, token);

            await _context.SaveChangesAsync(token);
        }

        return await _context.GetDashboardAsync(config, token: token);
    }

    public async Task<Dashboard> GetDashboardForAsync(ITuxboardConfig config, Guid userId, CancellationToken token = default)
    {
        if (!await _context.DashboardExistsForAsync(userId, token: token))
        {
            // Pass in a planid (int) to pull back specific dashboards.
            // If nothing passed, it'll grab the first Dashboard Template.
            var template = await _context.GetDashboardTemplateForAsync(token: token);

            await CreateDashboardFromAsync(template, userId, token);

            await _context.SaveChangesAsync(token);
        }

        return await _context.GetDashboardForAsync(config, userId, token: token);
    }

    public async Task<bool> RemoveLayoutRowAsync(LayoutRow row, CancellationToken token = default)
    {
        var item = await _context.LayoutRows.FirstOrDefaultAsync(
            t => t.LayoutRowId == row.LayoutRowId, cancellationToken: token);
        if (item != null)
        {
            _context.LayoutRows.Remove(item);
        }
        return await _context.SaveChangesAsync(token) > 0;
    }

    public async Task<bool> SaveLayoutAsync(Guid tabId, List<LayoutOrder> newList, CancellationToken token = default)
    {
        var oldLayout = await _context.GetLayoutForTabAsync(tabId, token: token);

        // poor man's synchronization
        var success = true;

        // Perform the "adds" first so we have a complete list of LayoutRows
        // Add
        foreach (var item in newList.Where(
                     e => e.LayoutRowId.ToString() == string.Empty
                          || e.LayoutRowId.Equals(Guid.Empty)))
        {
            _context.LayoutRows.Add(new LayoutRow
            {
                LayoutId = oldLayout.LayoutId,
                LayoutTypeId = item.TypeId,
                RowIndex = item.Index
            });
            try
            {
                await _context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                success = false;
            }
        }

        // Delete
        foreach (var layoutRow in oldLayout.LayoutRows.Where(e => newList.All(y => y.LayoutRowId != e.LayoutRowId)))
        {
            var loadedRow = _context.LayoutRows
                .Include(r => r.WidgetPlacements)
                .FirstOrDefault(e => e.LayoutRowId == layoutRow.LayoutRowId);
            if (loadedRow != null && !loadedRow.RowContainsWidgets())
            {
                _context.LayoutRows.Remove(loadedRow);
            }
            try
            {
                await _context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                success = false;
            }
        }

        // Update
        foreach (var item in newList)
        {
            var row = await _context.LayoutRows.FirstOrDefaultAsync(y => y.LayoutRowId == item.LayoutRowId, cancellationToken: token);
            if (row == null || row.RowIndex == item.Index)
                continue;

            row.RowIndex = item.Index;
            row.LayoutTypeId = item.TypeId;
            try
            {
                await _context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                success = false;
            }

        }

        return success;
    }

    public async Task<Dashboard> CreateFromTemplateAsync(DashboardDefault template, Guid? userId = null, CancellationToken token = default)
    {
        var dashboard = Dashboard.Create(userId);
        await _context.Dashboards.AddAsync(dashboard, token);
        await _context.SaveChangesAsync(token);

        var currentTab = dashboard.GetCurrentTab();
        var tabId = currentTab.TabId;

        currentTab.Layouts = Layout.CreateDefaultLayouts(tabId, template);

        return dashboard;

    }

    public async Task<WidgetPlacement> SaveWidgetPlacementAsync(PlacementParameter param, CancellationToken token = default)
    {
        var wp = await UpdateNewLayoutOrderAsync(param, token);

        await UpdatePreviousLayoutOrderAsync(param, token);

        return wp;
    }

    private async Task<WidgetPlacement> UpdateNewLayoutOrderAsync(PlacementParameter param, CancellationToken token = default)
    {
        foreach (var plItem in param.PlacementList)
        {
            var wp = await _context.GetWidgetPlacementAsync(plItem.PlacementId, token: token);
            if (wp == null) continue;

            if (wp.WidgetPlacementId == param.PlacementId)
            {
                wp.LayoutRowId = param.CurrentLayoutRowId;
                wp.ColumnIndex = param.CurrentColumn;
            }
            wp.WidgetIndex = plItem.Index;
        }

        await _context.SaveChangesAsync(token);

        return await _context.GetWidgetPlacementAsync(param.PlacementId, token: token);
    }

    private async Task UpdatePreviousLayoutOrderAsync(PlacementParameter param, CancellationToken token = default)
    {
        // update placements from previous layout.
        var placements = await _context.GetPlacementsByLayoutAsync(param.PreviousLayoutRowId, token: token);
        var index = 0;
        foreach (var wp in placements.Where(e => e.ColumnIndex == param.CurrentColumn).OrderBy(e => e.WidgetIndex))
        {
            wp.WidgetIndex = index++;
        }

        await _context.SaveChangesAsync(token);
    }

    public async Task<WidgetPlacement> UpdateCollapsedAsync(Guid id, bool collapsed, CancellationToken token = default)
    {
        var item = _context.WidgetPlacements.FirstOrDefault(e => e.WidgetPlacementId == id);
        if (item == null)
            return null;

        item.Collapsed = collapsed;
        await _context.SaveChangesAsync(token);

        return item;
    }

    public async Task<bool> AddLayoutRowAsync(Layout layout, int layoutTypeId, CancellationToken token = default)
    {
        var layoutRow = new LayoutRow
        {
            LayoutTypeId = layoutTypeId,
            LayoutId = layout.LayoutId,
            RowIndex = layout.LayoutRows.Count + 1
        };
        _context.LayoutRows.Add(layoutRow);
        return await _context.SaveChangesAsync(token) > 0;
    }

    public async Task<AddWidgetResponse> AddWidgetToTabAsync(Guid tabId, Guid widgetId, CancellationToken token = default)
    {
        var result = new AddWidgetResponse { Success = false };

        var layout = await _context.GetLayoutForTabAsync(tabId, token: token);
        if (layout == null)
            return result;

        var fullWidget = await _context.GetWidgetAsync(widgetId, token: token);
        var firstLayoutRow = layout.LayoutRows.MinBy(e => e.RowIndex);
        var placement = new WidgetPlacement
        {
            Collapsed = false,
            LayoutRowId = firstLayoutRow.LayoutRowId,
            ColumnIndex = 0,
            WidgetId = widgetId,
            Widget = fullWidget,
            UseTemplate = fullWidget.UseTemplate,
            UseSettings = fullWidget.UseSettings,
            WidgetIndex = firstLayoutRow.WidgetPlacements.Count + 1,
            WidgetSettings = fullWidget.WidgetDefaults.Select(def => new WidgetSetting
            {
                Value = def.DefaultValue,
                WidgetDefaultId = def.WidgetDefaultId,
                WidgetDefault = def
            }).ToList()
        };

        await _context.WidgetPlacements.AddAsync(placement, token);

        result.Success = await _context.SaveChangesAsync(token) > 0;
        result.PlacementId = placement.WidgetPlacementId;

        return result;
    }

    public async Task<bool> RemoveWidgetAsync(Guid placementId, CancellationToken token = default)
    {
        var placement = await _context.GetWidgetPlacementAsync(placementId, token);
        foreach (var setting in placement.WidgetSettings)
        {
            _context.WidgetSettings.Remove(setting);
            ;
        }

        await _context.SaveChangesAsync(new CancellationToken());

        _context.WidgetPlacements.Remove(placement);

        return await _context.SaveChangesAsync(token) > 0;
    }

    public async Task<List<WidgetSettingDto>> SaveWidgetSettingsAsync(List<WidgetSetting> settings, CancellationToken token = default)
    {
        var result = new List<WidgetSetting>();

        foreach (var widgetSetting in settings)
        {
            var setting =
                await _context.WidgetSettings.FirstOrDefaultAsync(e =>
                    e.WidgetSettingId == widgetSetting.WidgetSettingId, cancellationToken: token);
            if (setting == null) continue;

            setting.Value = widgetSetting.Value;
            await _context.SaveChangesAsync(token);
            result.Add(setting);
        }

        if (!result.Any())
        {
            return new List<WidgetSettingDto>();
        }

        var placementId = result.FirstOrDefault().WidgetPlacementId;
        var placement = await _context.GetWidgetPlacementAsync(placementId, token: token);

        return placement.ToSettingsDto();
    }

    public async Task<int> ChangeLayoutRowToAsync(LayoutRow row, LayoutType layoutType,
        CancellationToken token = default) 
        => await ChangeLayoutRowToAsync(row, layoutType.LayoutTypeId, token);

    public async Task<int> ChangeLayoutRowToAsync(LayoutRow row, int layoutTypeId,
        CancellationToken token = default)
    {
        var layoutRow = await _context.LayoutRows
            .FirstOrDefaultAsync(e => e.LayoutRowId.Equals(row.LayoutRowId), cancellationToken: token);
        if (layoutRow != null)
        {
            layoutRow.LayoutTypeId = layoutTypeId;
        }

        return await _context.SaveChangesAsync(token);
    }

    public async Task<bool> CanDeleteLayoutRowAsync(Guid tabId, Guid layoutRowId)
    {
        const bool result = false;
        if (layoutRowId.Equals(Guid.Empty)) return true;
        var layout = await GetLayoutFromTabAsync(tabId);
        if (layout == null) return result;
        var row = layout.LayoutRows.FirstOrDefault(e => e.LayoutRowId.Equals(layoutRowId));

        return !row?.RowContainsWidgets() ?? result;
    }

    public async Task<int> AddWidgetPlacementAsync(WidgetPlacement placement, CancellationToken token = default)
    {
        _context.WidgetPlacements.Add(placement);
        return await _context.SaveChangesAsync(token);
    }

    #endregion

}