using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class AppointmentSeriesConfiguration : IEntityTypeConfiguration<AppointmentSeries>
{
    public void Configure(EntityTypeBuilder<AppointmentSeries> builder)
    {
        builder.ToTable("appointment_series");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.DurationMinutes).HasDefaultValue(50);

        builder.HasOne(s => s.Patient)
            .WithMany()
            .HasForeignKey(s => s.PatientId);

        builder.HasOne(s => s.Doctor)
            .WithMany()
            .HasForeignKey(s => s.DoctorId);

        builder.HasOne(s => s.TherapyCase)
            .WithMany()
            .HasForeignKey(s => s.TherapyCaseId)
            .IsRequired(false);

        builder.HasMany(s => s.Appointments)
            .WithOne(a => a.AppointmentSeries)
            .HasForeignKey(a => a.AppointmentSeriesId)
            .IsRequired(false);

        builder.HasIndex(s => s.PatientId);
        builder.HasIndex(s => s.DoctorId);
    }
}
