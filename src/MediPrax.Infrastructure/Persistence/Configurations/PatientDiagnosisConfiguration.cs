using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PatientDiagnosisConfiguration : IEntityTypeConfiguration<PatientDiagnosis>
{
    public void Configure(EntityTypeBuilder<PatientDiagnosis> builder)
    {
        builder.ToTable("patient_diagnoses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.Icd10Code).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Icd10Description).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(2000);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Diagnoses)
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.CreatedByDoctor)
            .WithMany()
            .HasForeignKey(e => e.CreatedByDoctorId);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => new { e.PatientId, e.Icd10Code });
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.DiagnosisType);
    }
}
