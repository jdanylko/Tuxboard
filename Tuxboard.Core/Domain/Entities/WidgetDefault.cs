using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetDefault
{
    public Guid WidgetDefaultId { get; set; }
    public Guid WidgetId { get; set; }
    public string SettingName { get; set; }
    public string SettingTitle { get; set; }
    public short SettingType { get; set; }
    public string DefaultValue { get; set; }
    public int SettingIndex { get; set; }

    public virtual Widget Widget { get; set; }

    public virtual ICollection<WidgetDefaultOption> WidgetDefaultOptions { get; set; } =
        new HashSet<WidgetDefaultOption>();

    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; } = new HashSet<WidgetSetting>();
}