using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.ToTable("medications");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(m => m.Name).HasMaxLength(300).IsRequired();
        builder.Property(m => m.Wirkstoff).HasMaxLength(200);
        builder.Property(m => m.Pzn).HasMaxLength(10);
        builder.Property(m => m.Staerke).HasMaxLength(50);
        builder.Property(m => m.Dosierung).HasMaxLength(50);
        builder.Property(m => m.Einheit).HasMaxLength(50);
        builder.Property(m => m.Hinweis).HasMaxLength(500);

        builder.HasOne(m => m.Patient).WithMany(p => p.Medications).HasForeignKey(m => m.PatientId);
        builder.HasOne(m => m.PrescribedBy).WithMany().HasForeignKey(m => m.PrescribedById);

        builder.HasIndex(m => new { m.PatientId, m.IsActive });
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
