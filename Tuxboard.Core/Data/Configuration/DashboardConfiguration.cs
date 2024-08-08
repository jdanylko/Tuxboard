using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    private readonly TuxboardConfig _config;
    private readonly Action<EntityTypeBuilder<Dashboard>> _seedAction;

    public DashboardConfiguration(TuxboardConfig config,
        Action<EntityTypeBuilder<Dashboard>> seedAction = null)
    {
        _config = config;
        _seedAction = seedAction;
    }

    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.ToTable("Dashboard");

        builder.Property(e => e.DashboardId)
            .HasMaxLength(36)
            .IsUnicode(false)
            .HasDefaultValueSql("(newid())");

        builder.Property(e => e.UserId)
            .HasMaxLength(36)
            .IsUnicode(false);

        if (_seedAction != null) _seedAction(builder);

    }
}