using System;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// Data transfer object representing a single widget setting value.
/// </summary>
public class WidgetSettingDto
{
    /// <summary>
    /// The setting name as defined in <see cref="Tuxboard.Core.Domain.Entities.WidgetDefault.SettingName"/>.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The current value of this setting.
    /// </summary>
    public string Value { get; set; } = string.Empty;
    /// <summary>
    /// The unique identifier of the <see cref="Tuxboard.Core.Domain.Entities.WidgetSetting"/> record.
    /// </summary>
    public Guid Id { get; set; }
}