using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.ToTable("waitlist_entries");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(w => w.RequestDate).IsRequired();

        builder.HasOne(w => w.Patient)
            .WithMany()
            .HasForeignKey(w => w.PatientId);

        builder.HasOne(w => w.PreferredTherapist)
            .WithMany()
            .HasForeignKey(w => w.PreferredTherapistId)
            .IsRequired(false);

        builder.Property(w => w.PreferredDays)
            .HasColumnType("jsonb");

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.PatientId);
        builder.HasIndex(w => w.PreferredTherapistId);
    }
}
