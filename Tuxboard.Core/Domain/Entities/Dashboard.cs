using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Represents a Tuxboard dashboard for a specific user.
/// </summary>
/// <typeparam name="T">The type of the user ID</typeparam>
public partial class Dashboard<T> : DashboardBase where T : struct
{
    /// <summary>
    /// The ID of the user who owns this dashboard
    /// </summary>
    public T? UserId { get; set; }

    /// <summary>
    /// Returns a collection of <see cref="DashboardTab"/>s for this dashboard.
    /// </summary>
    public virtual ICollection<DashboardTab> Tabs { get; set; } = new HashSet<DashboardTab>();

}
