namespace Tuxboard.Core.Configuration;

/// <summary>
/// Data transfer object for dashboard configuration settings exposed to the client.
/// </summary>
public class TuxboardConfigDto
{
    /// <summary>
    /// Indicates whether users can drag and reposition widgets
    /// </summary>
    public bool Moveable { get; set; }
    /// <summary>
    /// Indicates whether users can remove widgets from their dashboard
    /// </summary>
    public bool DeleteWidgets { get; set; }
    /// <summary>
    /// Indicates whether widgets support user-configurable settings
    /// </summary>
    public bool UseSettings { get; set; }
    /// <summary>
    /// Indicates whether each user has their own personalized dashboard
    /// </summary>
    public bool Personalized { get; set; }
}