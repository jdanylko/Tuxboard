using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Tests.Domain.Entities;

public class WidgetPlacementTests
{
    // Dashboard
    // +-- Dashboard Tab(s)
    // |   +-- Layout(s)
    // |       +-- LayoutRow(s)
    // |           +-- LayoutType
    // |           +-- WidgetPlacements
    // |               +-- WidgetSetting
    // |               +-- Widget
    // |                   +-- WidgetDefault
    // +-- Dashboard Default
    //     +-- Dashboard Default Widgets

    private readonly Dashboard _dashboard = new Dashboard
    {
        SelectedTab = 1,
        Tabs = new List<DashboardTab>
        {
            new DashboardTab
            {
                Layouts = new List<Layout>
                {
                    new Layout
                    {
                        LayoutRows = new List<LayoutRow>
                        {
                            new LayoutRow
                            {
                                LayoutRowId = new Guid("3368B1B7-4DDF-4FBF-8016-8A5CBBDACA88"),
                                LayoutTypeId = 1, // 3 columns, equal
                                LayoutType = new LayoutType { LayoutTypeId = 1, Layout = "col-4/col-4/col-4" },
                                WidgetPlacements = new List<WidgetPlacement>
                                {
                                    new WidgetPlacement
                                    {
                                        WidgetPlacementId = new Guid("C85C0DAA-88FD-4E87-88B3-65A9ADB9B384"),
                                        WidgetId = new Guid("8689039B-6513-4D51-8415-174F1FB01695"),
                                        LayoutRowId = new Guid("3368B1B7-4DDF-4FBF-8016-8A5CBBDACA88"),
                                        WidgetIndex = 0,
                                        ColumnIndex = 0,
                                        Collapsed = false,
                                        UseSettings = true,
                                        UseTemplate = true,
                                        WidgetSettings = new List<WidgetSetting>()
                                        {
                                            new WidgetSetting()
                                            {
                                                WidgetSettingId = new Guid("103F7561-D42B-436A-A688-4E04DC194AE5"),
                                                Value = "Test Title",
                                                WidgetDefaultId = new Guid("E4D987B7-4087-4A8A-AC7F-8A5F3D33BF8D"),
                                                WidgetPlacementId = new Guid("C85C0DAA-88FD-4E87-88B3-65A9ADB9B384")
                                            }
                                        },
                                        Widget = new Widget
                                        {
                                            WidgetId = new Guid("8689039B-6513-4D51-8415-174F1FB01695"),
                                            Name = "testwidget",
                                            Title = "Test Widget",
                                            Description = "A test widget for testing",
                                            GroupName = "Test",
                                            UseSettings = true,
                                            UseTemplate = true,
                                            CanDelete = true,
                                            Moveable = true,
                                            WidgetDefaults = new List<WidgetDefault>
                                            {
                                                new WidgetDefault
                                                {
                                                    WidgetDefaultId = new Guid("E4D987B7-4087-4A8A-AC7F-8A5F3D33BF8D"),
                                                    WidgetId = new Guid("8689039B-6513-4D51-8415-174F1FB01695"),
                                                    DefaultValue = "My Title",
                                                    SettingName = "widgettitle",
                                                    SettingTitle = "Widget Title",
                                                    SettingIndex = 0
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    };

    [Fact]
    public void ReturnWidgetSettingByName()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();        

        // Act
        var result = placement?.GetSettingOrDefault("WidgetTitle");

        // Assert
        Assert.Equal("Test Title", result);
    }

    [Fact]
    public void ReturnWidgetSettingByNameWithMissingSetting()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();
        placement?.WidgetSettings.Clear();

        // Act
        var result = placement?.GetSettingOrDefault("WidgetTitle");

        // Assert
        Assert.Equal("My Title", result);
    }

    [Fact]
    public void ReturnWidgetSettingAfterSettingTitle()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();        

        // Act
        var result = placement?.SetValue("WidgetTitle", "My Test Title");

        // Assert
        Assert.Equal("My Test Title", result?.Value);
    }

    [Fact]
    public void ValidHasSettingsTest()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();        

        // Act
        var result = placement?.HasSettings;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void InvalidHasSettingsTest()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();
        placement?.WidgetSettings.Clear();

        // Act
        var result = placement?.HasSettings;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidDefaultSettingsExistTest()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();

        // Act
        var result = placement?.DefaultSettingsExist;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DefaultSettingsDontExistTest()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();
        placement?.Widget.WidgetDefaults.Clear();

        // Act
        var result = placement?.DefaultSettingsExist;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MissingSettingsTest()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();

        // Act
        var result = placement?.MissingSettings;

        // Assert
        // WidgetDefaults count should equal the WidgetSettings count.
        Assert.False(result);
    }

    [Fact]
    public void CreateSingleWidgetSettingFromWidgetDefault()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();
        var defaultSetting = placement?.GetDefaultSettingFor("widgettitle");

        // Act
        var expected = placement?.CreateFrom(defaultSetting);

        // Assert
        Assert.NotNull(expected);
        Assert.Equal("My Title", expected.Value);
        Assert.IsType<WidgetSetting>(expected);
    }

    [Fact]
    public void CreateWidgetSettingsBasedOnWidgetDefaults()
    {
        // Arrange
        var placement = _dashboard.GetFirstLayoutRow().WidgetPlacements.FirstOrDefault();
        placement?.WidgetSettings.Clear();

        // Act
        placement?.UpdateWidgetSettings();
        var result = placement?.WidgetSettings.FirstOrDefault();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(placement?.WidgetSettings.Count, placement?.Widget.WidgetDefaults.Count);
        Assert.Equal("My Title", result.Value);
    }


}