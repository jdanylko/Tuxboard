using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class WidgetSettingConfiguration : IEntityTypeConfiguration<WidgetSetting>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<WidgetSetting>> _seedAction;

    public WidgetSettingConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<WidgetSetting>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<WidgetSetting> builder)
    {
        builder.ToTable("WidgetSetting", _config.Schema);

        builder.HasIndex(e => e.WidgetDefaultId, "IX_WidgetSetting_WidgetDefaultId");

        builder.HasIndex(e => e.WidgetPlacementId, "IX_WidgetSetting_WidgetPlacementId");

        builder.Property(e => e.WidgetSettingId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.Value)
            .IsRequired()
            .IsUnicode(false);

        builder.Property(e => e.WidgetDefaultId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.WidgetPlacementId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.WidgetDefault)
            .WithMany(p => p.WidgetSettings)
            .HasForeignKey(d => d.WidgetDefaultId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WidgetSetting_WidgetDefault");

        builder.HasOne(d => d.WidgetPlacement)
            .WithMany(p => p.WidgetSettings)
            .HasForeignKey(d => d.WidgetPlacementId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WidgetSetting_WidgetPlacement");

        if (_seedAction != null) _seedAction(builder);
    }
}