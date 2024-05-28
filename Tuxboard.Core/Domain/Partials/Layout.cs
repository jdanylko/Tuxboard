using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

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
        // No default dashboard exists.
        if (defaultDashboard == null)
        {
            return new List<Layout>
            {
                new()
                {
                    LayoutIndex = 1,
                    TabId = tabId
                }
            };
        }

        return new List<Layout>{
            new()
            {
                LayoutIndex = defaultDashboard.Layout.LayoutIndex,
                TabId = tabId,
                LayoutRows = new List<LayoutRow>(
                    defaultDashboard.Layout.LayoutRows.Select(row => new LayoutRow
                    {
                        RowIndex = row.RowIndex,
                        LayoutTypeId = row.LayoutTypeId,
                        WidgetPlacements = new List<WidgetPlacement>(
                            defaultDashboard.DashboardDefaultWidgets.Where(y=> y.LayoutRowId == row.LayoutRowId)
                                .Select(ddw => new WidgetPlacement
                                {
                                    WidgetId = ddw.WidgetId,
                                    ColumnIndex = ddw.ColumnIndex,
                                    UseTemplate = ddw.Widget.UseTemplate,
                                    UseSettings = ddw.Widget.UseSettings,
                                    Collapsed = false
                                }))
                    }))
            }
        };
    }

    public LayoutDto ToDto()
    {
        return new()
        {
            LayoutId = LayoutId,
            LayoutIndex = LayoutIndex,
            LayoutRows = LayoutRows.Select<LayoutRow, LayoutRowDto>(e => e.ToDto())
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
        return row.WidgetPlacements.Any();
    }

    public bool RowContainsWidgets(string layoutRowId)
    {
        var row = LayoutRows.FirstOrDefault(t => t.LayoutRowId == layoutRowId);
        if (row != null)
        {
            return row.RowContainsWidgets();
        }

        return false;
    }

    public List<WidgetPlacement> GetWidgetPlacements()
    {
        return LayoutRows.SelectMany<LayoutRow, WidgetPlacement>(y => y.WidgetPlacements)
            .ToList();
    }

    public List<Widget> GetWidgetsUsed()
    {
        var widgets = LayoutRows.SelectMany<LayoutRow, WidgetPlacement>(y => y.WidgetPlacements)
            .Select<WidgetPlacement, Widget>(e => e.Widget)
            .ToList();

        var widgetIds = widgets.Select(y=> y.WidgetId)
            .Distinct()
            .ToList();

        return widgetIds
            .Select(r => widgets.FirstOrDefault(y => y.WidgetId == r))
            .ToList();
    }

    public WidgetPlacement GetWidgetPlacement(string placementId)
    {
        return LayoutRows.SelectMany(e => e.WidgetPlacements)
            .FirstOrDefault(e => e.WidgetPlacementId == placementId);
    }
}