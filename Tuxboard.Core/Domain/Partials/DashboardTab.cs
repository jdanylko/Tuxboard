using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="DashboardTab"/> is a child to a <see cref="Dashboard{T}"/>.
/// </summary>
/// <remarks>Should contain only one <see cref="Layout"/>.</remarks>
public partial class DashboardTab
{
    /// <summary>
    /// Create a <see cref="DashboardTabDto"/> from a <see cref="DashboardTab"/>
    /// </summary>
    /// <returns><see cref="DashboardTabDto"/></returns>
    public DashboardTabDto ToDto() =>
        new()
        {
            TabId = TabId,
            TabIndex = TabIndex,
            TabTitle = TabTitle,
            Layouts = Layouts.Select(layout => layout.ToDto())
                .OrderBy(t => t.LayoutIndex)
                .ToList(),
            WidgetPlacements = GetWidgetPlacements()
                .Select(wp => wp.ToDto())
                .ToList()
        };

    /// <summary>
    /// Returns the single <see cref="Layout"/> for this tab. Currently only one layout per tab is
    /// supported; multiple layouts are planned for a future release.
    /// </summary>
    /// <returns><see cref="Layout"/></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the tab has no layout, or has more than one layout.
    /// </exception>
    public Layout GetCurrentLayout() =>
        Layouts.Count switch
        {
            0 => throw new InvalidOperationException("DashboardTab has no layout."),
            1 => Layouts.First(),
            _ => throw new InvalidOperationException("DashboardTab has more than one layout.")
        };

    /// <summary>
    /// Return a list of Layouts; Only 1 <see cref="Layout"/> should be contained in 1 <see cref="DashboardTab"/>;
    /// multiple layouts are planned for a future release.
    /// </summary>
    /// <returns><see cref="List{Layout}"/></returns>
    public List<Layout> GetLayouts() => Layouts.ToList();

    /// <summary>
    /// Returns whether a <see cref="LayoutRow"/> contains widgets or not; Used for deleting a <see cref="LayoutRow"/>.
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <returns>true if widgets are in the <see cref="LayoutRow"/>, false if empty</returns>
    public bool RowContainsWidgets(LayoutRow row) 
        => RowContainsWidgets(row.LayoutRowId);

    /// <summary>
    /// Returns whether a <see cref="LayoutRow"/> contains widgets or not by using the layout row id; Used for deleting a <see cref="LayoutRow"/>.
    /// </summary>
    /// <param name="rowId">layout row id</param>
    /// <returns>true if widgets are in the <see cref="LayoutRow"/>, false if empty</returns>
    public bool RowContainsWidgets(Guid rowId) 
        => GetWidgetPlacements().Any(e=>e.LayoutRowId == rowId);

    /// <summary>
    /// Returns all of the <see cref="WidgetPlacement"/>s in a <see cref="Layout"/>
    /// </summary>
    /// <returns><see cref="List{WidgetPlacement}"/></returns>
    public List<WidgetPlacement> GetWidgetPlacements() =>
        GetCurrentLayout().GetWidgetPlacements();
}