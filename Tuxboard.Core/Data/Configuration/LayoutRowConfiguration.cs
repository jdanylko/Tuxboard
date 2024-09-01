using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="LayoutRow"/> for entity properties and relationships
/// </summary>
public class LayoutRowConfiguration : IEntityTypeConfiguration<LayoutRow>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<LayoutRow>> _seedAction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="seedAction"></param>
    public LayoutRowConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<LayoutRow>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LayoutRow> builder)
    {
        builder.ToTable("LayoutRow", _config.Schema);

        builder.HasIndex(e => e.LayoutId, "IX_LayoutRow_LayoutId");

        builder.HasIndex(e => e.LayoutTypeId, "IX_LayoutRow_LayoutTypeId");

        builder.Property(e => e.LayoutRowId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.LayoutId)
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.LayoutType)
            .WithMany(p => p.LayoutRows)
            .HasForeignKey(d => d.LayoutTypeId)
            .HasConstraintName("FK_LayoutRow_LayoutType");

        if (_seedAction != null) _seedAction(builder);

        if (!_config.CreateSeedData) return;

        builder.HasData(new List<LayoutRow>
            {
                new()
                {
                    LayoutRowId = new Guid("D58AFCD2-2007-4FD0-87A9-93C85C667F3F"),
                    LayoutId = new Guid("5267DA05-AFE4-4753-9CEE-D5D32C2B068E"),
                    LayoutTypeId = 4,
                    RowIndex = 0
                }
            }
        );
    }
}