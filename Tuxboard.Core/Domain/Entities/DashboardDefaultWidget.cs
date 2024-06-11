using System;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardDefaultWidget
{
    public Guid DefaultWidgetId { get; set; }
    public Guid DashboardDefaultId { get; set; }
    public Guid LayoutRowId { get; set; }
    public Guid WidgetId { get; set; }
    public int ColumnIndex { get; set; }
    public int WidgetIndex { get; set; }

    public virtual DashboardDefault DashboardDefault { get; set; }
    public virtual LayoutRow LayoutRow { get; set; }
    public virtual Widget Widget { get; set; }
}