using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class NeurologicalExaminationConfiguration : IEntityTypeConfiguration<NeurologicalExamination>
{
    public void Configure(EntityTypeBuilder<NeurologicalExamination> builder)
    {
        builder.ToTable("neurological_examinations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.ExamDate).IsRequired();

        // JSONB columns
        builder.OwnsOne(e => e.CranialNerves, b => b.ToJson());
        builder.OwnsOne(e => e.MotorSystem, b => b.ToJson());
        builder.OwnsOne(e => e.Reflexes, b => b.ToJson());
        builder.OwnsOne(e => e.SensorySystem, b => b.ToJson());
        builder.OwnsOne(e => e.Coordination, b => b.ToJson());
        builder.OwnsOne(e => e.Gait, b => b.ToJson());
        builder.OwnsOne(e => e.MeningealSigns, b => b.ToJson());

        // Relationships
        builder.HasOne(e => e.Encounter)
            .WithMany()
            .HasForeignKey(e => e.EncounterId);

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.ExaminedBy)
            .WithMany()
            .HasForeignKey(e => e.ExaminedById);

        // Indexes
        builder.HasIndex(e => e.EncounterId).IsUnique();
        builder.HasIndex(e => new { e.PatientId, e.ExamDate });

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
