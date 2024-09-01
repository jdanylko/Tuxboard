using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetPlacement
{
    /// <summary>
    /// 
    /// </summary>
    public WidgetPlacement()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnIndex"></param>
    public WidgetPlacement(int columnIndex)
    {
        ColumnIndex = columnIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetPlacementId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid LayoutRowId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int ColumnIndex { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int WidgetIndex { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool Collapsed { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool UseSettings { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool UseTemplate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual LayoutRow LayoutRow { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual Widget Widget { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; } = new HashSet<WidgetSetting>();
}