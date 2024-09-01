using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class LayoutRow
{
    /// <summary>
    /// 
    /// </summary>
    public Guid LayoutRowId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LayoutId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int LayoutTypeId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int RowIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual Layout Layout { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual LayoutType LayoutType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; } = new HashSet<WidgetPlacement>();
}