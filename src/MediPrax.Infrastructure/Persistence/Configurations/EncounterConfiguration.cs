using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class EncounterConfiguration : IEntityTypeConfiguration<Encounter>
{
    public void Configure(EntityTypeBuilder<Encounter> builder)
    {
        builder.ToTable("encounters");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.EncounterDate).IsRequired();
        builder.Property(e => e.Icd10Codes).HasColumnType("jsonb");

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Encounters)
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.Doctor)
            .WithMany(u => u.Encounters)
            .HasForeignKey(e => e.DoctorId);

        builder.HasOne(e => e.Appointment)
            .WithOne(a => a.Encounter)
            .HasForeignKey<Encounter>(e => e.AppointmentId);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.EncounterDate);
    }
}
