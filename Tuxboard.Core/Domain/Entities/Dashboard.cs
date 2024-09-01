using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Dashboard
{
    /// <summary>
    /// 
    /// </summary>
    public Guid DashboardId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int SelectedTab { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<DashboardTab> Tabs { get; set; } = new HashSet<DashboardTab>();
}