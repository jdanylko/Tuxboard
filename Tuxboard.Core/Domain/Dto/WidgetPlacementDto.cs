using System;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// 
/// </summary>
public class WidgetPlacementDto
{
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
    public int ColumnIndex { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int WidgetIndex { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool UseTemplate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool UseSettings { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool Collapsed { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool Moveable { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public WidgetDto Widget { get; set; }
}