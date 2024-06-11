using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class LayoutRow
{
    public Guid LayoutRowId { get; set; }
    public Guid? LayoutId { get; set; }
    public int LayoutTypeId { get; set; }
    public int RowIndex { get; set; }

    public virtual Layout Layout { get; set; }
    public virtual LayoutType LayoutType { get; set; }

    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();

    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; } = new HashSet<WidgetPlacement>();
}