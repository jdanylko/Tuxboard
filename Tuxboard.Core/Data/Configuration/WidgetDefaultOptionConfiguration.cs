using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="WidgetDefaultOption"/> for entity properties and relationships
/// </summary>
public class WidgetDefaultOptionConfiguration : IEntityTypeConfiguration<WidgetDefaultOption>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<WidgetDefaultOption>>? _seedAction;

    /// <summary>
    /// Initializes the EF Core configuration for <see cref="WidgetDefaultOption"/> with the provided options and optional seed action.
    /// </summary>
    /// <param name="config">Tuxboard configuration options, including schema name and seed data settings.</param>
    /// <param name="seedAction">Optional action to apply additional seed data for <see cref="WidgetDefaultOption"/>.</param>
    public WidgetDefaultOptionConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<WidgetDefaultOption>>? seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WidgetDefaultOption> builder)
    {
        builder.HasKey(e => e.WidgetOptionId)
            .HasName("PK_WidgetSettingOption");

        builder.ToTable("WidgetDefaultOption", _config.Schema);

        builder.HasIndex(e => e.WidgetDefaultId, "IX_WidgetDefaultOption_WidgetDefaultId");

        builder.Property(e => e.WidgetOptionId)
            .HasMaxLength(36)
            .IsUnicode(false);

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