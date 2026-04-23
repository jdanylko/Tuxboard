using System;
using System.ComponentModel.DataAnnotations;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Base class for a Tuxboard dashboard containing the primary key and tab selection state.
/// </summary>
public partial class DashboardBase
{
    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    [Key]
    public Guid DashboardId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The 1-based index of the currently selected tab
    /// </summary>
    public int SelectedTab { get; set; }
}