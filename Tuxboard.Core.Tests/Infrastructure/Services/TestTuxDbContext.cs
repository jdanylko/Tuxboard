using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;

namespace Tuxboard.Core.Tests.Infrastructure.Services;

/// <summary>
/// A test double that wraps TuxDbContext to record SaveChanges call counts and
/// tokens so tests can assert against single-save semantics.
/// </summary>
internal sealed class TestTuxDbContext<T>(DbContextOptions<TuxDbContext<T>> options, IOptions<TuxboardConfig> config)
    : TuxDbContext<T>(options, config)
    where T : struct
{
    public int SaveChangesAsyncCallCount { get; private set; }
    public List<CancellationToken> CapturedAsyncTokens { get; } = new();
    public int SaveChangesCallCount { get; private set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesAsyncCallCount++;
        CapturedAsyncTokens.Add(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        SaveChangesCallCount++;
        return base.SaveChanges();
    }
}
