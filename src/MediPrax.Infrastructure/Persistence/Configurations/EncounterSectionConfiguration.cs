using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class EncounterSectionConfiguration : IEntityTypeConfiguration<EncounterSection>
{
    public void Configure(EntityTypeBuilder<EncounterSection> builder)
    {
        builder.ToTable("encounter_sections");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.Content).IsRequired();
        builder.Property(e => e.SectionType).IsRequired();
        builder.Property(e => e.SortOrder).IsRequired();

        builder.HasOne(e => e.Encounter)
            .WithMany(enc => enc.Sections)
            .HasForeignKey(e => e.EncounterId);

        builder.HasOne(e => e.Author)
            .WithMany()
            .HasForeignKey(e => e.AuthorId)
            .IsRequired(false);

        builder.HasIndex(e => new { e.EncounterId, e.SectionType });
        builder.HasIndex(e => e.EncounterId);
    }
}
