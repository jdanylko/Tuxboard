using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardTab
{
    public Guid TabId { get; set; }
    public Guid DashboardId { get; set; }
    public string TabTitle { get; set; }
    public int TabIndex { get; set; }

    public virtual Dashboard Dashboard { get; set; }
    public virtual ICollection<Layout> Layouts { get; set; } = new HashSet<Layout>();
}