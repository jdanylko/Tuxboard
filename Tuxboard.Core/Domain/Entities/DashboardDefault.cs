using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardDefault
{
    public Guid DefaultId { get; set; }
    public Guid LayoutId { get; set; }
    public int? PlanId { get; set; }

    public virtual Layout Layout { get; set; }
    public virtual Plan Plan { get; set; }

    public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; } =
        new HashSet<DashboardDefaultWidget>();
}