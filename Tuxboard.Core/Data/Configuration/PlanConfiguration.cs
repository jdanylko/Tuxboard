using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Configuration;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plan");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

    }
}