using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class Icd10CodeConfiguration : IEntityTypeConfiguration<Icd10Code>
{
    public void Configure(EntityTypeBuilder<Icd10Code> builder)
    {
        builder.ToTable("icd10_codes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityAlwaysColumn();
        builder.Property(e => e.Code).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(100);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsActive);
    }
}
