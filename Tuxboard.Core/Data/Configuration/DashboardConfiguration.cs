using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="Dashboard"/> for entity properties and relationships
/// </summary>
public class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Dashboard>> _seedAction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="seedAction"></param>
    public DashboardConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Dashboard>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.ToTable("Dashboard", _config.Schema);

        builder.Property(e => e.DashboardId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.UserId)
            .HasMaxLength(36)
            .IsUnicode(false);

        if (_seedAction != null) _seedAction(builder);

    }
}