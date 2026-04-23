using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Represents a default setting definition for a <see cref="Widget"/>, including the setting name, type, and default value.
/// </summary>
public partial class WidgetDefault
{
    /// <summary>
    /// Auto-generated primary key (Guid)
    /// </summary>
    public Guid WidgetDefaultId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The <see cref="Widget"/> ID this default setting belongs to
    /// </summary>
    public Guid WidgetId { get; set; }
    /// <summary>
    /// The programmatic name of this setting (e.g. "WidgetTitle")
    /// </summary>
    public string SettingName { get; set; } = string.Empty;
    /// <summary>
    /// The human-readable label for this setting shown in the settings UI
    /// </summary>
    public string SettingTitle { get; set; } = string.Empty;
    /// <summary>
    /// The data type of this setting (e.g. text, number, boolean)
    /// </summary>
    public short SettingType { get; set; }
    /// <summary>
    /// The default value applied when a new <see cref="WidgetPlacement"/> is created
    /// </summary>
    public string DefaultValue { get; set; } = string.Empty;
    /// <summary>
    /// The display order of this setting in the settings UI
    /// </summary>
    public int SettingIndex { get; set; }

    /// <summary>
    /// The parent <see cref="Widget"/> that owns this default setting
    /// </summary>
    public virtual Widget Widget { get; set; } = null!;

    /// <summary>
    /// Returns a collection of <see cref="WidgetDefaultOption"/>s for dropdown/select settings
    /// </summary>
    public virtual ICollection<WidgetDefaultOption> WidgetDefaultOptions { get; set; } =
        new HashSet<WidgetDefaultOption>();

    /// <summary>
    /// Returns a collection of <see cref="WidgetSetting"/>s created from this default on widget placements
    /// </summary>
    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; } = new HashSet<WidgetSetting>();
}