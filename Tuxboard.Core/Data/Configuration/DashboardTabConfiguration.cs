using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="DashboardTab"/> for entity properties and relationships
/// </summary>
public class DashboardTabConfiguration<T> : IEntityTypeConfiguration<DashboardTab> where T: struct
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<DashboardTab>>? _seedAction;

    /// <summary>
    /// Initializes the EF Core configuration for <see cref="DashboardTab"/> with the provided options and optional seed action.
    /// </summary>
    /// <param name="config">Tuxboard configuration options, including schema name and seed data settings.</param>
    /// <param name="seedAction">Optional action to apply additional seed data for <see cref="DashboardTab"/>.</param>
    public DashboardTabConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<DashboardTab>>? seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DashboardTab> builder)
    {
        builder.HasKey(e => e.TabId);

        builder.ToTable("DashboardTab", _config.Schema);

        builder.HasIndex(e => e.DashboardId, "IX_DashboardTab_DashboardId");

        builder.Property(e => e.TabId)
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.DashboardId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.TabTitle)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        //builder.HasOne(d => d.Dashboard)
        //    .WithMany(p => p.Tabs)
        //    .HasForeignKey(d => d.DashboardId)
        //    .OnDelete(DeleteBehavior.ClientSetNull)
        //    .HasConstraintName("FK_DashboardTab_Dashboard");

        if (_seedAction != null) _seedAction(builder);
    }
}