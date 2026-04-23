namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Represents a layout validation error associated with a specific layout row.
/// </summary>
public class LayoutError
{
    /// <summary>
    /// The string representation of the layout row ID where the error occurred
    /// </summary>
    public string LayoutRowId { get; set; } = string.Empty;
    /// <summary>
    /// The error message describing what went wrong
    /// </summary>
    public string Message { get; set; } = string.Empty;
}