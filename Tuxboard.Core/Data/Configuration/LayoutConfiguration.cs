using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="Layout"/> for entity properties and relationships
/// </summary>
public class LayoutConfiguration : IEntityTypeConfiguration<Layout>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Layout>>? _seedAction;

    /// <summary>
    /// Initializes the EF Core configuration for <see cref="Layout"/> with the provided options and optional seed action.
    /// </summary>
    /// <param name="config">Tuxboard configuration options, including schema name and seed data settings.</param>
    /// <param name="seedAction">Optional action to apply additional seed data for <see cref="Layout"/>.</param>
    public LayoutConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Layout>>? seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Layout> builder)
    {
        builder.ToTable("Layout", _config.Schema);

        builder.HasIndex(e => e.TabId, "IX_Layout_TabId");

        builder.Property(e => e.LayoutId)
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.TabId)
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.Tab)
            .WithMany(p => p.Layouts)
            .HasForeignKey(d => d.TabId)
            .HasConstraintName("FK_DashboardLayout_DashboardTab");

        if (_seedAction != null) _seedAction(builder);

        if (!_config.CreateSeedData) return;

        builder.HasData(
            new List<Layout>
            {
                new() { LayoutId = new Guid("5267DA05-AFE4-4753-9CEE-D5D32C2B068E"), TabId = null, LayoutIndex = 1 }
            }
        );

    }
}