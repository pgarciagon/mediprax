using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class DoctorScheduleTemplateConfiguration : IEntityTypeConfiguration<DoctorScheduleTemplate>
{
    public void Configure(EntityTypeBuilder<DoctorScheduleTemplate> builder)
    {
        builder.ToTable("doctor_schedule_templates");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasOne(s => s.Doctor).WithMany().HasForeignKey(s => s.DoctorId);
        builder.HasIndex(s => s.DoctorId);
    }
}

public class DoctorAbsenceConfiguration : IEntityTypeConfiguration<DoctorAbsence>
{
    public void Configure(EntityTypeBuilder<DoctorAbsence> builder)
    {
        builder.ToTable("doctor_absences");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasOne(a => a.Doctor).WithMany().HasForeignKey(a => a.DoctorId);
        builder.HasOne(a => a.Substitute).WithMany().HasForeignKey(a => a.SubstituteId).IsRequired(false);
        builder.HasIndex(a => new { a.DoctorId, a.StartDate, a.EndDate });
    }
}
