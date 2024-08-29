using System;

namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Result of the AddWidget request returned back to the user.
/// </summary>
public class AddWidgetResponse
{
    /// <summary>
    /// Get or set the result of adding the widget
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Get or set the WidgetPlacementId created.
    /// </summary>
    public Guid PlacementId { get; set; }
}