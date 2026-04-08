using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class TherapySessionConfiguration : IEntityTypeConfiguration<TherapySession>
{
    public void Configure(EntityTypeBuilder<TherapySession> builder)
    {
        builder.ToTable("therapy_sessions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.SessionDate).IsRequired();

        builder.HasOne(e => e.TherapyCase)
            .WithMany(tc => tc.Sessions)
            .HasForeignKey(e => e.TherapyCaseId);

        builder.HasOne(e => e.Encounter)
            .WithMany()
            .HasForeignKey(e => e.EncounterId)
            .IsRequired(false);

        builder.HasOne(e => e.Appointment)
            .WithMany()
            .HasForeignKey(e => e.AppointmentId)
            .IsRequired(false);

        builder.HasIndex(e => new { e.TherapyCaseId, e.SessionNumber }).IsUnique();
        builder.HasIndex(e => e.SessionDate);
    }
}
