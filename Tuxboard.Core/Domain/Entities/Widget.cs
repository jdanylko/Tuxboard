using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Widget
{
    /// <summary>
    /// 
    /// </summary>
    public Widget()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ImageUrl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Permission { get; set; }
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
    public bool UseSettings { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool UseTemplate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<WidgetDefault> WidgetDefaults { get; set; } = new HashSet<WidgetDefault>();
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; } = new HashSet<WidgetPlacement>();
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Plan> Plans { get; set; } = new HashSet<Plan>();
}