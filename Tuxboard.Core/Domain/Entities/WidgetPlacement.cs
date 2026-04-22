using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetPlacement
{
    /// <summary>
    /// Constructs a <see cref="WidgetPlacement"/> with a specified column index.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index where this placement resides</param>
    public WidgetPlacement(int columnIndex)
    {
        ColumnIndex = columnIndex;
    }

    /// <summary>Required by EF Core and object-initializer syntax.</summary>
    public WidgetPlacement() { }

    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    public Guid WidgetPlacementId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The <see cref="LayoutRow"/> ID where this placement resides
    /// </summary>
    public Guid LayoutRowId { get; set; }
    /// <summary>
    /// The <see cref="Widget"/> ID this placement is based on
    /// </summary>
    public Guid WidgetId { get; set; }
    /// <summary>
    /// The zero-based column index within the layout row
    /// </summary>
    public int ColumnIndex { get; set; }
    /// <summary>
    /// The display order of this widget within its column
    /// </summary>
    public int WidgetIndex { get; set; }
    /// <summary>
    /// Indicates whether the widget body is collapsed (only the header is visible)
    /// </summary>
    public bool Collapsed { get; set; }
    /// <summary>
    /// Indicates whether this placement uses user-configurable settings
    /// </summary>
    public bool UseSettings { get; set; }
    /// <summary>
    /// Indicates whether this placement renders using a view template
    /// </summary>
    public bool UseTemplate { get; set; }

    /// <summary>
    /// The parent <see cref="LayoutRow"/> containing this placement
    /// </summary>
    public virtual LayoutRow LayoutRow { get; set; } = null!;
    /// <summary>
    /// The <see cref="Widget"/> template this placement is based on
    /// </summary>
    public virtual Widget Widget { get; set; } = null!;
    /// <summary>
    /// Returns a collection of user-configured <see cref="WidgetSetting"/>s for this placement
    /// </summary>
    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; } = new HashSet<WidgetSetting>();
}