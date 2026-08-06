using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Services;

namespace Tuxboard.Core.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for critical bug fixes in DashboardService:
///   - RemoveWidget returns false (not NRE) when placement is not found
///   - RemoveWidgetAsync returns false (not NRE) when placement is not found
///   - RemoveWidgetAsync calls SaveChangesAsync exactly once (no double-save)
///   - RemoveWidgetAsync passes the caller's CancellationToken (not a default token)
/// </summary>
public class DashboardServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TuxDbContext<int>> _dbOptions;
    private readonly IOptions<TuxboardConfig> _config;

    public DashboardServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<TuxDbContext<int>>()
            .UseSqlite(_connection)
            .Options;

        _config = Options.Create(new TuxboardConfig { Schema = "dbo" });

        // Create the schema once; all tests share this connection.
        using var ctx = new TestTuxDbContext<int>(_dbOptions, _config);
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private TestTuxDbContext<int> CreateContext() =>
        new(_dbOptions, _config);

    private static DashboardService<int> CreateService(TestTuxDbContext<int> context) =>
        new(context, NullLogger<DashboardService<int>>.Instance);

    /// <summary>
    /// Seeds the minimal required hierarchy so that a WidgetPlacement can be inserted
    /// and later retrieved via GetWidgetPlacement (which uses Include for WidgetSettings
    /// and Widget.WidgetDefaults).
    /// </summary>
    private static (Guid placementId, Guid settingId1, Guid settingId2)
        SeedPlacementWithSettings(TestTuxDbContext<int> context)
    {
        var widgetId      = Guid.NewGuid();
        var widgetDefaultId = Guid.NewGuid();
        var layoutTypeId  = 99;
        var layoutRowId   = Guid.NewGuid();
        var placementId   = Guid.NewGuid();
        var settingId1    = Guid.NewGuid();
        var settingId2    = Guid.NewGuid();

        context.LayoutTypes.Add(new LayoutType
        {
            LayoutTypeId = layoutTypeId,
            Title        = "Test",
            Layout       = "col-12"
        });

        context.Widgets.Add(new Widget
        {
            WidgetId    = widgetId,
            Name        = "test",
            Title       = "Test",
            Description = "Test widget",
            ImageUrl    = "",
            GroupName   = "Test"
        });

        context.WidgetDefaults.Add(new WidgetDefault
        {
            WidgetDefaultId = widgetDefaultId,
            WidgetId        = widgetId,
            SettingName     = "title",
            SettingTitle    = "Title",
            DefaultValue    = "Default"
        });

        context.LayoutRows.Add(new LayoutRow
        {
            LayoutRowId  = layoutRowId,
            LayoutTypeId = layoutTypeId,
            RowIndex     = 0
        });

        context.WidgetPlacements.Add(new WidgetPlacement
        {
            WidgetPlacementId = placementId,
            LayoutRowId       = layoutRowId,
            WidgetId          = widgetId,
            WidgetSettings    = new List<WidgetSetting>
            {
                new()
                {
                    WidgetSettingId   = settingId1,
                    WidgetPlacementId = placementId,
                    WidgetDefaultId   = widgetDefaultId,
                    Value             = "Value 1"
                },
                new()
                {
                    WidgetSettingId   = settingId2,
                    WidgetPlacementId = placementId,
                    WidgetDefaultId   = widgetDefaultId,
                    Value             = "Value 2"
                }
            }
        });

        context.SaveChanges();

        return (placementId, settingId1, settingId2);
    }

    private static Guid SeedPlacementWithNoSettings(TestTuxDbContext<int> context)
    {
        var widgetId     = Guid.NewGuid();
        var layoutTypeId = 98;
        var layoutRowId  = Guid.NewGuid();
        var placementId  = Guid.NewGuid();

        context.LayoutTypes.Add(new LayoutType
        {
            LayoutTypeId = layoutTypeId,
            Title        = "Test2",
            Layout       = "col-12"
        });

        context.Widgets.Add(new Widget
        {
            WidgetId    = widgetId,
            Name        = "test2",
            Title       = "Test 2",
            Description = "Another test widget",
            ImageUrl    = "",
            GroupName   = "Test"
        });

        context.LayoutRows.Add(new LayoutRow
        {
            LayoutRowId  = layoutRowId,
            LayoutTypeId = layoutTypeId,
            RowIndex     = 0
        });

        context.WidgetPlacements.Add(new WidgetPlacement
        {
            WidgetPlacementId = placementId,
            LayoutRowId       = layoutRowId,
            WidgetId          = widgetId
        });

        context.SaveChanges();

        return placementId;
    }

    // -------------------------------------------------------------------------
    // RemoveWidget (sync)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Bug fix: RemoveWidget previously dereferenced the null placement → NRE.
    /// Now it should return false gracefully.
    /// </summary>
    [Fact]
    public void RemoveWidget_WhenPlacementNotFound_ReturnsFalse()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.RemoveWidget(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public void RemoveWidget_WhenPlacementExistsWithSettings_RemovesPlacementAndSettings_ReturnsTrue()
    {
        using var seedCtx = CreateContext();
        var (placementId, settingId1, settingId2) = SeedPlacementWithSettings(seedCtx);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.RemoveWidget(placementId);

        Assert.True(result);

        // Verify placement and both settings are gone.
        using var verifyCtx = CreateContext();
        Assert.Null(verifyCtx.WidgetPlacements.Find(placementId));
        Assert.Null(verifyCtx.WidgetSettings.Find(settingId1));
        Assert.Null(verifyCtx.WidgetSettings.Find(settingId2));
    }

    [Fact]
    public void RemoveWidget_WhenPlacementExistsWithNoSettings_RemovesPlacement_ReturnsTrue()
    {
        using var seedCtx = CreateContext();
        var placementId = SeedPlacementWithNoSettings(seedCtx);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.RemoveWidget(placementId);

        Assert.True(result);

        using var verifyCtx = CreateContext();
        Assert.Null(verifyCtx.WidgetPlacements.Find(placementId));
    }

    // -------------------------------------------------------------------------
    // RemoveWidgetAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// Bug fix: RemoveWidgetAsync previously dereferenced the null placement → NRE.
    /// Now it should return false gracefully.
    /// </summary>
    [Fact]
    public async Task RemoveWidgetAsync_WhenPlacementNotFound_ReturnsFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RemoveWidgetAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveWidgetAsync_WhenPlacementExistsWithSettings_RemovesPlacementAndSettings_ReturnsTrue()
    {
        await using var seedCtx = CreateContext();
        var (placementId, settingId1, settingId2) = SeedPlacementWithSettings(seedCtx);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RemoveWidgetAsync(placementId, CancellationToken.None);

        Assert.True(result);

        await using var verifyCtx = CreateContext();
        Assert.Null(await verifyCtx.WidgetPlacements.FindAsync(new object[] { placementId }, CancellationToken.None));
        Assert.Null(await verifyCtx.WidgetSettings.FindAsync(new object[] { settingId1 }, CancellationToken.None));
        Assert.Null(await verifyCtx.WidgetSettings.FindAsync(new object[] { settingId2 }, CancellationToken.None));
    }

    [Fact]
    public async Task RemoveWidgetAsync_WhenPlacementExistsWithNoSettings_RemovesPlacement_ReturnsTrue()
    {
        await using var seedCtx = CreateContext();
        var placementId = SeedPlacementWithNoSettings(seedCtx);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RemoveWidgetAsync(placementId, CancellationToken.None);

        Assert.True(result);

        await using var verifyCtx = CreateContext();
        Assert.Null(await verifyCtx.WidgetPlacements.FindAsync(new object[] { placementId }, CancellationToken.None));
    }

    /// <summary>
    /// Bug fix: the original RemoveWidgetAsync called SaveChangesAsync twice — once after
    /// removing settings (using a discarded new CancellationToken()) and once after removing
    /// the placement. This created a partial-delete window. The fix saves once at the end
    /// with the caller's token.
    /// </summary>
    [Fact]
    public async Task RemoveWidgetAsync_WhenPlacementExists_CallsSaveChangesAsyncExactlyOnce()
    {
        await using var seedCtx = CreateContext();
        var (placementId, _, _) = SeedPlacementWithSettings(seedCtx);

        await using var context = CreateContext();
        var service = CreateService(context);

        await service.RemoveWidgetAsync(placementId, CancellationToken.None);

        Assert.Equal(1, context.SaveChangesAsyncCallCount);
    }

    /// <summary>
    /// Bug fix: the original code passed "new CancellationToken()" (a discarded default
    /// token) to SaveChangesAsync instead of the caller's token.  The fix forwards the
    /// actual token so that callers can cancel the operation.
    /// </summary>
    [Fact]
    public async Task RemoveWidgetAsync_WhenPlacementExists_ForwardsCallerCancellationToken()
    {
        await using var seedCtx = CreateContext();
        var (placementId, _, _) = SeedPlacementWithSettings(seedCtx);

        using var cts = new CancellationTokenSource();
        await using var context = CreateContext();
        var service = CreateService(context);

        await service.RemoveWidgetAsync(placementId, cts.Token);

        Assert.Single(context.CapturedAsyncTokens);
        Assert.Equal(cts.Token, context.CapturedAsyncTokens[0]);
    }

    // -------------------------------------------------------------------------
    // AddWidgetToTab (sync + async) — null guard for widget and firstLayoutRow
    // -------------------------------------------------------------------------

    /// <summary>
    /// Seeds a DashboardTab with a Layout (with one LayoutRow) but no Widget,
    /// so AddWidgetToTab hits the widget-not-found guard.
    /// Returns tabId and a non-existent widgetId.
    /// </summary>
    private static (Guid tabId, Guid missingWidgetId) SeedTabWithLayoutNoWidget(TestTuxDbContext<int> context)
    {
        var dashboardId  = Guid.NewGuid();
        var tabId        = Guid.NewGuid();
        var layoutId     = Guid.NewGuid();
        var layoutTypeId = 97;
        var layoutRowId  = Guid.NewGuid();

        // Dashboard must exist first: EF infers DashboardTab.DashboardId → Dashboard.DashboardId FK.
        context.Dashboards.Add(new Dashboard<int> { DashboardId = dashboardId });
        context.LayoutTypes.Add(new LayoutType { LayoutTypeId = layoutTypeId, Title = "Test3", Layout = "col-12" });
        context.SaveChanges();

        context.DashboardTabs.Add(new DashboardTab { TabId = tabId, DashboardId = dashboardId, TabIndex = 1, TabTitle = "Tab" });
        context.SaveChanges();

        context.Layouts.Add(new Layout { LayoutId = layoutId, TabId = tabId, LayoutIndex = 1 });
        context.SaveChanges();

        context.LayoutRows.Add(new LayoutRow { LayoutRowId = layoutRowId, LayoutId = layoutId, LayoutTypeId = layoutTypeId });
        context.SaveChanges();

        return (tabId, Guid.NewGuid()); // widgetId doesn't exist in DB
    }

    /// <summary>
    /// Seeds a DashboardTab with a Layout that has NO LayoutRows,
    /// so AddWidgetToTab hits the firstLayoutRow-not-found guard.
    /// Returns tabId and a real widgetId.
    /// </summary>
    private static (Guid tabId, Guid widgetId) SeedTabWithEmptyLayout(TestTuxDbContext<int> context)
    {
        var dashboardId = Guid.NewGuid();
        var tabId    = Guid.NewGuid();
        var layoutId = Guid.NewGuid();
        var widgetId = Guid.NewGuid();

        // Dashboard must exist first: EF infers DashboardTab.DashboardId → Dashboard.DashboardId FK.
        context.Dashboards.Add(new Dashboard<int> { DashboardId = dashboardId });
        context.Widgets.Add(new Widget
        {
            WidgetId = widgetId, Name = "test3", Title = "T3",
            Description = "d", ImageUrl = "", GroupName = "G"
        });
        context.SaveChanges();

        context.DashboardTabs.Add(new DashboardTab { TabId = tabId, DashboardId = dashboardId, TabIndex = 1, TabTitle = "Tab" });
        context.SaveChanges();

        context.Layouts.Add(new Layout { LayoutId = layoutId, TabId = tabId, LayoutIndex = 1 });
        context.SaveChanges();

        return (tabId, widgetId);
    }

    /// <summary>
    /// Bug fix: AddWidgetToTab previously dereferenced widget without a null check.
    /// When the widget does not exist, it should return a failed response, not throw.
    /// </summary>
    [Fact]
    public void AddWidgetToTab_WhenWidgetNotFound_ReturnsFailure()
    {
        using var seedCtx = CreateContext();
        var (tabId, missingWidgetId) = SeedTabWithLayoutNoWidget(seedCtx);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.AddWidgetToTab(tabId, missingWidgetId);

        Assert.False(result.Success);
    }

    /// <summary>
    /// Bug fix: AddWidgetToTab previously dereferenced firstLayoutRow without a null check.
    /// When the layout has no rows, it should return a failed response, not throw.
    /// </summary>
    [Fact]
    public void AddWidgetToTab_WhenNoLayoutRows_ReturnsFailure()
    {
        using var seedCtx = CreateContext();
        var (tabId, widgetId) = SeedTabWithEmptyLayout(seedCtx);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.AddWidgetToTab(tabId, widgetId);

        Assert.False(result.Success);
    }

    /// <summary>
    /// Bug fix: AddWidgetToTabAsync previously dereferenced widget without a null check.
    /// When the widget does not exist, it should return a failed response, not throw.
    /// </summary>
    [Fact]
    public async Task AddWidgetToTabAsync_WhenWidgetNotFound_ReturnsFailure()
    {
        await using var seedCtx = CreateContext();
        var (tabId, missingWidgetId) = SeedTabWithLayoutNoWidget(seedCtx);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.AddWidgetToTabAsync(tabId, missingWidgetId, CancellationToken.None);

        Assert.False(result.Success);
    }

    /// <summary>
    /// Bug fix: AddWidgetToTabAsync previously dereferenced firstLayoutRow without a null check.
    /// When the layout has no rows, it should return a failed response, not throw.
    /// </summary>
    [Fact]
    public async Task AddWidgetToTabAsync_WhenNoLayoutRows_ReturnsFailure()
    {
        await using var seedCtx = CreateContext();
        var (tabId, widgetId) = SeedTabWithEmptyLayout(seedCtx);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.AddWidgetToTabAsync(tabId, widgetId, CancellationToken.None);

        Assert.False(result.Success);
    }

    // -------------------------------------------------------------------------
    // SaveWidgetSettings / SaveWidgetSettingsAsync — single-save (no N+1)
    // -------------------------------------------------------------------------

    private static (Guid settingId1, Guid settingId2) SeedTwoWidgetSettings(TestTuxDbContext<int> context)
    {
        var widgetId        = Guid.NewGuid();
        var widgetDefaultId = Guid.NewGuid();
        var layoutTypeId    = 96;
        var layoutRowId     = Guid.NewGuid();
        var placementId     = Guid.NewGuid();
        var settingId1      = Guid.NewGuid();
        var settingId2      = Guid.NewGuid();

        context.LayoutTypes.Add(new LayoutType { LayoutTypeId = layoutTypeId, Title = "T4", Layout = "col-12" });
        context.Widgets.Add(new Widget { WidgetId = widgetId, Name = "t4", Title = "T4", Description = "d", ImageUrl = "", GroupName = "G" });
        context.WidgetDefaults.Add(new WidgetDefault { WidgetDefaultId = widgetDefaultId, WidgetId = widgetId, SettingName = "s", SettingTitle = "S", DefaultValue = "v" });
        context.LayoutRows.Add(new LayoutRow { LayoutRowId = layoutRowId, LayoutTypeId = layoutTypeId });
        context.WidgetPlacements.Add(new WidgetPlacement { WidgetPlacementId = placementId, LayoutRowId = layoutRowId, WidgetId = widgetId });
        context.WidgetSettings.Add(new WidgetSetting { WidgetSettingId = settingId1, WidgetPlacementId = placementId, WidgetDefaultId = widgetDefaultId, Value = "original1" });
        context.WidgetSettings.Add(new WidgetSetting { WidgetSettingId = settingId2, WidgetPlacementId = placementId, WidgetDefaultId = widgetDefaultId, Value = "original2" });
        context.SaveChanges();

        return (settingId1, settingId2);
    }

    /// <summary>
    /// Bug fix: SaveWidgetSettings previously called SaveChanges inside the loop (once per
    /// setting). The fix loads all settings in one query and saves once at the end.
    /// </summary>
    [Fact]
    public void SaveWidgetSettings_CallsSaveChangesExactlyOnce()
    {
        using var seedCtx = CreateContext();
        var (settingId1, settingId2) = SeedTwoWidgetSettings(seedCtx);

        using var context = CreateContext();
        var service = CreateService(context);

        var updates = new List<WidgetSetting>
        {
            new() { WidgetSettingId = settingId1, Value = "updated1" },
            new() { WidgetSettingId = settingId2, Value = "updated2" }
        };

        service.SaveWidgetSettings(updates);

        Assert.Equal(1, context.SaveChangesCallCount);
    }

    /// <summary>
    /// Bug fix: SaveWidgetSettingsAsync previously called SaveChangesAsync inside the loop.
    /// The fix loads all settings in one query and saves once at the end.
    /// </summary>
    [Fact]
    public async Task SaveWidgetSettingsAsync_CallsSaveChangesAsyncExactlyOnce()
    {
        await using var seedCtx = CreateContext();
        var (settingId1, settingId2) = SeedTwoWidgetSettings(seedCtx);

        await using var context = CreateContext();
        var service = CreateService(context);

        var updates = new List<WidgetSetting>
        {
            new() { WidgetSettingId = settingId1, Value = "updated1" },
            new() { WidgetSettingId = settingId2, Value = "updated2" }
        };

        await service.SaveWidgetSettingsAsync(updates, CancellationToken.None);

        Assert.Equal(1, context.SaveChangesAsyncCallCount);
    }

    // -------------------------------------------------------------------------
    // AddLayoutRow (sync) and AddLayoutRowAsync — RowIndex = Count + 1,
    // exactly one row inserted, single SaveChanges call.
    // Bug fix: sync used Count (0-based) instead of Count + 1 (1-based).
    // -------------------------------------------------------------------------

    /// <summary>
    /// Seeds a Layout with <paramref name="existingRowCount"/> rows using a unique
    /// <paramref name="layoutTypeId"/>.  Returns the Layout with LayoutRows populated
    /// (AsNoTracking so it can be passed freely to a different context).
    /// </summary>
    private Layout SeedLayoutWithRows(TestTuxDbContext<int> context, int layoutTypeId, int existingRowCount)
    {
        var layoutId = Guid.NewGuid();
        context.LayoutTypes.Add(new LayoutType { LayoutTypeId = layoutTypeId, Title = $"LRT{layoutTypeId}", Layout = "col-12" });
        context.Layouts.Add(new Layout { LayoutId = layoutId, LayoutIndex = 1 });
        context.SaveChanges();

        for (var i = 1; i <= existingRowCount; i++)
        {
            context.LayoutRows.Add(new LayoutRow
            {
                LayoutRowId  = Guid.NewGuid(),
                LayoutId     = layoutId,
                LayoutTypeId = layoutTypeId,
                RowIndex     = i
            });
        }

        if (existingRowCount > 0)
            context.SaveChanges();

        return context.Layouts
            .Include(l => l.LayoutRows)
            .AsNoTracking()
            .First(l => l.LayoutId == layoutId);
    }

    /// <summary>
    /// Bug fix: sync AddLayoutRow previously set RowIndex = Count (0 when no rows exist).
    /// It now sets RowIndex = Count + 1, so the first row on an empty layout gets index 1.
    /// </summary>
    [Fact]
    public void AddLayoutRow_WithEmptyLayout_SetsRowIndexToOne()
    {
        using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 1, existingRowCount: 0);

        using var context = CreateContext();
        var service = CreateService(context);

        service.AddLayoutRow(layout, 1);

        using var verifyCtx = CreateContext();
        var newRow = verifyCtx.LayoutRows.Single(r => r.LayoutId == layout.LayoutId);
        Assert.Equal(1, newRow.RowIndex);
    }

    /// <summary>
    /// AddLayoutRow on a layout with N existing rows must assign RowIndex = N + 1.
    /// </summary>
    [Fact]
    public void AddLayoutRow_WithExistingRows_SetsRowIndexToCountPlusOne()
    {
        using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 2, existingRowCount: 2);

        using var context = CreateContext();
        var service = CreateService(context);

        service.AddLayoutRow(layout, 2);

        using var verifyCtx = CreateContext();
        var newRow = verifyCtx.LayoutRows
            .Where(r => r.LayoutId == layout.LayoutId)
            .OrderByDescending(r => r.RowIndex)
            .First();
        Assert.Equal(3, newRow.RowIndex); // 2 existing + 1 = 3
    }

    /// <summary>
    /// AddLayoutRow must insert exactly one new row — no accidental duplicates.
    /// </summary>
    [Fact]
    public void AddLayoutRow_InsertsExactlyOneNewRow()
    {
        using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 3, existingRowCount: 1);
        var countBefore = seedCtx.LayoutRows.Count(r => r.LayoutId == layout.LayoutId);

        using var context = CreateContext();
        var service = CreateService(context);

        service.AddLayoutRow(layout, 3);

        using var verifyCtx = CreateContext();
        var countAfter = verifyCtx.LayoutRows.Count(r => r.LayoutId == layout.LayoutId);
        Assert.Equal(countBefore + 1, countAfter);
    }

    /// <summary>
    /// AddLayoutRow must call SaveChanges exactly once (single round-trip).
    /// </summary>
    [Fact]
    public void AddLayoutRow_CallsSaveChangesExactlyOnce()
    {
        using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 4, existingRowCount: 0);

        using var context = CreateContext();
        var service = CreateService(context);

        service.AddLayoutRow(layout, 4);

        Assert.Equal(1, context.SaveChangesCallCount);
    }

    /// <summary>
    /// Async: first row on an empty layout must get RowIndex = 1 (Count + 1).
    /// </summary>
    [Fact]
    public async Task AddLayoutRowAsync_WithEmptyLayout_SetsRowIndexToOne()
    {
        await using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 5, existingRowCount: 0);

        await using var context = CreateContext();
        var service = CreateService(context);

        await service.AddLayoutRowAsync(layout, 5, CancellationToken.None);

        await using var verifyCtx = CreateContext();
        var newRow = verifyCtx.LayoutRows.Single(r => r.LayoutId == layout.LayoutId);
        Assert.Equal(1, newRow.RowIndex);
    }

    /// <summary>
    /// Async: RowIndex must be N + 1 when N rows already exist.
    /// </summary>
    [Fact]
    public async Task AddLayoutRowAsync_WithExistingRows_SetsRowIndexToCountPlusOne()
    {
        await using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 6, existingRowCount: 2);

        await using var context = CreateContext();
        var service = CreateService(context);

        await service.AddLayoutRowAsync(layout, 6, CancellationToken.None);

        await using var verifyCtx = CreateContext();
        var newRow = verifyCtx.LayoutRows
            .Where(r => r.LayoutId == layout.LayoutId)
            .OrderByDescending(r => r.RowIndex)
            .First();
        Assert.Equal(3, newRow.RowIndex); // 2 existing + 1 = 3
    }

    /// <summary>
    /// Async: AddLayoutRowAsync must insert exactly one new row — no accidental duplicates.
    /// </summary>
    [Fact]
    public async Task AddLayoutRowAsync_InsertsExactlyOneNewRow()
    {
        await using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 7, existingRowCount: 1);
        var countBefore = seedCtx.LayoutRows.Count(r => r.LayoutId == layout.LayoutId);

        await using var context = CreateContext();
        var service = CreateService(context);

        await service.AddLayoutRowAsync(layout, 7, CancellationToken.None);

        await using var verifyCtx = CreateContext();
        var countAfter = verifyCtx.LayoutRows.Count(r => r.LayoutId == layout.LayoutId);
        Assert.Equal(countBefore + 1, countAfter);
    }

    /// <summary>
    /// Async: AddLayoutRowAsync must call SaveChangesAsync exactly once (single round-trip).
    /// </summary>
    [Fact]
    public async Task AddLayoutRowAsync_CallsSaveChangesAsyncExactlyOnce()
    {
        await using var seedCtx = CreateContext();
        var layout = SeedLayoutWithRows(seedCtx, layoutTypeId: 8, existingRowCount: 0);

        await using var context = CreateContext();
        var service = CreateService(context);

        await service.AddLayoutRowAsync(layout, 8, CancellationToken.None);

        Assert.Equal(1, context.SaveChangesAsyncCallCount);
    }

    // -------------------------------------------------------------------------
    // CreateFromTemplate / CreateFromTemplateAsync
    // -------------------------------------------------------------------------

    [Fact]
    public void CreateFromTemplate_WhenTemplateNull_CreatesDashboardAndDefaultLayoutAndCallsSaveChanges()
    {
        using var seedCtx = CreateContext();
        seedCtx.LayoutTypes.Add(new LayoutType { LayoutTypeId = 1, Title = "Default", Layout = "col-12" });
        seedCtx.SaveChanges();

        using var context = CreateContext();
        var service = CreateService(context);
        var userId = 123;

        var dashboard = service.CreateFromTemplate(null, userId);

        Assert.NotNull(dashboard);

        using var verifyCtx = CreateContext();
        Assert.Equal(1, verifyCtx.Dashboards.Count());
        var stored = verifyCtx.Dashboards.First();
        Assert.Equal(userId, stored.UserId);

        Assert.Equal(1, verifyCtx.Layouts.Count());
        Assert.Equal(1, verifyCtx.LayoutRows.Count());
        var row = verifyCtx.LayoutRows.First();
        Assert.Equal(1, row.LayoutTypeId);

        Assert.Equal(2, context.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateFromTemplateAsync_WhenTemplateNull_CreatesDashboardAndDefaultLayoutAndPassesToken()
    {
        await using var seedCtx = CreateContext();
        seedCtx.LayoutTypes.Add(new LayoutType { LayoutTypeId = 1, Title = "Default", Layout = "col-12" });
        await seedCtx.SaveChangesAsync(CancellationToken.None);

        await using var context = CreateContext();
        var service = CreateService(context);
        var userId = 456;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var dashboard = await service.CreateFromTemplateAsync(null, userId, token);

        Assert.NotNull(dashboard);

        await using var verifyCtx = CreateContext();
        Assert.Equal(1, verifyCtx.Dashboards.Count());
        var stored = verifyCtx.Dashboards.First();
        Assert.Equal(userId, stored.UserId);

        Assert.Equal(1, verifyCtx.Layouts.Count());
        Assert.Equal(1, verifyCtx.LayoutRows.Count());
        var row = verifyCtx.LayoutRows.First();
        Assert.Equal(1, row.LayoutTypeId);

        Assert.Equal(2, context.SaveChangesAsyncCallCount);
        Assert.All(context.CapturedAsyncTokens, t => Assert.Equal(token, t));
    }
}
