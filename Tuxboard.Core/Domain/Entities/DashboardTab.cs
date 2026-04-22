using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardTab
{
    /// <summary>
    /// Auto-generated ID (Guid)
    /// </summary>
    public Guid TabId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Reference Id to a <see cref="Dashboard{T}"/>
    /// </summary>
    public Guid DashboardId { get; set; }
    /// <summary>
    /// The title placed on a tab (optional)
    /// </summary>
    public string TabTitle { get; set; } = string.Empty;
    /// <summary>
    /// The order of a tab; usually 1; possible future support of tabs
    /// </summary>
    public int TabIndex { get; set; }

    /// <summary>
    /// Returns a collection of <see cref="Layout"/>s for this tab.
    /// </summary>
    public virtual ICollection<Layout> Layouts { get; set; } = new HashSet<Layout>();
}