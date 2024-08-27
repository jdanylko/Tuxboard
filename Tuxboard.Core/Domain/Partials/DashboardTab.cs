using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardTab
{
    /// <summary>
    /// Create a DashboardTabDto from a DashboardTab
    /// </summary>
    /// <returns>DashboardTabDto</returns>
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
    /// Return a list of Layouts; Should only EVER be 1 Layout in the list; Only 1 Layout should be contained in 1 DashboardTab
    /// </summary>
    /// <returns>Collection of Layouts</returns>
    public List<Layout> GetLayouts() => Layouts.ToList();

    /// <summary>
    /// Returns whether a LayoutRow contains widgets or not; Used for deleting a LayoutRow.
    /// </summary>
    /// <param name="row">Instance of LayoutRow to check</param>
    /// <returns>true if widgets are in the LayoutRow, false if empty</returns>
    public bool RowContainsWidgets(LayoutRow row) 
        => RowContainsWidgets(row.LayoutRowId);

    /// <summary>
    /// Returns whether a LayoutRow contains widgets or not by using the LayoutRowId; Used for deleting a LayoutRow.
    /// </summary>
    /// <param name="rowId">LayoutRowId</param>
    /// <returns>true if widgets are in the LayoutRow, false if empty</returns>
    public bool RowContainsWidgets(Guid rowId) 
        => GetWidgetPlacements().Any(e=>e.LayoutRowId == rowId);

    /// <summary>
    /// Returns the WidgetPlacements
    /// </summary>
    /// <returns>List of WidgetPlacements in a Layout</returns>
    public List<WidgetPlacement> GetWidgetPlacements()
    {
        var layout = Layouts.FirstOrDefault();
        return layout != null 
            ? layout.GetWidgetPlacements() 
            : new List<WidgetPlacement>();
    }
}