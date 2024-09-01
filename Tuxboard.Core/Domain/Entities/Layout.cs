using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Layout
{
    /// <summary>
    /// AUto-gen Id
    /// </summary>
    public Guid LayoutId { get; set; }

    /// <summary>
    /// The <see cref="DashboardTab"/> Id (parent)
    /// </summary>
    public Guid? TabId { get; set; }
    
    /// <summary>
    /// The index of a layout (should always be one...possible future expansion)
    /// </summary>
    public int LayoutIndex { get; set; }

    /// <summary>
    /// The <see cref="DashboardTab"/>
    /// </summary>
    public virtual DashboardTab Tab { get; set; }

    /// <summary>
    /// Returns a collection of <see cref="DashboardDefault"/>.
    /// </summary>
    public virtual ICollection<DashboardDefault> DashboardDefaults { get; set; } = new HashSet<DashboardDefault>();

    /// <summary>
    /// Returns a collection of <see cref="LayoutRow"/> contained in a <see cref="Layout"/>.
    /// </summary>
    public virtual ICollection<LayoutRow> LayoutRows { get; set; } = new HashSet<LayoutRow>();
}
