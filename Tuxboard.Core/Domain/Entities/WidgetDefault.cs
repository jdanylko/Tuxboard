using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetDefault
{
    public WidgetDefault()
    {
            WidgetDefaultOptions = new HashSet<WidgetDefaultOption>();
            WidgetSettings = new HashSet<WidgetSetting>();
        }

    public string WidgetDefaultId { get; set; }
    public string WidgetId { get; set; }
    public string SettingName { get; set; }
    public string SettingTitle { get; set; }
    public short SettingType { get; set; }
    public string DefaultValue { get; set; }
    public int SettingIndex { get; set; }

    public virtual Widget Widget { get; set; }
    public virtual ICollection<WidgetDefaultOption> WidgetDefaultOptions { get; set; }
    public virtual ICollection<WidgetSetting> WidgetSettings { get; set; }
}