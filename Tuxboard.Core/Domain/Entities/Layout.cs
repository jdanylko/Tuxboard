using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Layout
{
    public Guid LayoutId { get; set; }
    public Guid? TabId { get; set; }
    public int LayoutIndex { get; set; }

    public virtual DashboardTab Tab { get; set; }
    public virtual ICollection<DashboardDefault> DashboardDefaults { get; set; } = new HashSet<DashboardDefault>();
    public virtual ICollection<LayoutRow> LayoutRows { get; set; } = new HashSet<LayoutRow>();
}