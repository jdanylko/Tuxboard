using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Creates a template of a widget for a dashboard.
/// In programming terms, a Widget is an abstract class where a WidgetPlacement implements a Widget.
/// </summary>
public partial class WidgetPlacement
{
    /// <summary>
    /// Creates a shallow <see cref="WidgetPlacementDto"/>
    /// </summary>
    /// <returns><see cref="WidgetPlacementDto"/></returns>
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

    /// <summary>
    /// Returns a <see cref="WidgetDefault"/> based on a SettingName
    /// </summary>
    /// <param name="settingName">
    /// The name of the setting. For example, to retrieve the widget's title,
    /// GetDefaultSettingFor("WidgetTitle") would return the title
    /// </param>
    /// <returns><see cref="WidgetDefault"/> if the setting is found, null if it's not found.</returns>
    public WidgetDefault GetDefaultSettingFor(string settingName)
    {
        return Widget?.WidgetDefaults?.FirstOrDefault(e => e.SettingName.ToLower() == settingName.ToLower());
    }

    /// <summary>
    /// Sets the value of a WidgetSetting based on a WidgetDefault.
    /// Locates a Widget Setting based on a widget default when initially created.
    /// When found, the widget setting's Value is assigned.
    /// </summary>
    /// <param name="settingName">Setting Name (i.e. "WidgetTitle")</param>
    /// <param name="val">The setting's value (i.e. "My Projects")</param>
    /// <returns><see cref="WidgetSetting"/> if successfully set, null if not set or not found.</returns>
    public WidgetSetting SetValue(string settingName, string val)
    {
        WidgetSetting current = null;
        var defaultSetting = GetDefaultSettingFor(settingName);
        if (defaultSetting == null)
            return current;

        current = WidgetSettings.FirstOrDefault(e => e.WidgetDefaultId == defaultSetting.WidgetDefaultId);
        if (current != null)
        {
            current.Value = val;
        }

        return current;
    }

    /// <summary>
    /// Return the value of a widget placement's setting by name.
    /// If not found, the default value of the widget's setting is returned.
    /// </summary>
    /// <param name="settingName">The widget's setting name (i.e. "widgetTitle")</param>
    /// <returns>string</returns>
    public string GetSettingOrDefault(string settingName)
    {
        var defaultSetting = GetDefaultSettingFor(settingName);
        if (defaultSetting == null) 
            return string.Empty;

        var current = WidgetSettings.FirstOrDefault(e => e.WidgetDefaultId == defaultSetting.WidgetDefaultId);
        return current != null 
            ? current.Value 
            : defaultSetting.DefaultValue;
    }

    /// <summary>
    /// Returns whether a <see cref="WidgetPlacement"/> has any settings
    /// </summary>
    public bool HasSettings => WidgetSettings.Count > 0;

    /// <summary>
    /// Returns whether a <see cref="Widget"/> has any default settings.
    /// </summary>
    public bool DefaultSettingsExist => Widget.WidgetDefaults.Any();

    /// <summary>
    /// Identifies whether a setting is missing; Compares the <see cref="WidgetSetting"/>.Count to the <see cref="Widget"/>.<see cref="WidgetDefault"/>.Count.
    /// Returns true if the <see cref="WidgetPlacement"/> is missing a setting, false if no settings are missing.
    /// </summary>
    public bool MissingSettings => WidgetSettings.Count != Widget.WidgetDefaults.Count;

    /// <summary>
    /// Creates a <see cref="WidgetSetting"/> based on a <see cref="WidgetDefault"/>; Does NOT save, merely creates the entity.
    /// </summary>
    /// <param name="widgetDefault"><see cref="WidgetDefault"/> to create a new <see cref="WidgetSetting"/></param>
    /// <returns><see cref="WidgetSetting"/></returns>
    public WidgetSetting CreateFrom(WidgetDefault widgetDefault) =>
        new()
        {
            WidgetSettingId = Guid.Empty,
            WidgetDefaultId = widgetDefault.WidgetDefaultId,
            Value = widgetDefault.DefaultValue,
            WidgetPlacementId = WidgetPlacementId
        };


    /// <summary>
    /// Creates missing <see cref="WidgetSetting"/> based on a Widget's Default settings
    /// </summary>
    public void UpdateWidgetSettings()
    {
        foreach (var widgetDefault in Widget.WidgetDefaults)
        {
            var setting =
                WidgetSettings.FirstOrDefault(e => e.WidgetDefaultId == widgetDefault.WidgetDefaultId);
            if (setting == null)
            {
                WidgetSettings.Add(CreateFrom(widgetDefault));
            }
        }
    }

    /// <summary>
    /// Creates <see cref="WidgetSettingDto"/> (Data Transfer Object) based on an existing <see cref="WidgetSetting"/>s
    /// </summary>
    /// <returns><see cref="List{WidgetSettingDto}"/></returns>
    public List<WidgetSettingDto> ToSettingsDto()
    {
        return WidgetSettings.Select(setting => new
            {
                setting,
                defaultSetting =
                    Widget.WidgetDefaults.FirstOrDefault(e => e.WidgetDefaultId == setting.WidgetDefaultId)
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