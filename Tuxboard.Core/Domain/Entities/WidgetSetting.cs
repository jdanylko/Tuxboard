namespace Tuxboard.Core.Domain.Entities
{
    public partial class WidgetSetting
    {
        public string WidgetSettingId { get; set; }
        public string WidgetPlacementId { get; set; }
        public string WidgetDefaultId { get; set; }
        public string Value { get; set; }

        public virtual WidgetDefault WidgetDefault { get; set; }
        public virtual WidgetPlacement WidgetPlacement { get; set; }
    }
}
