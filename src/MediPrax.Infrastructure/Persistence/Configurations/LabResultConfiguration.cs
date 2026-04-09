using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class LabResultConfiguration : IEntityTypeConfiguration<LabResult>
{
    public void Configure(EntityTypeBuilder<LabResult> builder)
    {
        builder.ToTable("lab_results");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(l => l.LabName).HasMaxLength(200).IsRequired();
        builder.Property(l => l.Notes).HasMaxLength(2000);
        builder.Property(l => l.ImportSource).HasMaxLength(50);

        builder.Property(l => l.Values)
            .HasColumnType("jsonb");

        builder.HasOne(l => l.Patient)
            .WithMany(p => p.LabResults)
            .HasForeignKey(l => l.PatientId);

        builder.HasOne(l => l.OrderedBy)
            .WithMany()
            .HasForeignKey(l => l.OrderedById);

        builder.HasIndex(l => new { l.PatientId, l.OrderDate });
        builder.HasIndex(l => l.Status);

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}
