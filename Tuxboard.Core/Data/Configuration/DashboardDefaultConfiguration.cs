using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class DashboardDefaultConfiguration : IEntityTypeConfiguration<DashboardDefault>
{
    public void Configure(EntityTypeBuilder<DashboardDefault> builder)
    {
        builder.HasKey(e => e.DefaultId);

        builder.ToTable("DashboardDefault");

        builder.HasIndex(e => e.LayoutId, "IX_DashboardDefault_LayoutId");

        builder.HasIndex(e => e.PlanId, "IX_DashboardDefault_PlanId");

        builder.Property(e => e.DefaultId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.LayoutId)
            .IsRequired()
            .HasMaxLength(36)
            .IsUnicode(false);

        builder.HasOne(d => d.Layout)
            .WithMany(p => p.DashboardDefaults)
            .HasForeignKey(d => d.LayoutId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DashboardDefault_Layout");

        builder.HasOne(d => d.Plan)
            .WithMany(p => p.DashboardDefaults)
            .HasForeignKey(d => d.PlanId)
            .HasConstraintName("FK_DashboardDefault_Plan");

        builder.HasData(new List<DashboardDefault>
            {
                new()
                {
                    DefaultId = new Guid("0D96A18E-90B8-4A9F-9DF1-126653D68FE6"),
                    LayoutId = new Guid("5267DA05-AFE4-4753-9CEE-D5D32C2B068E"),
                    PlanId = null
                }
            }
        );
    }
}