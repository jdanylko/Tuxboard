namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Used to retrieve a request to add a new layout row to an existing dashboard.
/// </summary>
public class AddLayoutRowParameter
{
    /// <summary>
    /// Get or set the Layout Type
    /// </summary>
    public string LayoutTypeId { get; set; }

    /// <summary>
    /// Get or set the Tab ID of where to add the new layout row
    /// </summary>
    public string TabId { get; set; }
}