using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("prescriptions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.MedicationName).HasMaxLength(300).IsRequired();
        builder.Property(p => p.MedicationPzn).HasMaxLength(20);
        builder.Property(p => p.ERezeptId).HasMaxLength(100);

        builder.HasOne(p => p.Patient)
            .WithMany(pat => pat.Prescriptions)
            .HasForeignKey(p => p.PatientId);

        builder.HasOne(p => p.Encounter)
            .WithMany(e => e.Prescriptions)
            .HasForeignKey(p => p.EncounterId);

        builder.HasIndex(p => p.PatientId);
        builder.HasIndex(p => p.IsBtm).HasFilter("\"IsBtm\" = true");
    }
}
