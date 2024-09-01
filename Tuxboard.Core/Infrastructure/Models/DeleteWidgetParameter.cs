using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Request parameter to remove a <see cref="Widget"/> from a <see cref="DashboardTab"/>.
/// </summary>
public class DeleteWidgetParameter
{
    /// <summary>
    /// Get or set the user requesting the deletion
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// Get or set the tab id
    /// </summary>
    public string TabId { get; set; }
    
    /// <summary>
    /// Get or set the Widget Placement Id to remove
    /// </summary>
    public string PlacementId { get; set; }
}