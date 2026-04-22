using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// View model passed to a widget's partial view, carrying the widget's placement context.
/// </summary>
public class WidgetModel
{
    /// <summary>
    /// The <see cref="WidgetPlacement"/> containing position, settings, and widget metadata for rendering
    /// </summary>
    public WidgetPlacement Placement { get; set; } = null!;
}