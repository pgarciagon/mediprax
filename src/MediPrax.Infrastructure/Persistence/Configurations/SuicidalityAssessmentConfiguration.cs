using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class SuicidalityAssessmentConfiguration : IEntityTypeConfiguration<SuicidalityAssessment>
{
    public void Configure(EntityTypeBuilder<SuicidalityAssessment> builder)
    {
        builder.ToTable("suicidality_assessments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(a => a.RiskFactors).HasColumnType("jsonb");
        builder.Property(a => a.ProtectiveFactors).HasColumnType("jsonb");
        builder.Property(a => a.ActionsTaken).HasColumnType("jsonb");

        builder.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId);
        builder.HasOne(a => a.Encounter).WithMany().HasForeignKey(a => a.EncounterId).IsRequired(false);
        builder.HasOne(a => a.AssessedBy).WithMany().HasForeignKey(a => a.AssessedById).IsRequired(false);

        builder.HasIndex(a => a.PatientId);
        builder.HasIndex(a => a.EncounterId);
    }
}
