using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Widget
{
    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    public Guid WidgetId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The programmatic name used to identify this widget (e.g. component name)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The display title shown in the widget header
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// A short description of the widget's purpose
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// URL to a preview image shown in the widget library
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
    /// <summary>
    /// Optional grouping name for organizing widgets in the widget library
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// The minimum permission level required to view this widget
    /// </summary>
    public int Permission { get; set; }
    /// <summary>
    /// Indicates whether users can drag and reposition this widget
    /// </summary>
    public bool Moveable { get; set; }
    /// <summary>
    /// Indicates whether users can remove this widget from their dashboard
    /// </summary>
    public bool CanDelete { get; set; }
    /// <summary>
    /// Indicates whether this widget supports user-configurable settings
    /// </summary>
    public bool UseSettings { get; set; }
    /// <summary>
    /// Indicates whether this widget uses a view template for rendering
    /// </summary>
    public bool UseTemplate { get; set; }

    /// <summary>
    /// Returns a collection of <see cref="DashboardDefaultWidget"/> entries for default dashboard templates
    /// </summary>
    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();

    /// <summary>
    /// Returns a collection of default settings (<see cref="WidgetDefault"/>) for this widget
    /// </summary>
    public virtual ICollection<WidgetDefault> WidgetDefaults { get; set; } = new HashSet<WidgetDefault>();
    /// <summary>
    /// Returns a collection of active placements (<see cref="WidgetPlacement"/>) of this widget on dashboards
    /// </summary>
    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; } = new HashSet<WidgetPlacement>();
    /// <summary>
    /// Returns a collection of subscriber <see cref="Plan"/>s that have access to this widget
    /// </summary>
    public virtual ICollection<Plan> Plans { get; set; } = new HashSet<Plan>();
}