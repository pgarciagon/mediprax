using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PsychometricTestConfiguration : IEntityTypeConfiguration<PsychometricTest>
{
    public void Configure(EntityTypeBuilder<PsychometricTest> builder)
    {
        builder.ToTable("psychometric_tests");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.TestDate).IsRequired();
        builder.Property(e => e.Responses).HasColumnType("jsonb");
        builder.Property(e => e.Interpretation).HasMaxLength(500);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.PsychometricTests)
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.Encounter)
            .WithMany()
            .HasForeignKey(e => e.EncounterId)
            .IsRequired(false);

        builder.HasOne(e => e.AdministeredBy)
            .WithMany(u => u.AdministeredTests)
            .HasForeignKey(e => e.AdministeredById);

        builder.HasIndex(e => new { e.PatientId, e.TestType, e.TestDate });
        builder.HasIndex(e => e.EncounterId);
    }
}
