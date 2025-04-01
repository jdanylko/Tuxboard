using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class Dashboard<T> : DashboardBase where T : struct
{
    /// <summary>
    /// 
    /// </summary>
    public T? UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<DashboardTab> Tabs { get; set; } = new HashSet<DashboardTab>();

}
