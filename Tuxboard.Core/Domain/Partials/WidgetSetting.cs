using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class WidgetSetting
    {
        public WidgetSettingDto ToDto(WidgetDefault defaultWidget)
        {
            return new WidgetSettingDto
            {
                Id = WidgetPlacementId,
                Value = Value,
                Name = defaultWidget.SettingName
            };
        }
    }
}