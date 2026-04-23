using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardDefault
{
    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    public Guid DefaultId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The <see cref="Layout"/> ID associated with this default dashboard template
    /// </summary>
    public Guid LayoutId { get; set; }
    /// <summary>
    /// Optional plan ID linking this default dashboard to a subscriber plan
    /// </summary>
    public int? PlanId { get; set; }

    /// <summary>
    /// The <see cref="Layout"/> used as the template for this default dashboard
    /// </summary>
    public virtual Layout Layout { get; set; } = null!;
    /// <summary>
    /// The optional subscriber <see cref="Plan"/> associated with this default dashboard
    /// </summary>
    public virtual Plan? Plan { get; set; }

    /// <summary>
    /// Returns a collection of <see cref="DashboardDefaultWidget"/>s pre-placed on this default dashboard template
    /// </summary>
    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();
}