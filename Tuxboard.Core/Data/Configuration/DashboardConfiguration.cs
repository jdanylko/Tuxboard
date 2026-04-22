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
    private readonly Action<EntityTypeBuilder<Dashboard<T>>>? _seedAction;

    /// <summary>
    /// Initializes the EF Core configuration for <see cref="Dashboard{T}"/> with the provided options and optional seed action.
    /// </summary>
    /// <param name="config">Tuxboard configuration options, including schema name and seed data settings.</param>
    /// <param name="seedAction">Optional action to apply additional seed data for <see cref="Dashboard{T}"/>.</param>
    public DashboardConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Dashboard<T>>>? seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dashboard<T>> builder)
    {
        builder.ToTable("Dashboard", _config.Schema);

        builder.Property(e => e.DashboardId)
            .IsUnicode(false);

        if (_seedAction != null) _seedAction(builder);
    }
}