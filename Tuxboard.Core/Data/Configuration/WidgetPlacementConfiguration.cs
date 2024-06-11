using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class WidgetPlacementConfiguration : IEntityTypeConfiguration<WidgetPlacement>
{
    public void Configure(EntityTypeBuilder<WidgetPlacement> builder)
    {
        builder.ToTable("WidgetPlacement");

        builder.HasIndex(e => e.LayoutRowId, "IX_WidgetPlacement_LayoutRowId");

        builder.HasIndex(e => e.WidgetId, "IX_WidgetPlacement_WidgetId");

        builder.Property(e => e.WidgetPlacementId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.LayoutRowId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.WidgetId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.LayoutRow)
            .WithMany(p => p.WidgetPlacements)
            .HasForeignKey(d => d.LayoutRowId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WidgetPlacement_LayoutRow1");

        builder.HasOne(d => d.Widget)
            .WithMany(p => p.WidgetPlacements)
            .HasForeignKey(d => d.WidgetId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WidgetPlacement_Widget1");

    }
}