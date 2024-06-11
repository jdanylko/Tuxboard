using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
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
    }
}