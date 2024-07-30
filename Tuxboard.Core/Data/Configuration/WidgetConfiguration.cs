using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Widget>> _seedAction;

    public WidgetConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Widget>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.ToTable("Widget");

        builder.Property(e => e.WidgetId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.GroupName)
            .IsRequired()
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.ImageUrl)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.HasMany(d => d.Plans)
            .WithMany(p => p.Widgets)
            .UsingEntity<Dictionary<string, object>>(
                "WidgetPlan",
                l => l.HasOne<Plan>().WithMany().HasForeignKey("PlanId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WidgetPlan_Plan"),
                r => r.HasOne<Widget>().WithMany().HasForeignKey("WidgetId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WidgetPlan_Widget"),
                j =>
                {
                    j.HasKey("WidgetId", "PlanId");

                    j.ToTable("WidgetPlan");

                    j.HasIndex(new[] { "PlanId" }, "IX_WidgetPlan_PlanId");
                });

        if (_seedAction != null) _seedAction(builder);

        if (!_config.CreateSeedData) return;

        builder.HasData(new List<Widget>
            {
                new()
                {
                    WidgetId = new Guid("1885170C-7C48-4557-ABC7-BC06D3FC51EE"),
                    Name = "generalinfo",
                    Title = "General Info",
                    Description = "Display General Information",
                    ImageUrl = "", GroupName = "", Permission = 0, Moveable = false, CanDelete = false,
                    UseSettings = false, UseTemplate = false
                },
                new()
                {
                    WidgetId = new Guid("C9A9DB53-14CA-4551-87E7-F9656F39A396"),
                    Name = "helloworld",
                    Title = "Hello World",
                    Description = "A Simple Hello World Widget",
                    ImageUrl = "", GroupName = "", Permission = 0, Moveable = true, CanDelete = true,
                    UseSettings = true, UseTemplate = true
                },
                new()
                {
                    WidgetId = new Guid("EE84443B-7EE7-4754-BB3C-313CC0DA6039"),
                    Name = "table",
                    Title = "Sample Table",
                    Description = "Demonstration of data table",
                    ImageUrl = "", GroupName = "", Permission = 0, Moveable = true, CanDelete = true,
                    UseSettings = true, UseTemplate = true
                }
            }
        );
    }
}