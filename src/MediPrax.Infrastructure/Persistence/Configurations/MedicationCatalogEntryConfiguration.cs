using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class MedicationCatalogEntryConfiguration : IEntityTypeConfiguration<MedicationCatalogEntry>
{
    public void Configure(EntityTypeBuilder<MedicationCatalogEntry> builder)
    {
        builder.ToTable("medication_catalog");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.Pzn).HasMaxLength(8).IsRequired();
        builder.Property(e => e.Handelsname).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Wirkstoff).HasMaxLength(200).IsRequired();
        builder.Property(e => e.WirkstoffAsk).HasMaxLength(20);
        builder.Property(e => e.AtcCode).HasMaxLength(10);
        builder.Property(e => e.Staerke).HasMaxLength(50);
        builder.Property(e => e.Darreichungsform).HasMaxLength(100);
        builder.Property(e => e.Packungsgroesse).HasMaxLength(50);
        builder.Property(e => e.NormPackungsgroesse).HasMaxLength(5);
        builder.Property(e => e.Hersteller).HasMaxLength(100);
        builder.Property(e => e.Festbetrag).HasPrecision(10, 2);
        builder.Property(e => e.Avp).HasPrecision(10, 2);
        builder.Property(e => e.DataSource).HasMaxLength(20).HasDefaultValue("SEED");
        builder.Property(e => e.Category).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(e => e.Pzn).IsUnique();
        builder.HasIndex(e => e.Wirkstoff);
        builder.HasIndex(e => e.AtcCode);
        builder.HasIndex(e => e.Handelsname);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => new { e.IsActive, e.Category });
    }
}
