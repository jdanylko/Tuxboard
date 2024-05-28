using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Widget
{
    public Widget()
    {
        DashboardDefaultWidgets = new HashSet<DashboardDefaultWidget>();
        WidgetDefaults = new HashSet<WidgetDefault>();
        WidgetPlacements = new HashSet<WidgetPlacement>();
        WidgetPlans = new HashSet<WidgetPlan>();
    }

    public string WidgetId { get; set; }
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

    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; }
    public virtual ICollection<WidgetDefault> WidgetDefaults { get; set; }
    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; }
    public virtual ICollection<WidgetPlan> WidgetPlans { get; set; }
}