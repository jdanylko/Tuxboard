using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="Dashboard{T}"/> for entity properties and relationships
/// </summary>
public class DashboardConfiguration<T> : IEntityTypeConfiguration<Dashboard<T>> where T : struct
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Dashboard<T>>> _seedAction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="seedAction"></param>
    public DashboardConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Dashboard<T>>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dashboard<T>> builder)
    {
        builder.ToTable("Dashboard", _config.Schema);

        builder.Property(e => e.DashboardId)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        if (_seedAction != null) _seedAction(builder);
    }
}