using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels;

/// <summary>
/// ViewModel for a WidgetPlacement's settings; Used for display settings on a WidgetPlacement
/// </summary>
public class SaveSettingsViewModel
{
    /// <summary>
    /// List of a WidgetPlacement's settings
    /// </summary>
    public List<WidgetSetting> Settings { get; set; } = new List<WidgetSetting>();
}