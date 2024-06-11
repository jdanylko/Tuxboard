using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Dashboard
{
    public Guid DashboardId { get; set; }
    public int SelectedTab { get; set; }
    public Guid? UserId { get; set; }

    public virtual ICollection<DashboardTab> Tabs { get; set; } = new HashSet<DashboardTab>();
}