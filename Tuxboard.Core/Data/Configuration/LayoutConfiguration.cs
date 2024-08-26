using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class LayoutConfiguration : IEntityTypeConfiguration<Layout>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Layout>> _seedAction;

    public LayoutConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Layout>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<Layout> builder)
    {
        builder.ToTable("Layout", _config.Schema);

        builder.HasIndex(e => e.TabId, "IX_Layout_TabId");

        builder.Property(e => e.LayoutId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

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