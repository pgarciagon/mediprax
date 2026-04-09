using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PrivateInvoiceConfiguration : IEntityTypeConfiguration<PrivateInvoice>
{
    public void Configure(EntityTypeBuilder<PrivateInvoice> builder)
    {
        builder.ToTable("private_invoices");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(i => i.TotalNet).HasPrecision(10, 2);
        builder.Property(i => i.TotalGross).HasPrecision(10, 2);
        builder.Property(i => i.PaidAmount).HasPrecision(10, 2);
        builder.Property(i => i.Notes).HasMaxLength(2000);

        builder.Property(i => i.Items)
            .HasColumnType("jsonb");

        builder.HasOne(i => i.Patient)
            .WithMany()
            .HasForeignKey(i => i.PatientId);

        builder.HasIndex(i => i.PatientId);
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.Status);
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
