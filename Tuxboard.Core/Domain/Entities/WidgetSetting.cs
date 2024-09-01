using System;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetSetting
{
    /// <summary>
    /// Get or set the Widget Setting ID
    /// </summary>
    public Guid WidgetSettingId { get; set; }
    /// <summary>
    /// Get or set the Widget Placement ID
    /// </summary>
    public Guid WidgetPlacementId { get; set; }
    /// <summary>
    /// Get or set the WIdget Default ID
    /// </summary>
    public Guid WidgetDefaultId { get; set; }

    /// <summary>
    /// Get or set the current Value of a widget setting
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Get or set the Widget Default
    /// </summary>
    public virtual WidgetDefault WidgetDefault { get; set; }
    
    /// <summary>
    /// Get or set the Widget Placement
    /// </summary>
    public virtual WidgetPlacement WidgetPlacement { get; set; }
}