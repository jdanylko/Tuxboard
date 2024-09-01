namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Used for various Tuxboard notifications (i.e. Tuxbar messages, announcements)
/// </summary>
public record struct TuxViewMessage
{
    /// <summary>
    /// Used to identify view message in the DOM (i.e. id="main-alert")
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Return true whether something succeeded, false if failed
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Return the text to display in the alert message
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Identify the type of alert
    /// </summary>
    public TuxMessageType Type { get; set; }

    /// <summary>
    /// Constructor for TuxViewMessage
    /// </summary>
    /// <param name="text">Return the text to display in the alert message</param>
    /// <param name="type">Identify the type of alert</param>
    /// <param name="success">Return true whether something succeeded, false if failed</param>
    /// <param name="id">Used to identify view message in the DOM (i.e. id="main-alert")</param>
    public TuxViewMessage(string text, TuxMessageType type, bool success = true, string id = "")
    {
            Text = text;
            Type = type;
            Success = success;
            Id = id;
        }
}