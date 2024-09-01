using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

/// <summary>
/// Entity Framework Configuration for <see cref="Plan"/> for entity properties and relationships
/// </summary>
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Plan>> _seedAction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="seedAction"></param>
    public PlanConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Plan>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plan", _config.Schema);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        if (_seedAction != null) _seedAction(builder);
    }
}