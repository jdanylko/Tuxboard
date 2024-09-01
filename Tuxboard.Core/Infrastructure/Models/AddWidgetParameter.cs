namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Used to store a request when a user adds a new widget to their dashboard.
/// </summary>
public class AddWidgetParameter
{
    /// <summary>
    /// Get or Set the UserID
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// Get or set the tab id
    /// </summary>
    public string TabId { get; set; }
    /// <summary>
    /// Get or set the Widget Id to add to their dashboard
    /// </summary>
    public string WidgetId { get; set; }
}