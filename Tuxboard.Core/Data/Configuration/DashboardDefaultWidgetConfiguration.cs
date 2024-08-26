using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class DashboardDefaultWidgetConfiguration : IEntityTypeConfiguration<DashboardDefaultWidget>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<DashboardDefaultWidget>> _seedAction;

    public DashboardDefaultWidgetConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<DashboardDefaultWidget>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<DashboardDefaultWidget> builder)
    {
        builder.HasKey(e => e.DefaultWidgetId);

        builder.ToTable("DashboardDefaultWidget", _config.Schema);

        builder.HasIndex(e => e.DashboardDefaultId, "IX_DashboardDefaultWidget_DashboardDefaultId");

        builder.HasIndex(e => e.LayoutRowId, "IX_DashboardDefaultWidget_LayoutRowId");

        builder.HasIndex(e => e.WidgetId, "IX_DashboardDefaultWidget_WidgetId");

        builder.Property(e => e.DefaultWidgetId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.DashboardDefaultId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.LayoutRowId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.WidgetId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.DashboardDefault)
            .WithMany(p => p.DashboardDefaultWidgets)
            .HasForeignKey(d => d.DashboardDefaultId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DashboardDefaultWidget_DashboardDefault");

        builder.HasOne(d => d.LayoutRow)
            .WithMany(p => p.DashboardDefaultWidgets)
            .HasForeignKey(d => d.LayoutRowId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DashboardDefaultWidget_LayoutRow");

        builder.HasOne(d => d.Widget)
            .WithMany(p => p.DashboardDefaultWidgets)
            .HasForeignKey(d => d.WidgetId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DashboardDefaultWidget_Widget");

        if (_seedAction != null) _seedAction(builder);

        if (!_config.CreateSeedData) return;

        builder.HasData(new List<DashboardDefaultWidget>
            {
                new()
                {
                    DefaultWidgetId = new Guid("D21E94CF-86A9-4058-BB72-F269728AC8AD"),
                    DashboardDefaultId = new Guid("0D96A18E-90B8-4A9F-9DF1-126653D68FE6"),
                    LayoutRowId = new Guid("D58AFCD2-2007-4FD0-87A9-93C85C667F3F"),
                    WidgetId = new Guid("C9A9DB53-14CA-4551-87E7-F9656F39A396"),
                    ColumnIndex = 0,
                    WidgetIndex = 0
                }
            }
        );
    }
}