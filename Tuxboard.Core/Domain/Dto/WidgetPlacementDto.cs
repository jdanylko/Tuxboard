using System;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// Data transfer object representing the placement of a widget on a dashboard layout row.
/// </summary>
public class WidgetPlacementDto
{
    /// <summary>
    /// The unique identifier for this widget placement.
    /// </summary>
    public Guid WidgetPlacementId { get; set; }
    /// <summary>
    /// The <see cref="LayoutRow"/> ID that this placement belongs to.
    /// </summary>
    public Guid LayoutRowId { get; set; }
    /// <summary>
    /// The zero-based column index within the layout row.
    /// </summary>
    public int ColumnIndex { get; set; }
    /// <summary>
    /// The zero-based ordering index of the widget within its column.
    /// </summary>
    public int WidgetIndex { get; set; }
    /// <summary>
    /// Whether the widget renders via a Razor partial template.
    /// </summary>
    public bool UseTemplate { get; set; }
    /// <summary>
    /// Whether the widget supports user-configurable settings.
    /// </summary>
    public bool UseSettings { get; set; }
    /// <summary>
    /// Whether the widget is currently collapsed.
    /// </summary>
    public bool Collapsed { get; set; }
    /// <summary>
    /// Whether the widget can be dragged to a different position.
    /// </summary>
    public bool Moveable { get; set; }
    /// <summary>
    /// Whether the widget can be removed from the dashboard.
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// The widget definition associated with this placement.
    /// </summary>
    public WidgetDto Widget { get; set; } = null!;
}