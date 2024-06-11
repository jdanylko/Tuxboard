using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetPlacement
{
    public Guid WidgetPlacementId { get; set; }
    public Guid LayoutRowId { get; set; }
    public Guid WidgetId { get; set; }
    public int ColumnIndex { get; set; }
    public int WidgetIndex { get; set; }
    public bool Collapsed { get; set; }
    public bool UseSettings { get; set; }
    public bool UseTemplate { get; set; }

    public virtual LayoutRow LayoutRow { get; set; }
    public virtual Widget Widget { get; set; }
    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; } = new HashSet<WidgetSetting>();
}