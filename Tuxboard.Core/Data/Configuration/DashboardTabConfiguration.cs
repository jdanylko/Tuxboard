using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class DashboardTabConfiguration : IEntityTypeConfiguration<DashboardTab>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<DashboardTab>> _seedAction;

    public DashboardTabConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<DashboardTab>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<DashboardTab> builder)
    {
        builder.HasKey(e => e.TabId);

        builder.ToTable("DashboardTab");

        builder.HasIndex(e => e.DashboardId, "IX_DashboardTab_DashboardId");

        builder.Property(e => e.TabId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.DashboardId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.Property(e => e.TabTitle)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.HasOne(d => d.Dashboard)
            .WithMany(p => p.Tabs)
            .HasForeignKey(d => d.DashboardId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DashboardTab_Dashboard");

        if (_seedAction != null) _seedAction(builder);
    }
}