using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Returns a single <see cref="WidgetSettingDto"/> from a <see cref="WidgetDefault"/> setting
/// </summary>
public partial class WidgetSetting
{
    /// <summary>
    /// Returns a <see cref="WidgetSettingDto"/>
    /// </summary>
    /// <param name="defaultWidget"><see cref="WidgetDefault"/></param>
    /// <returns><see cref="WidgetSettingDto"/></returns>
    public WidgetSettingDto ToDto(WidgetDefault defaultWidget) =>
        new()
        {
            Id = WidgetPlacementId,
            Value = Value,
            Name = defaultWidget.SettingName
        };
}