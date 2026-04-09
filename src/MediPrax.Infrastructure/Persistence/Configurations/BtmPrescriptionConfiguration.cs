using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class BtmPrescriptionConfiguration : IEntityTypeConfiguration<BtmPrescription>
{
    public void Configure(EntityTypeBuilder<BtmPrescription> builder)
    {
        builder.ToTable("btm_prescriptions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.MedicationName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Pzn).HasMaxLength(20);
        builder.Property(p => p.Substance).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Amount).HasMaxLength(100);
        builder.Property(p => p.Dosierung).HasMaxLength(200);
        builder.Property(p => p.BtmRecipeNumber).HasMaxLength(100).IsRequired();
        builder.Property(p => p.PrescriberBtmNumber).HasMaxLength(50);
        builder.Property(p => p.Notes).HasMaxLength(2000);

        builder.HasOne(p => p.Patient)
            .WithMany()
            .HasForeignKey(p => p.PatientId);

        builder.HasOne(p => p.PrescribedBy)
            .WithMany()
            .HasForeignKey(p => p.PrescribedById);

        builder.HasIndex(p => p.PatientId);
        builder.HasIndex(p => p.PrescriptionDate);
        builder.HasIndex(p => p.Substance);
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
