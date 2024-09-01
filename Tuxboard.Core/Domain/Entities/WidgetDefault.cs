using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
public partial class WidgetDefault
{
    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetDefaultId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid WidgetId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SettingName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SettingTitle { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public short SettingType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string DefaultValue { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int SettingIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual Widget Widget { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<WidgetDefaultOption> WidgetDefaultOptions { get; set; } =
        new HashSet<WidgetDefaultOption>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; } = new HashSet<WidgetSetting>();
}