using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class TherapyCaseConfiguration : IEntityTypeConfiguration<TherapyCase>
{
    public void Configure(EntityTypeBuilder<TherapyCase> builder)
    {
        builder.ToTable("therapy_cases");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.Diagnoses).HasColumnType("jsonb");

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.Therapist)
            .WithMany()
            .HasForeignKey(e => e.TherapistId);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.TherapistId);
        builder.HasIndex(e => e.Status);
    }
}
