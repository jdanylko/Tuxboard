using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class LayoutTypeConfiguration : IEntityTypeConfiguration<LayoutType>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<LayoutType>> _seedAction;

    public LayoutTypeConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<LayoutType>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<LayoutType> builder)
    {
        builder.ToTable("LayoutType");

        builder.Property(e => e.Layout)
            .IsRequired()
            .IsUnicode(false);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        if (_seedAction != null) _seedAction(builder);

        if (!_config.CreateSeedData) return;

        builder.HasData(
            new List<LayoutType>
            {
                new() { LayoutTypeId = 1, Title = "Three Columns, Equal", Layout = "col-4/col-4/col-4" },
                new() { LayoutTypeId = 2, Title = "Three Columns, 50% Middle", Layout = "col-3/col-6/col-3" },
                new() { LayoutTypeId = 3, Title = "Four Columns, 25%", Layout = "col-3/col-3/col-3/col-3" },
                new() { LayoutTypeId = 4, Title = "Two Columns, 50%", Layout = "col-6/col-6" }
            }
        );
    }
}