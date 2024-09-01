using System;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// DTO of a Widget
/// </summary>
public class WidgetDto
{
    /// <summary>
    /// Get or set a Widget ID
    /// </summary>
    public Guid WidgetId { get; set; }
    /// <summary>
    /// Get or set the name of a widget.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Get or set the title of a widget.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Get or set the description of a widget.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Get or set the url of an image for a widget.
    /// </summary>
    public string ImageUrl { get; set; }
    /// <summary>
    /// Get or set the group name of a widget.
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// Get or set the permission of a widget.
    /// </summary>
    public int Permission { get; set; }
}