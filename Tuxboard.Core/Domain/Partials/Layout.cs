using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="Layout"/> contains a collection of <see cref="Layout.LayoutRows"/>.
/// A <see cref="DashboardTab"/> always contains a single <see cref="Layout"/>. Multiple layouts are not supported.
/// </summary>
public partial class Layout
{
    /// <summary>
    /// Add a new <see cref="LayoutRow"/> based on a <see cref="LayoutType"/>
    /// </summary>
    /// <param name="layoutTypeId">A layout type id referenced through the layout type table</param>
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
    /// Create a default <see cref="Layout"/> for a user based on a <see cref="DashboardDefault"/> layout
    /// </summary>
    /// <param name="tabId">An existing dashboard tab id</param>
    /// <param name="defaultDashboard"><see cref="DashboardDefault"/></param>
    /// <returns><see cref="List{Layout}"/></returns>
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
    /// Creates a <see cref="LayoutDto"/>(Data Transfer Object)
    /// </summary>
    /// <returns><see cref="LayoutDto"/></returns>
    public LayoutDto ToDto() =>
        new()
        {
            LayoutId = LayoutId,
            LayoutIndex = LayoutIndex,
            LayoutRows = LayoutRows.Select<LayoutRow, LayoutRowDto>(e => e.ToDto())
                .OrderBy(y => y.RowIndex)
                .ToList()
        };

    /// <summary>
    /// Returns whether a single <see cref="LayoutRow"/> exists; Used for the Advanced Layout
    /// example; a dashboard should ALWAYS have at least one <see cref="LayoutRow"/> contained in ONE <see cref="Layout"/>
    /// </summary>
    /// <returns>true if there is only one, false is not</returns>
    public bool ContainsOneRow() => LayoutRows.Count == 1;

    /// <summary>
    /// Returns whether a <see cref="LayoutRow"/> contains widgets or not; Used for deleting a <see cref="LayoutRow"/>.
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <returns>true if widgets are in the <see cref="LayoutRow"/>, false if empty</returns>
    public bool RowsContainWidgets(LayoutRow row) => row.WidgetPlacements.Any();

    /// <summary>
    /// Returns whether a <see cref="LayoutRow"/> contains widgets or not by using the layout row id; Used for deleting a <see cref="LayoutRow"/>.
    /// </summary>
    /// <param name="layoutRowId">layout row id</param>
    /// <returns>true if widgets are in the <see cref="LayoutRow"/>, false if empty</returns>
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
    /// Return a list of <see cref="WidgetPlacement"/>s in all <see cref="LayoutRow"/>s
    /// </summary>
    /// <returns><see cref="List{WIdgetPlacement}"/></returns>
    public List<WidgetPlacement> GetWidgetPlacements() =>
        LayoutRows.SelectMany(y => y.WidgetPlacements)
            .ToList();

    /// <summary>
    /// Return a list of distinct widgets (<see cref="Widget"/>s, NOT <see cref="WidgetPlacement"/>s) used
    /// in every <see cref="LayoutRow"/>; Good for identifying widgets used by all.
    /// </summary>
    /// <returns><see cref="List{Widget}"/></returns>
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
    /// Return a <see cref="WidgetPlacement"/> instance from <see cref="LayoutRow"/>s
    /// </summary>
    /// <param name="placementId">Widget Placement Id</param>
    /// <returns><see cref="WidgetPlacement"/> if found, null if not found.</returns>
    public WidgetPlacement GetWidgetPlacement(Guid placementId) =>
        GetWidgetPlacements().FirstOrDefault(e => e.WidgetPlacementId == placementId);
}