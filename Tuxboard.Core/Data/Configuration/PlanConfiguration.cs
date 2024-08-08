using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Plan>> _seedAction;

    public PlanConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Plan>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plan");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        if (_seedAction != null) _seedAction(builder);
    }
}