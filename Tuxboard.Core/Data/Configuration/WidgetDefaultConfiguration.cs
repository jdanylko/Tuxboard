using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class WidgetDefaultConfiguration : IEntityTypeConfiguration<WidgetDefault>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<WidgetDefault>> _seedAction;

    public WidgetDefaultConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<WidgetDefault>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<WidgetDefault> builder)
    {
        builder.ToTable("WidgetDefault");

        builder.HasIndex(e => e.WidgetId, "IX_WidgetDefault_WidgetId");

        builder.Property(e => e.WidgetDefaultId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.DefaultValue)
            .IsRequired()
            .IsUnicode(false);

        builder.Property(e => e.SettingName)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.SettingTitle)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.WidgetId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.Widget)
            .WithMany(p => p.WidgetDefaults)
            .HasForeignKey(d => d.WidgetId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WidgetDefault_Widget");

        if (_seedAction != null) _seedAction(builder);

        if (!_config.CreateSeedData) return;

        builder.HasData(new List<WidgetDefault>
            {
                new()
                {
                    WidgetDefaultId = new Guid("046F4AA8-5E45-4C86-B2F8-CBF3E42647E7"),
                    WidgetId = new Guid("EE84443B-7EE7-4754-BB3C-313CC0DA6039"),
                    SettingName = "widgettitle",
                    SettingTitle = "Title",
                    DefaultValue = "Sample Table",
                    SettingIndex = 1
                },
                new()
                {
                    WidgetDefaultId = new Guid("5C85537A-1319-48ED-A475-83D3DC3E7A8D"),
                    WidgetId = new Guid("C9A9DB53-14CA-4551-87E7-F9656F39A396"),
                    SettingName = "widgettitle",
                    SettingTitle = "Title",
                    DefaultValue = "Projects",
                    SettingIndex = 1
                }
            }
        );
    }
}