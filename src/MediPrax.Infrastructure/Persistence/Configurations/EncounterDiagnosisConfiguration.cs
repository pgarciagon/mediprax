using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class EncounterDiagnosisConfiguration : IEntityTypeConfiguration<EncounterDiagnosis>
{
    public void Configure(EntityTypeBuilder<EncounterDiagnosis> builder)
    {
        builder.ToTable("encounter_diagnoses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.HasOne(e => e.Encounter)
            .WithMany(enc => enc.EncounterDiagnoses)
            .HasForeignKey(e => e.EncounterId);

        builder.HasOne(e => e.PatientDiagnosis)
            .WithMany(pd => pd.EncounterDiagnoses)
            .HasForeignKey(e => e.PatientDiagnosisId);

        builder.HasIndex(e => new { e.EncounterId, e.PatientDiagnosisId }).IsUnique();
        builder.HasIndex(e => e.EncounterId);
    }
}
