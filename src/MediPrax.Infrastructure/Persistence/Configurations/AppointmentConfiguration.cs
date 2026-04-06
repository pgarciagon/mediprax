using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(a => a.StartTime).IsRequired();
        builder.Property(a => a.DurationMinutes).HasDefaultValue(10);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId);

        builder.HasOne(a => a.Doctor)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.DoctorId);

        builder.HasIndex(a => new { a.DoctorId, a.StartTime });
        builder.HasIndex(a => a.PatientId);
    }
}
