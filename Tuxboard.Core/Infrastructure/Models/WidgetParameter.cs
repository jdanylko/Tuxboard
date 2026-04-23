namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Represents the parameters posted to a widget action, carrying its placement ID and collapsed state.
/// </summary>
public class WidgetParameter
{
    /// <summary>
    /// Get or set the Id
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Get or set whether a widget is collapsed or not.
    /// </summary>
    public bool Collapsed { get; set; }
}