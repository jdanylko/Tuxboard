namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetDefaultOption
{
    public string WidgetOptionId { get; set; }
    public string WidgetDefaultId { get; set; }
    public string SettingLabel { get; set; }
    public string SettingValue { get; set; }
    public int SettingIndex { get; set; }

    public virtual WidgetDefault WidgetDefault { get; set; }
}