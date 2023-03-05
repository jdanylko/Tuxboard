using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
#if NET7_0

#else
using Microsoft.EntityFrameworkCore;
#endif
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Data.Extensions;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITuxDbContext _context;

        public DashboardService(ITuxDbContext context)
        {
            _context = context;
        }

        #region Sync

        public Dashboard GetDashboard(ITuxboardConfig config)
        {
            return _context.GetDashboard(config);
        }

        public Dashboard GetDashboardFor(ITuxboardConfig config, string userId)
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

        public Layout GetLayoutFromTab(string tabId)
        {
            return _context.GetLayoutForTab(tabId);
        }

        public bool RemoveLayoutRow(LayoutRow row)
        {
            var item = _context.LayoutRow.FirstOrDefault(t => t.LayoutRowId == row.LayoutRowId);
            if (item != null)
            {
                _context.LayoutRow.Remove(item);
            }
            return _context.SaveChanges() > 0;
        }

        public Dashboard CreateDashboardFrom(DashboardDefault template, string userId)
        {
            return CreateFromTemplate(template, userId);
        }

        public Dashboard CreateDashboardFrom(DashboardDefault template)
        {
            return CreateFromTemplate(template);
        }

        public Dashboard CreateFromTemplate(DashboardDefault template, string userId = null)
        {
            var dashboard = Dashboard.Create(userId);
            _context.Dashboard.Add(dashboard);
            _context.SaveChanges();

            var currentTab = dashboard.GetCurrentTab();
            var tabId = currentTab.TabId;

            currentTab.Layouts = Layout.CreateDefaultLayouts(tabId, template);

            return dashboard;

        }

        public Widget GetWidget(string id)
        {
            return _context.Widget.FirstOrDefault(e => e.WidgetId == id);
        }

        public WidgetPlacement GetWidgetPlacement(string widgetPlacementId)
        {
            return _context.GetWidgetPlacement(widgetPlacementId);
        }

        public List<WidgetPlacement> GetWidgetsForTab(DashboardTab tab)
        {
            return _context.GetWidgetsForTab(tab);
        }

        public List<LayoutType> GetLayoutTypes()
        {
            return _context.LayoutType.ToList();
        }

        public List<Widget> GetWidgets()
        {
            return _context.Widget.ToList();
        }

        public List<Widget> GetWidgetsFor(int planId=0)
        {
            if (planId > 0)
            {
                return _context.Widget.Where(u => u.WidgetPlans.Any(i => i.PlanId == planId))
                    .ToList();
            }
            return GetWidgets();
        }


        public bool SaveLayout(string tabId, List<LayoutOrder> newList)
        {
            var oldLayout = _context.GetLayoutForTab(tabId);

            var success = true;

            // Delete
            foreach (var layoutRow in oldLayout.LayoutRows.Where(e => newList.All(y => y.LayoutRowId != e.LayoutRowId)))
            {
                var item = _context.LayoutRow.FirstOrDefault(e => e.LayoutRowId == layoutRow.LayoutRowId);
                if (item != null)
                {
                    _context.LayoutRow.Remove(item);
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

            // Add
            foreach (var item in newList.Where(e => e.LayoutRowId == "0"))
            {
                _context.LayoutRow.Add(new LayoutRow
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

            // Update
            foreach (var item in newList)
            {
                var row = _context.LayoutRow.FirstOrDefault(y => y.LayoutRowId == item.LayoutRowId);
                if (row != null)
                {
                    row.RowIndex = item.Index;
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

        public bool AddLayoutRow(Layout layout, string layoutTypeId)
        {
            var layoutRow = new LayoutRow
            {
                LayoutTypeId = layoutTypeId,
                LayoutId = layout.LayoutId,
                RowIndex = layout.LayoutRows.Count
            };
            _context.LayoutRow.Add(layoutRow);
            return _context.SaveChanges() > 0;
        }

        public AddWidgetResponse AddWidgetToTab(string tabId, string widgetId)
        {
            var result = new AddWidgetResponse {Success = false};

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
                WidgetIndex = firstLayoutRow.WidgetPlacements.Count+1,
                WidgetSettings = widget.WidgetDefaults.Select(def => new WidgetSetting
                {
                    Value = def.DefaultValue,
                    WidgetDefaultId = def.WidgetDefaultId,
                    WidgetDefault = def
                }).ToList()
            };

            _context.WidgetPlacement.Add(placement);

            result.Success = _context.SaveChanges() > 0;
            result.PlacementId = placement.WidgetPlacementId;

            return result;
        }

        public bool RemoveWidget(string placementId)
        {
            var placement = _context.GetWidgetPlacement(placementId);
            foreach (var setting in placement.WidgetSettings)
            {
                _context.WidgetSetting.Remove(setting);
            }

            _context.WidgetPlacement.Remove(placement);

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
                    wp.LayoutRowId = param.LayoutRowId;
                    wp.ColumnIndex = param.Column;
                }
                wp.WidgetIndex = plItem.Index;
            }

            _context.SaveChanges();

            return _context.GetWidgetPlacement(param.PlacementId);
        }

        private void UpdatePreviousLayoutOrder(PlacementParameter param)
        {
            // update placements from previous layout.
            var placements = _context.GetPlacementsByLayout(param.PreviousLayout);
            var index = 0;
            foreach (var wp in placements.Where(e => e.ColumnIndex == param.Column).OrderBy(e => e.WidgetIndex))
            {
                wp.WidgetIndex = index++;
            }

            _context.SaveChanges();
        }

        public WidgetPlacement UpdateCollapsed(string id, bool collapsed)
        {
            var item = _context.WidgetPlacement.FirstOrDefault(e => e.WidgetPlacementId == id);
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
                var setting = _context.WidgetSetting.FirstOrDefault(e =>
                    e.WidgetSettingId == widgetSetting.WidgetSettingId);
                if (setting == null) continue;

                setting.Value = widgetSetting.Value;
                _context.SaveChanges();
                result.Add(setting);
            }

            if (!result.Any()) return new List<WidgetSettingDto>();

            var placementId = result.FirstOrDefault()?.WidgetPlacementId;
            var placement = _context.GetWidgetPlacement(placementId);

            return placement.ToSettingsDto();
        }

        #endregion

        #region Async

        public async Task<Dashboard> GetDashboardAsync(ITuxboardConfig config)
        {
            if (!await _context.DashboardExistsAsync())
            {
                // Pass in a planid (int) to pull back specific dashboards.
                // If nothing passed, it'll grab the first Dashboard Template.
                var template = await _context.GetDashboardTemplateForAsync();

                await CreateDashboardFromAsync(template);

                await _context.SaveChangesAsync(new CancellationToken());
            }

            return await _context.GetDashboardAsync(config);
        }

        public async Task<Dashboard> GetDashboardForAsync(ITuxboardConfig config, string userId)
        {
            if (!await _context.DashboardExistsForAsync(userId))
            {
                // Pass in a planid (int) to pull back specific dashboards.
                // If nothing passed, it'll grab the first Dashboard Template.
                var template = await _context.GetDashboardTemplateForAsync();

                await CreateDashboardFromAsync(template, userId);

                await _context.SaveChangesAsync(new CancellationToken());
            }

            return await _context.GetDashboardForAsync(config, userId);
        }

        public Task<Widget> GetWidgetAsync(string id)
        {
            return _context.Widget.FirstOrDefaultAsync(e => e.WidgetId == id);
        }

        public Task<WidgetPlacement> GetWidgetPlacementAsync(string id)
        {
            return _context.GetWidgetPlacementAsync(id);
        }

        public Task<Layout> GetLayoutFromTabAsync(string tabId)
        {
            return _context.GetLayoutForTabAsync(tabId);
        }

        public async Task<bool> RemoveLayoutRowAsync(LayoutRow row)
        {
            var token = new CancellationToken();
            var item = await _context.LayoutRow.FirstOrDefaultAsync(
                t => t.LayoutRowId == row.LayoutRowId, cancellationToken: token);
            if (item != null)
            {
                _context.LayoutRow.Remove(item);
            }
            return await _context.SaveChangesAsync(token) > 0;
        }

        public async Task<bool> SaveLayoutAsync(string tabId, List<LayoutOrder> newList)
        {
            var oldLayout = await _context.GetLayoutForTabAsync(tabId);

            // poor man's synchronization
            var success = true;

            // Delete
            foreach (var layoutRow in oldLayout.LayoutRows.Where(e => newList.All(y => y.LayoutRowId != e.LayoutRowId)))
            {
                var item = await _context.LayoutRow.FirstOrDefaultAsync(e => e.LayoutRowId == layoutRow.LayoutRowId);
                if (item != null)
                {
                    _context.LayoutRow.Remove(item);
                }
                try
                {
                    await _context.SaveChangesAsync(new CancellationToken());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    success = false;
                }
            }

            // Add
            foreach (var item in newList.Where(e => e.LayoutRowId == "0"))
            {
                _context.LayoutRow.Add(new LayoutRow
                {
                    LayoutId = oldLayout.LayoutId,
                    LayoutTypeId = item.TypeId,
                    RowIndex = item.Index
                });
                try
                {
                    await _context.SaveChangesAsync(new CancellationToken());
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
                var row = await _context.LayoutRow.FirstOrDefaultAsync(y => y.LayoutRowId == item.LayoutRowId);
                if (row == null || row.RowIndex == item.Index) 
                    continue;
                
                row.RowIndex = item.Index;
                try
                {
                    await _context.SaveChangesAsync(new CancellationToken());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    success = false;
                }

            }

            return success;
        }

        public Task<Dashboard> CreateDashboardFromAsync(DashboardDefault template, string userId)
        {
            return CreateFromTemplateAsync(template, userId);
        }

        public Task<Dashboard> CreateDashboardFromAsync(DashboardDefault template)
        {
            return CreateFromTemplateAsync(template);
        }

        public async Task<Dashboard> CreateFromTemplateAsync(DashboardDefault template, string userId = null)
        {
            var dashboard = Dashboard.Create(userId);
            await _context.Dashboard.AddAsync(dashboard);
            await _context.SaveChangesAsync(CancellationToken.None);

            var currentTab = dashboard.GetCurrentTab();
            var tabId = currentTab.TabId;

            currentTab.Layouts = Layout.CreateDefaultLayouts(tabId, template);

            return dashboard;

        }

        public async Task<WidgetPlacement> SaveWidgetPlacementAsync(PlacementParameter param)
        {
            var wp = await UpdateNewLayoutOrderAsync(param);

            await UpdatePreviousLayoutOrderAsync(param);

            return wp;
        }

        private async Task<WidgetPlacement> UpdateNewLayoutOrderAsync(PlacementParameter param)
        {
            foreach (var plItem in param.PlacementList)
            {
                var wp = await _context.GetWidgetPlacementAsync(plItem.PlacementId);
                if (wp == null) continue;

                if (wp.WidgetPlacementId == param.PlacementId)
                {
                    wp.LayoutRowId = param.LayoutRowId;
                    wp.ColumnIndex = param.Column;
                }
                wp.WidgetIndex = plItem.Index;
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return await _context.GetWidgetPlacementAsync(param.PlacementId);
        }

        private async Task UpdatePreviousLayoutOrderAsync(PlacementParameter param)
        {
            // update placements from previous layout.
            var placements = await _context.GetPlacementsByLayoutAsync(param.PreviousLayout);
            var index = 0;
            foreach (var wp in placements.Where(e => e.ColumnIndex == param.Column).OrderBy(e=> e.WidgetIndex))
            {
                wp.WidgetIndex = index++;
            }

            await _context.SaveChangesAsync(new CancellationToken());
        }


        public async Task<WidgetPlacement> UpdateCollapsedAsync(string id, bool collapsed)
        {
            var item = _context.WidgetPlacement.FirstOrDefault(e => e.WidgetPlacementId == id);
            if (item == null) 
                return null;

            item.Collapsed = collapsed;
            await _context.SaveChangesAsync(new CancellationToken());

            return item;
        }

        public Task<List<WidgetPlacement>> GetWidgetsForTabAsync(DashboardTab tab) => 
            _context.GetWidgetsForTabAsync(tab);

        public Task<List<LayoutType>> GetLayoutTypesAsync() => 
            _context.LayoutType.ToListAsync();

        public Task<List<Widget>> GetWidgetsAsync() => 
            _context.Widget.ToListAsync();

        public Task<List<Widget>> GetWidgetsForAsync(int planId=0) =>
            planId > 0
                ? _context.Widget.Where(u => u.WidgetPlans.Any(i => i.PlanId == planId))
                    .ToListAsync()
                : GetWidgetsAsync();

        public async Task<bool> AddLayoutRowAsync(Layout layout, string layoutTypeId)
        {
            var layoutRow = new LayoutRow
            {
                LayoutTypeId = layoutTypeId,
                LayoutId = layout.LayoutId,
                RowIndex = layout.LayoutRows.Count+1
            };
            _context.LayoutRow.Add(layoutRow);
            return await _context.SaveChangesAsync(new CancellationToken()) > 0;
        }

        public async Task<AddWidgetResponse> AddWidgetToTabAsync(string tabId, string widgetId)
        {
            var result = new AddWidgetResponse {Success = false};
            
            var layout = await _context.GetLayoutForTabAsync(tabId);
            if (layout == null)
                return result;

            var fullWidget = await _context.GetWidgetAsync(widgetId);
            var firstLayoutRow = layout.LayoutRows.OrderBy(e=> e.RowIndex).FirstOrDefault();
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

            await _context.WidgetPlacement.AddAsync(placement);

            result.Success = await _context.SaveChangesAsync(new CancellationToken()) > 0;
            result.PlacementId = placement.WidgetPlacementId;

            return result;
        }

        public async Task<bool> RemoveWidgetAsync(string placementId)
        {
            var placement = await _context.GetWidgetPlacementAsync(placementId);
            foreach (var setting in placement.WidgetSettings)
            {
                _context.WidgetSetting.Remove(setting);
;           }

            await _context.SaveChangesAsync(new CancellationToken());

            _context.WidgetPlacement.Remove(placement);

            return await _context.SaveChangesAsync(new CancellationToken()) > 0;
        }

        public async Task<List<WidgetSettingDto>> SaveWidgetSettingsAsync(List<WidgetSetting> settings)
        {
            var result = new List<WidgetSetting>();

            foreach (var widgetSetting in settings)
            {
                var setting =
                    await _context.WidgetSetting.FirstOrDefaultAsync(e =>
                        e.WidgetSettingId == widgetSetting.WidgetSettingId);
                if (setting == null) continue;

                setting.Value = widgetSetting.Value;
                await _context.SaveChangesAsync(new CancellationToken());
                result.Add(setting);
            }

            if (!result.Any())
            {
                return new List<WidgetSettingDto>();
            }
            
            var placementId = result.FirstOrDefault().WidgetPlacementId;
            var placement = await _context.GetWidgetPlacementAsync(placementId);

            return placement.ToSettingsDto();
        }

        #endregion

    }
}