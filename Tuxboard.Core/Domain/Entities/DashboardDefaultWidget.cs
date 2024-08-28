using System;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="DashboardDefaultWidget"/> is used in conjunction with a <see cref="DashboardDefault"/>.
/// The <see cref="DashboardDefault"/> points to and defines the layout of a default dashboard while
/// the <see cref="DashboardDefaultWidget"/> defines the widgets assigned to default layout rows.
/// </summary>
public partial class DashboardDefaultWidget
{
    /// <summary>
    /// Guid identifier
    /// </summary>
    public Guid DefaultWidgetId { get; set; }

    /// <summary>
    /// <see cref="DashboardDefault"/> ID
    /// </summary>
    public Guid DashboardDefaultId { get; set; }

    /// <summary>
    /// Which <see cref="LayoutRow"/> (by ID) contains this widget?
    /// </summary>
    public Guid LayoutRowId { get; set; }

    /// <summary>
    /// Which <see cref="Widget"/> will be turned into a <see cref="WidgetPlacement"/> on the dashboard?
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// The column (by index) to place the widget
    /// </summary>
    public int ColumnIndex { get; set; }

    /// <summary>
    /// In the column, where should the widget go in order of other widgets?
    /// </summary>
    public int WidgetIndex { get; set; }

    public virtual DashboardDefault DashboardDefault { get; set; }
    public virtual LayoutRow LayoutRow { get; set; }
    public virtual Widget Widget { get; set; }
}