using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Plan
{
    public int PlanId { get; set; }
    public string Title { get; set; }

    public virtual ICollection<DashboardDefault> DashboardDefaults { get; set; } = new HashSet<DashboardDefault>();
    public virtual ICollection<Widget> Widgets { get; set; } = new HashSet<Widget>();
}