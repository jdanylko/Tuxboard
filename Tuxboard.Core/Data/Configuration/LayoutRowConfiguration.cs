using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class LayoutRowConfiguration : IEntityTypeConfiguration<LayoutRow>
{
    public void Configure(EntityTypeBuilder<LayoutRow> builder)
    {
        builder.ToTable("LayoutRow");

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