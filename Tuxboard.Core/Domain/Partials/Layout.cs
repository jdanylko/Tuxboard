using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class Layout
    {
        public void AddLayoutRow(string layoutTypeId)
        {
            LayoutRows.Add(new LayoutRow
            {
                LayoutId = LayoutId,
                LayoutTypeId = layoutTypeId,
                RowIndex = LayoutRows.Count+1
            });
        }

        public static List<Layout> CreateDefaultLayouts(string tabId, DashboardDefault defaultDashboard)
        {
            return new List<Layout>{
                new Layout {
                    LayoutIndex = defaultDashboard.Layout.LayoutIndex,
                    TabId = tabId,
                    LayoutRows = new List<LayoutRow>(
                        Enumerable.Select<LayoutRow, LayoutRow>(defaultDashboard.Layout.LayoutRows, row => new LayoutRow
                        {
                            RowIndex = row.RowIndex,
                            LayoutTypeId = row.LayoutTypeId,
                            WidgetPlacements = new List<WidgetPlacement>(
                                Enumerable.Where<DashboardDefaultWidget>(defaultDashboard.DashboardDefaultWidgets, y=> y.LayoutRowId == row.LayoutRowId)
                                    .Select(ddw => new WidgetPlacement
                            {
                                WidgetId = ddw.WidgetId,
                                ColumnIndex = ddw.ColumnIndex,
                                UseTemplate = ddw.Widget.UseTemplate,
                                UseSettings = ddw.Widget.UseSettings,
                                Collapsed = false
                            }))
                        }
                    ))
                }
            };
        }

        public LayoutDto ToDto()
        {
            return new LayoutDto
            {
                LayoutId = LayoutId,
                LayoutIndex = LayoutIndex,
                LayoutRows = Enumerable.Select<LayoutRow, LayoutRowDto>(LayoutRows, e => e.ToDto())
                    .OrderBy(y=> y.RowIndex)
                    .ToList()
            };
        }

        public bool ContainsOneRow()
        {
            return LayoutRows.Count == 1;
        }

        public bool RowsContainWidgets(LayoutRow row)
        {
            return Enumerable.Any<WidgetPlacement>(row.WidgetPlacements);
        }

        public List<WidgetPlacement> GetWidgetPlacements()
        {
            return Enumerable.SelectMany<LayoutRow, WidgetPlacement>(LayoutRows, y => y.WidgetPlacements)
                .ToList();
        }

        public List<Widget> GetWidgetsUsed()
        {
            var widgets = Enumerable.SelectMany<LayoutRow, WidgetPlacement>(LayoutRows, y => y.WidgetPlacements)
                .Select<WidgetPlacement, Widget>(e => e.Widget)
                .ToList();

            var widgetIds = widgets
                .Select<Widget, string>(y=> y.WidgetId)
                .Distinct()
                .ToList();

            return widgetIds
                .Select(r => widgets.FirstOrDefault(y => y.WidgetId == r))
                .ToList();
        }

        public WidgetPlacement GetWidgetPlacement(string placementId)
        {
            return Enumerable.SelectMany<LayoutRow, WidgetPlacement>(LayoutRows, e => e.WidgetPlacements)
                .FirstOrDefault(e => e.WidgetPlacementId == placementId);
        }
    }
}
