using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class BillingItemConfiguration : IEntityTypeConfiguration<BillingItem>
{
    public void Configure(EntityTypeBuilder<BillingItem> builder)
    {
        builder.ToTable("billing_items");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(b => b.GopCode).HasMaxLength(10).IsRequired();
        builder.Property(b => b.GopDescription).HasMaxLength(300);
        builder.Property(b => b.Quantity).HasDefaultValue(1);
        builder.Property(b => b.Quarter).HasMaxLength(7);

        builder.HasOne(b => b.Encounter)
            .WithMany(e => e.BillingItems)
            .HasForeignKey(b => b.EncounterId);

        builder.HasIndex(b => new { b.PatientId, b.Quarter });
        builder.HasIndex(b => b.KvdtExported).HasFilter("\"KvdtExported\" = false");
    }
}
