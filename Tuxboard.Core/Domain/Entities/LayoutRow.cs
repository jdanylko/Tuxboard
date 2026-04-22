using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class LayoutRow
{
    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    public Guid LayoutRowId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The <see cref="Layout"/> ID this row belongs to
    /// </summary>
    public Guid? LayoutId { get; set; }
    /// <summary>
    /// The <see cref="LayoutType"/> ID that defines the column structure of this row
    /// </summary>
    public int LayoutTypeId { get; set; }
    /// <summary>
    /// The display order of this row within a <see cref="Layout"/>
    /// </summary>
    public int RowIndex { get; set; }

    /// <summary>
    /// The parent <see cref="Layout"/> that contains this row
    /// </summary>
    public virtual Layout Layout { get; set; } = null!;
    /// <summary>
    /// The <see cref="LayoutType"/> that defines the column structure of this row
    /// </summary>
    public virtual LayoutType LayoutType { get; set; } = null!;

    /// <summary>
    /// Returns a collection of <see cref="DashboardDefaultWidget"/>s assigned to this row in a default template
    /// </summary>
    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();

    /// <summary>
    /// Returns a collection of <see cref="WidgetPlacement"/>s positioned in this row
    /// </summary>
    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; } = new HashSet<WidgetPlacement>();
}