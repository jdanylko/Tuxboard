using System;

namespace Tuxboard.Core.Domain.Entities;

public partial class WidgetSetting
{
    public Guid WidgetSettingId { get; set; }
    public Guid WidgetPlacementId { get; set; }
    public Guid WidgetDefaultId { get; set; }
    public string Value { get; set; }

    public virtual WidgetDefault WidgetDefault { get; set; }
    public virtual WidgetPlacement WidgetPlacement { get; set; }
}