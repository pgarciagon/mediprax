using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(d => d.Title).HasMaxLength(300).IsRequired();
        builder.Property(d => d.KimMessageId).HasMaxLength(200);

        builder.HasOne(d => d.Patient)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.PatientId);

        builder.HasOne(d => d.Encounter)
            .WithMany(e => e.Documents)
            .HasForeignKey(d => d.EncounterId);

        builder.HasIndex(d => d.PatientId);
    }
}
