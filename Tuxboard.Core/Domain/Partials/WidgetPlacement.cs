using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class WidgetPlacement
    {
        public WidgetPlacementDto ToDto()
        {
            return new WidgetPlacementDto
            {
                ColumnIndex = ColumnIndex,
                LayoutRowId = LayoutRowId,
                WidgetIndex = WidgetIndex,
                WidgetPlacementId = WidgetPlacementId,
                Widget = Widget.ToDto(),
                UseTemplate = Widget.UseTemplate,
                UseSettings = Widget.UseSettings,
                Moveable = Widget.Moveable,
                Collapsed = Collapsed,
                CanDelete = Widget.CanDelete
            };
        }

        private WidgetSetting GetSettingById(string settingId)
        {
            return Enumerable.FirstOrDefault<WidgetSetting>(WidgetSettings, e => e.WidgetSettingId == settingId);
        }

        private WidgetSetting GetSettingByName(string settingName)
        {
            var widgetDefault= Enumerable.FirstOrDefault<WidgetDefault>(Widget.WidgetDefaults, e => e.SettingName.ToLower() == settingName.ToLower());
            return Enumerable.FirstOrDefault<WidgetSetting>(WidgetSettings, t => t.WidgetDefaultId == widgetDefault.WidgetDefaultId);
        }

        public string GetSettingValueById(string settingId)
        {
            var setting = GetSettingById(settingId);
            return setting != null ? setting.Value : string.Empty;
        }

        public string GetSettingValueByName(string settingName)
        {
            var setting = GetSettingByName(settingName);
            return setting != null ? setting.Value : string.Empty;
        }

        public string GetSettingTitleById(string settingId)
        {
            var setting = GetSettingById(settingId);
            var defaultSetting =
                Enumerable.FirstOrDefault<WidgetDefault>(Widget.WidgetDefaults, e => e.WidgetDefaultId == setting.WidgetDefaultId);
            return defaultSetting != null ? defaultSetting.SettingTitle : string.Empty;
        }

        public bool HasSettings => WidgetSettings.Count > 0;

        public bool SettingDefaultsExist => Enumerable.Any<WidgetDefault>(Widget.WidgetDefaults);

        public bool MissingSettings => WidgetSettings.Count != Widget.WidgetDefaults.Count;

        public void UpdateWidgetSettings()
        {
            foreach (var widgetDefault in Widget.WidgetDefaults)
            {
                var setting =
                    Enumerable.FirstOrDefault<WidgetSetting>(WidgetSettings, e => e.WidgetDefaultId == widgetDefault.WidgetDefaultId);
                if (setting == null)
                {
                    WidgetSettings.Add(CreateFrom(widgetDefault));
                }
            }
        }

        public WidgetSetting CreateFrom(WidgetDefault widgetDefault)
        {
            return new WidgetSetting
            {
                WidgetDefaultId = widgetDefault.WidgetDefaultId,
                Value = widgetDefault.DefaultValue,
                WidgetPlacementId = WidgetPlacementId
            };
        }

        public List<WidgetSettingDto> ToSettingsDto()
        {
            return Enumerable.Select(WidgetSettings, setting => new
                {
                    setting,
                    defaultSetting =
                        Enumerable.FirstOrDefault<WidgetDefault>(Widget.WidgetDefaults, e => e.WidgetDefaultId == setting.WidgetDefaultId)
                })
                .Where(t => t.defaultSetting != null)
                .Select(t => new WidgetSettingDto
                {
                    Id = WidgetPlacementId, 
                    Value = t.setting.Value, 
                    Name = t.defaultSetting.SettingName
                })
                .ToList();
        }
    }
}
