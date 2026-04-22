using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="WidgetSetting"/> for entity properties and relationships
/// </summary>
public class WidgetSettingConfiguration : IEntityTypeConfiguration<WidgetSetting>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<WidgetSetting>>? _seedAction;

    /// <summary>
    /// Initializes the EF Core configuration for <see cref="WidgetSetting"/> with the provided options and optional seed action.
    /// </summary>
    /// <param name="config">Tuxboard configuration options, including schema name and seed data settings.</param>
    /// <param name="seedAction">Optional action to apply additional seed data for <see cref="WidgetSetting"/>.</param>
    public WidgetSettingConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<WidgetSetting>>? seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WidgetSetting> builder)
    {
        builder.ToTable("WidgetSetting", _config.Schema);

        builder.HasIndex(e => e.WidgetDefaultId, "IX_WidgetSetting_WidgetDefaultId");

        builder.HasIndex(e => e.WidgetPlacementId, "IX_WidgetSetting_WidgetPlacementId");

        builder.Property(e => e.WidgetSettingId)
            .HasMaxLength(36)
            .IsUnicode(false);

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