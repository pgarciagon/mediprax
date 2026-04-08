using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PsychopathologicalFindingConfiguration : IEntityTypeConfiguration<PsychopathologicalFinding>
{
    public void Configure(EntityTypeBuilder<PsychopathologicalFinding> builder)
    {
        builder.ToTable("psychopathological_findings");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(f => f.AssessmentDate).IsRequired();
        builder.Property(f => f.Findings).HasColumnType("jsonb");

        builder.HasOne(f => f.Patient)
            .WithMany()
            .HasForeignKey(f => f.PatientId);

        builder.HasOne(f => f.Encounter)
            .WithMany()
            .HasForeignKey(f => f.EncounterId);

        builder.HasOne(f => f.AssessedBy)
            .WithMany()
            .HasForeignKey(f => f.AssessedById);

        builder.HasIndex(f => f.EncounterId).IsUnique();
        builder.HasIndex(f => new { f.PatientId, f.AssessmentDate });

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}
