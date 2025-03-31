using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
public partial class DashboardBase
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
    public virtual ICollection<DashboardTab> Tabs { get; set; } = new HashSet<DashboardTab>();
}