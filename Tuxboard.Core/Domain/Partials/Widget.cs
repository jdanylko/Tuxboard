using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="Widget"/> is a contained piece of data meant to convey a message to a user on a dashboard.
///
/// Widgets are different from <see cref="WidgetPlacement"/> where widgets are abstracted classes or templates and widget placements
/// use a widget's structure to create a widget placement on a dashboard.
///
/// One example is an RSS widget. The RSS Widget contains a property called URL. Once an RSS Widget is added to a dashboard,
/// it uses the Widget template and creates a <see cref="WidgetPlacement"/> with a custom URL.
/// There can be multiple RSS <see cref="WidgetPlacement"/>s on a dashboard, but they are created off of a <see cref="Widget"/>.
/// </summary>
public partial class Widget
{
    /// <summary>
    /// Creates an instance of a <see cref="WidgetDto">WidgetDto</see>
    /// </summary>
    /// <returns><see cref="WidgetDto"/></returns>
    public WidgetDto ToDto()
    {
        return new WidgetDto
        {
            Name = this.Name,
            Description = this.Description,
            GroupName = this.GroupName,
            ImageUrl = this.ImageUrl,
            Permission = this.Permission,
            Title = this.Title,
            WidgetId = this.WidgetId
        };
    }
}