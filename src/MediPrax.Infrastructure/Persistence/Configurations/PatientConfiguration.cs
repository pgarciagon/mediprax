using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.DateOfBirth).IsRequired();
        builder.Property(p => p.Gender).HasMaxLength(10);
        builder.Property(p => p.InsuranceNumber).HasMaxLength(30);
        builder.Property(p => p.InsuranceProvider).HasMaxLength(200);
        builder.Property(p => p.Kvnr).HasMaxLength(10);
        builder.Property(p => p.Phone).HasMaxLength(50);
        builder.Property(p => p.Email).HasMaxLength(200);

        builder.HasIndex(p => new { p.LastName, p.FirstName });
        builder.HasIndex(p => p.Kvnr);
        builder.HasIndex(p => p.DateOfBirth);
    }
}
