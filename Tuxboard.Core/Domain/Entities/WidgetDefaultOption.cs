using System;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Represents a selectable option for a <see cref="WidgetDefault"/> setting that uses a dropdown input.
/// </summary>
public partial class WidgetDefaultOption
{
    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    public Guid WidgetOptionId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The <see cref="WidgetDefault"/> ID this option belongs to
    /// </summary>
    public Guid WidgetDefaultId { get; set; }
    /// <summary>
    /// The human-readable label displayed in the dropdown for this option
    /// </summary>
    public string SettingLabel { get; set; } = string.Empty;
    /// <summary>
    /// The underlying value stored when this option is selected
    /// </summary>
    public string SettingValue { get; set; } = string.Empty;
    /// <summary>
    /// The display order of this option within the dropdown
    /// </summary>
    public int SettingIndex { get; set; }

    /// <summary>
    /// The parent <see cref="WidgetDefault"/> that owns this option
    /// </summary>
    public virtual WidgetDefault WidgetDefault { get; set; } = null!;
}