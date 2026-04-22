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
    /// When SelectedTab points to a non-existent tab, GetCurrentTab throws an
    /// InvalidOperationException — the invariant requires every dashboard to have a valid tab.
    /// </summary>
    [Fact]
    public void RowContainsWidgets_WithInvalidSelectedTab_Throws()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 99, // no tab at this index
            Tabs = _dashboard.Tabs
        };

        Assert.Throws<InvalidOperationException>(() => dashboard.RowContainsWidgets(RowWithWidgetsId));
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
    /// When SelectedTab points to a non-existent tab, ContainsOneRow throws an
    /// InvalidOperationException — the invariant requires a valid selected tab.
    /// </summary>
    [Fact]
    public void ContainsOneRow_WithInvalidSelectedTab_Throws()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 99, // no tab at this index
            Tabs = _dashboard.Tabs
        };

        Assert.Throws<InvalidOperationException>(() => dashboard.ContainsOneRow());
    }

    /// <summary>
    /// When the tab has no layouts, GetCurrentLayout throws, so ContainsOneRow throws too.
    /// </summary>
    [Fact]
    public void ContainsOneRow_WithNoLayouts_Throws()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 1,
            Tabs = new List<DashboardTab>
            {
                new() { TabIndex = 1, Layouts = new List<Layout>() }
            }
        };

        Assert.Throws<InvalidOperationException>(() => dashboard.ContainsOneRow());
    }

    // -------------------------------------------------------------------------
    // GetCurrentTab invariant
    // -------------------------------------------------------------------------

    /// <summary>
    /// A valid dashboard with one tab returns that tab from GetCurrentTab without throwing.
    /// </summary>
    [Fact]
    public void GetCurrentTab_WithSingleTab_ReturnsNonNull()
    {
        var tab = _dashboard.GetCurrentTab();

        Assert.NotNull(tab);
        Assert.Equal(1, tab.TabIndex);
    }

    /// <summary>
    /// A dashboard with no tabs throws InvalidOperationException from GetCurrentTab.
    /// </summary>
    [Fact]
    public void GetCurrentTab_WithNoTabs_Throws()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 1,
            Tabs = new List<DashboardTab>()
        };

        Assert.Throws<InvalidOperationException>(() => dashboard.GetCurrentTab());
    }

    /// <summary>
    /// A dashboard whose SelectedTab index does not map to any tab throws
    /// InvalidOperationException from GetCurrentTab.
    /// </summary>
    [Fact]
    public void GetCurrentTab_WithInvalidSelectedTab_Throws()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 5, // only one tab exists at index 1
            Tabs = _dashboard.Tabs
        };

        Assert.Throws<InvalidOperationException>(() => dashboard.GetCurrentTab());
    }

    // -------------------------------------------------------------------------
    // Dashboard.Create invariant
    // -------------------------------------------------------------------------

    /// <summary>
    /// Dashboard.Create always produces exactly one tab, satisfying the current invariant.
    /// </summary>
    [Fact]
    public void Dashboard_Create_HasExactlyOneTab()
    {
        var dashboard = Dashboard<int>.Create(42);

        Assert.Single(dashboard.Tabs);
    }

    /// <summary>
    /// The tab created by Dashboard.Create is immediately retrievable via GetCurrentTab.
    /// </summary>
    [Fact]
    public void Dashboard_Create_GetCurrentTab_DoesNotThrow()
    {
        var dashboard = Dashboard<int>.Create(42);

        var tab = dashboard.GetCurrentTab();

        Assert.NotNull(tab);
    }

    // -------------------------------------------------------------------------
    // DashboardTab.GetCurrentLayout invariant
    // -------------------------------------------------------------------------

    /// <summary>
    /// A tab with exactly one layout returns it from GetCurrentLayout without throwing.
    /// </summary>
    [Fact]
    public void GetCurrentLayout_WithSingleLayout_ReturnsNonNull()
    {
        var tab = _dashboard.GetCurrentTab();

        var layout = tab.GetCurrentLayout();

        Assert.NotNull(layout);
    }

    /// <summary>
    /// A tab with no layouts throws InvalidOperationException from GetCurrentLayout.
    /// </summary>
    [Fact]
    public void GetCurrentLayout_WithNoLayouts_Throws()
    {
        var tab = new DashboardTab { TabIndex = 1, Layouts = new List<Layout>() };

        Assert.Throws<InvalidOperationException>(() => tab.GetCurrentLayout());
    }

    /// <summary>
    /// A tab with more than one layout throws InvalidOperationException from GetCurrentLayout;
    /// multiple layouts per tab are not yet supported.
    /// </summary>
    [Fact]
    public void GetCurrentLayout_WithMultipleLayouts_Throws()
    {
        var tab = new DashboardTab
        {
            TabIndex = 1,
            Layouts = new List<Layout> { new(), new() }
        };

        Assert.Throws<InvalidOperationException>(() => tab.GetCurrentLayout());
    }

    // -------------------------------------------------------------------------
    // Layout.LayoutRows — at least one row invariant
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetFirstLayoutRow returns the row when the layout has at least one.
    /// </summary>
    [Fact]
    public void GetFirstLayoutRow_WithLayoutRow_ReturnsNonNull()
    {
        var result = _dashboard.GetFirstLayoutRow();

        Assert.NotNull(result);
    }

    /// <summary>
    /// GetFirstLayoutRow returns null when the single layout has no rows, signalling
    /// an invariant violation in the persisted data.
    /// </summary>
    [Fact]
    public void GetFirstLayoutRow_WithNoLayoutRows_ReturnsNull()
    {
        var dashboard = new Dashboard<int>
        {
            SelectedTab = 1,
            Tabs = new List<DashboardTab>
            {
                new()
                {
                    TabIndex = 1,
                    Layouts = new List<Layout> { new() { LayoutRows = new List<LayoutRow>() } }
                }
            }
        };

        var result = dashboard.GetFirstLayoutRow();

        Assert.Null(result);
    }
}
