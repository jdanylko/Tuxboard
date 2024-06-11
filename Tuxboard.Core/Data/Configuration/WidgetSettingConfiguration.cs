using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class WidgetSettingConfiguration : IEntityTypeConfiguration<WidgetSetting>
{
    public void Configure(EntityTypeBuilder<WidgetSetting> builder)
    {
        builder.ToTable("WidgetSetting");

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

    }
}