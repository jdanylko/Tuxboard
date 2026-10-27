using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.Infrastructure.Services;

namespace Tuxboard.Core.Tests.Infrastructure.Services;

/// <summary>
/// Tests for layout-row behavior in DashboardService:
///   - RemoveLayoutRow enforces "at least one row" using the stored LayoutId, not the caller's
///   - SaveLayoutAsync persists a LayoutType change even when the row index is unchanged
///   - SaveWidgetPlacement returns null (not NRE) when the placement does not exist
/// </summary>
public class DashboardServiceLayoutTests : IDisposable
{
    private const int TypeA = 201;
    private const int TypeB = 202;

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TuxDbContext<int>> _dbOptions;
    private readonly IOptions<TuxboardConfig> _config;

    public DashboardServiceLayoutTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<TuxDbContext<int>>()
            .UseSqlite(_connection)
            .Options;

        _config = Options.Create(new TuxboardConfig { Schema = "dbo" });

        using var ctx = new TestTuxDbContext<int>(_dbOptions, _config);
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private TestTuxDbContext<int> CreateContext() => new(_dbOptions, _config);

    private static DashboardService<int> CreateService(TestTuxDbContext<int> context) =>
        new(context, NullLogger<DashboardService<int>>.Instance);

    /// <summary>
    /// Seeds a dashboard, tab and layout containing <paramref name="rowCount"/> rows
    /// (all of type <see cref="TypeA"/>, RowIndex 1..n).
    /// </summary>
    private static (Guid tabId, Guid layoutId, List<Guid> rowIds) SeedLayout(
        TestTuxDbContext<int> context, int rowCount)
    {
        var dashboardId = Guid.NewGuid();
        var tabId       = Guid.NewGuid();
        var layoutId    = Guid.NewGuid();

        context.Dashboards.Add(new Dashboard<int> { DashboardId = dashboardId });
        context.LayoutTypes.Add(new LayoutType { LayoutTypeId = TypeA, Title = "A", Layout = "col-12" });
        context.LayoutTypes.Add(new LayoutType { LayoutTypeId = TypeB, Title = "B", Layout = "col-6" });
        context.SaveChanges();

        context.DashboardTabs.Add(new DashboardTab
            { TabId = tabId, DashboardId = dashboardId, TabIndex = 1, TabTitle = "Tab" });
        context.SaveChanges();

        context.Layouts.Add(new Layout { LayoutId = layoutId, TabId = tabId, LayoutIndex = 1 });
        context.SaveChanges();

        var rowIds = new List<Guid>();
        for (var i = 1; i <= rowCount; i++)
        {
            var rowId = Guid.NewGuid();
            rowIds.Add(rowId);
            context.LayoutRows.Add(new LayoutRow
                { LayoutRowId = rowId, LayoutId = layoutId, LayoutTypeId = TypeA, RowIndex = i });
        }
        context.SaveChanges();

        return (tabId, layoutId, rowIds);
    }

    // -------------------------------------------------------------------------
    // RemoveLayoutRow
    // -------------------------------------------------------------------------

    [Fact]
    public void RemoveLayoutRow_WhenRowNotFound_ReturnsFalse()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.RemoveLayoutRow(new LayoutRow { LayoutRowId = Guid.NewGuid() });

        Assert.False(result);
    }

    [Fact]
    public void RemoveLayoutRow_WhenLayoutHasMultipleRows_RemovesRow()
    {
        using var seedCtx = CreateContext();
        var (_, layoutId, rowIds) = SeedLayout(seedCtx, 2);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.RemoveLayoutRow(new LayoutRow { LayoutRowId = rowIds[0], LayoutId = layoutId });

        Assert.True(result);
        using var verifyCtx = CreateContext();
        Assert.Null(verifyCtx.LayoutRows.Find(rowIds[0]));
        Assert.NotNull(verifyCtx.LayoutRows.Find(rowIds[1]));
    }

    /// <summary>
    /// The "at least one row" rule must use the stored LayoutId. A caller-supplied row with
    /// a null LayoutId previously skipped the check and allowed the last row to be deleted.
    /// </summary>
    [Fact]
    public void RemoveLayoutRow_WhenCallerOmitsLayoutId_StillKeepsLastRow()
    {
        using var seedCtx = CreateContext();
        var (_, _, rowIds) = SeedLayout(seedCtx, 1);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.RemoveLayoutRow(new LayoutRow { LayoutRowId = rowIds[0], LayoutId = null });

        Assert.False(result);
        using var verifyCtx = CreateContext();
        Assert.NotNull(verifyCtx.LayoutRows.Find(rowIds[0]));
    }

    [Fact]
    public async Task RemoveLayoutRowAsync_WhenRowNotFound_ReturnsFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RemoveLayoutRowAsync(new LayoutRow { LayoutRowId = Guid.NewGuid() }, TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveLayoutRowAsync_WhenLayoutHasMultipleRows_RemovesRow()
    {
        await using var seedCtx = CreateContext();
        var (_, layoutId, rowIds) = SeedLayout(seedCtx, 2);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RemoveLayoutRowAsync(
            new LayoutRow { LayoutRowId = rowIds[0], LayoutId = layoutId }, TestContext.Current.CancellationToken);

        Assert.True(result);
        await using var verifyCtx = CreateContext();
        Assert.Null(await verifyCtx.LayoutRows.FindAsync(new object[] { rowIds[0] }, TestContext.Current.CancellationToken));
        Assert.NotNull(await verifyCtx.LayoutRows.FindAsync(new object[] { rowIds[1] }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RemoveLayoutRowAsync_WhenCallerOmitsLayoutId_StillKeepsLastRow()
    {
        await using var seedCtx = CreateContext();
        var (_, _, rowIds) = SeedLayout(seedCtx, 1);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RemoveLayoutRowAsync(
            new LayoutRow { LayoutRowId = rowIds[0], LayoutId = null }, TestContext.Current.CancellationToken);

        Assert.False(result);
        await using var verifyCtx = CreateContext();
        Assert.NotNull(await verifyCtx.LayoutRows.FindAsync(new object[] { rowIds[0] }, TestContext.Current.CancellationToken));
    }

    // -------------------------------------------------------------------------
    // SaveLayoutAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// Bug fix: SaveLayoutAsync skipped rows whose RowIndex was unchanged, so a
    /// LayoutType change on such a row was never persisted.
    /// </summary>
    [Fact]
    public async Task SaveLayoutAsync_WhenOnlyLayoutTypeChanges_PersistsNewType()
    {
        await using var seedCtx = CreateContext();
        var (tabId, _, rowIds) = SeedLayout(seedCtx, 1);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.SaveLayoutAsync(tabId, new List<LayoutOrder>
        {
            new() { LayoutRowId = rowIds[0], Index = 1, TypeId = TypeB }
        }, TestContext.Current.CancellationToken);

        Assert.True(result);
        await using var verifyCtx = CreateContext();
        var row = await verifyCtx.LayoutRows.FindAsync(new object[] { rowIds[0] }, TestContext.Current.CancellationToken);
        Assert.Equal(TypeB, row!.LayoutTypeId);
    }

    [Fact]
    public async Task SaveLayoutAsync_WhenIndexChanges_PersistsNewIndex()
    {
        await using var seedCtx = CreateContext();
        var (tabId, _, rowIds) = SeedLayout(seedCtx, 2);

        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.SaveLayoutAsync(tabId, new List<LayoutOrder>
        {
            new() { LayoutRowId = rowIds[0], Index = 2, TypeId = TypeA },
            new() { LayoutRowId = rowIds[1], Index = 1, TypeId = TypeA }
        }, TestContext.Current.CancellationToken);

        Assert.True(result);
        await using var verifyCtx = CreateContext();
        Assert.Equal(2, (await verifyCtx.LayoutRows.FindAsync(new object[] { rowIds[0] }, TestContext.Current.CancellationToken))!.RowIndex);
        Assert.Equal(1, (await verifyCtx.LayoutRows.FindAsync(new object[] { rowIds[1] }, TestContext.Current.CancellationToken))!.RowIndex);
    }

    [Fact]
    public void SaveLayout_WhenOnlyLayoutTypeChanges_PersistsNewType()
    {
        using var seedCtx = CreateContext();
        var (tabId, _, rowIds) = SeedLayout(seedCtx, 1);

        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.SaveLayout(tabId, new List<LayoutOrder>
        {
            new() { LayoutRowId = rowIds[0], Index = 1, TypeId = TypeB }
        });

        Assert.True(result);
        using var verifyCtx = CreateContext();
        Assert.Equal(TypeB, verifyCtx.LayoutRows.Find(rowIds[0])!.LayoutTypeId);
    }

    // -------------------------------------------------------------------------
    // SaveWidgetPlacement
    // -------------------------------------------------------------------------

    /// <summary>
    /// Bug fix: the sync UpdateNewLayoutOrder used a null-forgiving operator, so a missing
    /// placement surfaced as a NullReferenceException in the caller instead of null.
    /// </summary>
    [Fact]
    public void SaveWidgetPlacement_WhenPlacementNotFound_ReturnsNull()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var result = service.SaveWidgetPlacement(new PlacementParameter
        {
            PlacementId         = Guid.NewGuid(),
            PreviousLayoutRowId = Guid.NewGuid(),
            CurrentLayoutRowId  = Guid.NewGuid()
        });

        Assert.Null(result);
    }
}
