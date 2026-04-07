using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class RecallConfiguration : IEntityTypeConfiguration<Recall>
{
    public void Configure(EntityTypeBuilder<Recall> builder)
    {
        builder.ToTable("recalls");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(r => r.Reason).HasMaxLength(500).IsRequired();
        builder.Property(r => r.Notes).HasMaxLength(1000);

        builder.HasOne(r => r.Patient).WithMany(p => p.Recalls).HasForeignKey(r => r.PatientId);
        builder.HasOne(r => r.CreatedBy).WithMany().HasForeignKey(r => r.CreatedById);

        builder.HasIndex(r => new { r.DueDate, r.Status });
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
