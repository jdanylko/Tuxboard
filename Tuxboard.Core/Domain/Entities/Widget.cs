using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Widget
{
    public Guid WidgetId { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string GroupName { get; set; }
    public int Permission { get; set; }
    public bool Moveable { get; set; }
    public bool CanDelete { get; set; }
    public bool UseSettings { get; set; }
    public bool UseTemplate { get; set; }

    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();

    public virtual ICollection<WidgetDefault> WidgetDefaults { get; set; } = new HashSet<WidgetDefault>();
    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; } = new HashSet<WidgetPlacement>();
    public virtual ICollection<Plan> Plans { get; set; } = new HashSet<Plan>();
}