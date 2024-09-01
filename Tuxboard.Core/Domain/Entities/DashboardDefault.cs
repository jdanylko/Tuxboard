using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardDefault
{
    /// <summary>
    /// 
    /// </summary>
    public Guid DefaultId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid LayoutId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? PlanId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual Layout Layout { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual Plan Plan { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();
}