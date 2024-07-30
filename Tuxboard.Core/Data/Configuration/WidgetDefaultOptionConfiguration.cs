﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class WidgetDefaultOptionConfiguration : IEntityTypeConfiguration<WidgetDefaultOption>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<WidgetDefaultOption>> _seedAction;

    public WidgetDefaultOptionConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<WidgetDefaultOption>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<WidgetDefaultOption> builder)
    {
        builder.HasKey(e => e.WidgetOptionId)
            .HasName("PK_WidgetSettingOption");

        builder.ToTable("WidgetDefaultOption");

        builder.HasIndex(e => e.WidgetDefaultId, "IX_WidgetDefaultOption_WidgetDefaultId");

        builder.Property(e => e.WidgetOptionId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.SettingLabel)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.SettingValue)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.WidgetDefaultId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.WidgetDefault)
            .WithMany(p => p.WidgetDefaultOptions)
            .HasForeignKey(d => d.WidgetDefaultId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WidgetDefaultOption_WidgetDefault");

        if (_seedAction != null) _seedAction(builder);
    }
}