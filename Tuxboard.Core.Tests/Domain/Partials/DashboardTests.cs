using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Tests.Domain.Partials;

public class DashboardTests
{
    private static readonly Guid RowWithWidgetsId    = new("A1B2C3D4-1000-4000-8000-000000000001");
    private static readonly Guid RowWithoutWidgetsId = new("A1B2C3D4-1000-4000-8000-000000000002");
    private static readonly Guid PlacementId         = new("B2C3D4E5-1000-4000-8000-000000000001");

    // Dashboard with SelectedTab = 1, one tab containing two layout rows:
    //   RowWithWidgetsId    – has one WidgetPlacement
    //   RowWithoutWidgetsId – empty
    private readonly Dashboard<int> _dashboard = new()
    {
        SelectedTab = 1,
        Tabs = new List<DashboardTab>
        {
            new()
            {
                TabIndex = 1,
                Layouts = new List<Layout>
                {
                    new()
                    {
                        LayoutRows = new List<LayoutRow>
                        {
                            new()
                            {
                                LayoutRowId = RowWithWidgetsId,
                                WidgetPlacements = new List<WidgetPlacement>
                                {
                                    new()
                                    {
                                        WidgetPlacementId = PlacementId,
                                        LayoutRowId = RowWithWidgetsId
                                    }
                                }
                            },
                            new()
                            {
                                LayoutRowId = RowWithoutWidgetsId,
                                WidgetPlacements = new List<WidgetPlacement>()
                            }
                        }
                    }
                }
            }
        }
    };

    /// <summary>
    /// When the current tab exists and the row has placements, RowContainsWidgets returns true.
    /// </summary>
    [Fact]
    public void RowContainsWidgets_WithWidgetsInRow_ReturnsTrue()
    {
        Assert.True(_dashboard.RowContainsWidgets(RowWithWidgetsId));
    }

    /// <summary>
    /// When the current tab exists but the row has no placements, RowContainsWidgets returns false.
    /// </summary>
    [Fact]
    public void RowContainsWidgets_WithNoWidgetsInRow_ReturnsFalse()
    {
        Assert.False(_dashboard.RowContainsWidgets(RowWithoutWidgetsId));
    }

    /// <summary>
    /// When SelectedTab points to a non-existent tab (GetCurrentTab returns null),
    /// RowContainsWidgets must return false without throwing.
    /// Bug: original code had "tab == null &amp;&amp;" which caused a NullReferenceException
    /// when tab was null and returned false for all valid tabs.
    /// </summary>
    [Fact]
    public void RowContainsWidgets_WithNoMatchingTab_ReturnsFalse()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 99, // no tab at this index
            Tabs = _dashboard.Tabs
        };

        Assert.False(dashboard.RowContainsWidgets(RowWithWidgetsId));
    }

    // -------------------------------------------------------------------------
    // GetLayoutRow
    // -------------------------------------------------------------------------

    /// <summary>
    /// When a matching layout row exists, GetLayoutRow returns it.
    /// </summary>
    [Fact]
    public void GetLayoutRow_WithMatchingRowId_ReturnsRow()
    {
        var result = _dashboard.GetLayoutRow(RowWithWidgetsId);

        Assert.NotNull(result);
        Assert.Equal(RowWithWidgetsId, result.LayoutRowId);
    }

    /// <summary>
    /// Bug fix: layout from FirstOrDefault was dereferenced without a null check.
    /// When the rowId doesn't exist in any layout, GetLayoutRow must return null
    /// instead of throwing a NullReferenceException.
    /// </summary>
    [Fact]
    public void GetLayoutRow_WithNonExistentRowId_ReturnsNull()
    {
        var result = _dashboard.GetLayoutRow(Guid.NewGuid());

        Assert.Null(result);
    }

    // -------------------------------------------------------------------------
    // ContainsOneRow
    // -------------------------------------------------------------------------

    /// <summary>
    /// When the current tab and layout both exist with one row, ContainsOneRow returns true.
    /// </summary>
    [Fact]
    public void ContainsOneRow_WithSingleRow_ReturnsTrue()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 1,
            Tabs = new List<DashboardTab>
            {
                new()
                {
                    TabIndex = 1,
                    Layouts = new List<Layout>
                    {
                        new()
                        {
                            LayoutRows = new List<LayoutRow>
                            {
                                new() { LayoutRowId = RowWithWidgetsId }
                            }
                        }
                    }
                }
            }
        };

        Assert.True(dashboard.ContainsOneRow());
    }

    /// <summary>
    /// Bug fix: both tab and layout were dereferenced without null checks.
    /// When SelectedTab points to a non-existent tab, ContainsOneRow must
    /// return false instead of throwing a NullReferenceException.
    /// </summary>
    [Fact]
    public void ContainsOneRow_WithNoMatchingTab_ReturnsFalse()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 99, // no tab at this index
            Tabs = _dashboard.Tabs
        };

        Assert.False(dashboard.ContainsOneRow());
    }

    /// <summary>
    /// When the tab exists but has no layouts, ContainsOneRow returns false
    /// without throwing.
    /// </summary>
    [Fact]
    public void ContainsOneRow_WithNoLayouts_ReturnsFalse()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 1,
            Tabs = new List<DashboardTab>
            {
                new() { TabIndex = 1, Layouts = new List<Layout>() }
            }
        };

        Assert.False(dashboard.ContainsOneRow());
    }
}
