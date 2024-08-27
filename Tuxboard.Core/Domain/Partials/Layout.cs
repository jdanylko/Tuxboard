using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Layouts contain LayoutRows
/// </summary>
public partial class Layout
{
    /// <summary>
    /// Add a new LayoutRow based on a LayoutType
    /// </summary>
    /// <param name="layoutTypeId">Integer - LayoutTypeId from LayoutType table</param>
    public void AddLayoutRow(int layoutTypeId)
    {
        LayoutRows.Add(new LayoutRow
        {
            LayoutId = LayoutId,
            LayoutTypeId = layoutTypeId,
            RowIndex = LayoutRows.Count + 1
        });
    }

    /// <summary>
    /// Create a default layout for a user based on a default dashboard layout
    /// </summary>
    /// <param name="tabId">An existing dashboard tab id</param>
    /// <param name="defaultDashboard">Instance of a DashboardDefault</param>
    /// <returns></returns>
    public static List<Layout> CreateDefaultLayouts(Guid tabId, DashboardDefault defaultDashboard)
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

    /// <summary>
    /// Creates a LayoutDto (Data Transfer Object)
    /// </summary>
    /// <returns>LayoutDto</returns>
    public LayoutDto ToDto()
    {
        return new()
        {
            LayoutId = LayoutId,
            LayoutIndex = LayoutIndex,
            LayoutRows = LayoutRows.Select<LayoutRow, LayoutRowDto>(e => e.ToDto())
                .OrderBy(y => y.RowIndex)
                .ToList()
        };
    }

    /// <summary>
    /// Returns whether a single LayoutRow exists; Used for the Advanced Layout
    /// example; a dashboard should ALWAYS have at least one LayoutRow.
    /// </summary>
    /// <returns>true if there is only one, false is not</returns>
    public bool ContainsOneRow() => LayoutRows.Count == 1;

    /// <summary>
    /// Returns whether a LayoutRow contains widgets or not; Used for deleting a LayoutRow.
    /// </summary>
    /// <param name="row">Instance of LayoutRow to check</param>
    /// <returns>true if widgets are in the LayoutRow, false if empty</returns>
    public bool RowsContainWidgets(LayoutRow row) => row.WidgetPlacements.Any();

    /// <summary>
    /// Returns whether a LayoutRow contains widgets or not by using the LayoutRowId; Used for deleting a LayoutRow.
    /// </summary>
    /// <param name="layoutRowId">LayoutRowId</param>
    /// <returns>true if widgets are in the LayoutRow, false if empty</returns>
    public bool RowContainsWidgets(Guid layoutRowId)
    {
        var row = LayoutRows.FirstOrDefault(t => t.LayoutRowId == layoutRowId);
        if (row != null)
        {
            return row.RowContainsWidgets();
        }

        return false;
    }

    /// <summary>
    /// Return a list of WidgetPlacements in all LayoutRows
    /// </summary>
    /// <returns>List of WidgetPlacement</returns>
    public List<WidgetPlacement> GetWidgetPlacements() =>
        LayoutRows.SelectMany(y => y.WidgetPlacements)
            .ToList();

    /// <summary>
    /// Return a list of widgets used in every LayoutRow; Good for identifying widgets used by all.
    /// </summary>
    /// <returns>List of Widget types</returns>
    public List<Widget> GetWidgetsUsed()
    {
        var widgets = LayoutRows.SelectMany(y => y.WidgetPlacements)
            .Select(e => e.Widget)
            .ToList();

        var widgetIds = widgets.Select(y => y.WidgetId)
            .Distinct()
            .ToList();

        return widgetIds
            .Select(r => widgets.FirstOrDefault(y => y.WidgetId == r))
            .ToList();
    }

    /// <summary>
    /// Return a WidgetPlacement instance from LayoutRows
    /// </summary>
    /// <param name="placementId">WidgetPlacementId to locate</param>
    /// <returns>Instance of WidgetPlacement if found, null if not found.</returns>
    public WidgetPlacement GetWidgetPlacement(Guid placementId) =>
        GetWidgetPlacements().FirstOrDefault(e => e.WidgetPlacementId == placementId);
}