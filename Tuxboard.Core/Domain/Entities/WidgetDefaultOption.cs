using System;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
public partial class WidgetDefaultOption
{
    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetOptionId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetDefaultId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SettingLabel { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SettingValue { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int SettingIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual WidgetDefault WidgetDefault { get; set; }
}