using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="DashboardTab"/> is a child to a <see cref="Dashboard"/>.
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
    /// Return a list of Layouts; Should only EVER be 1 <see cref="Layout"/> in the list; Only 1 <see cref="Layout"/> should be contained in 1 <see cref="DashboardTab"/>
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
    public List<WidgetPlacement> GetWidgetPlacements()
    {
        var layout = Layouts.FirstOrDefault();
        return layout != null 
            ? layout.GetWidgetPlacements() 
            : new List<WidgetPlacement>();
    }
}