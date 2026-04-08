using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class PtvFormConfiguration : IEntityTypeConfiguration<PtvForm>
{
    public void Configure(EntityTypeBuilder<PtvForm> builder)
    {
        builder.ToTable("ptv_forms");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.CreatedDate).IsRequired();
        builder.Property(e => e.FormData).HasColumnType("jsonb");

        builder.HasOne(e => e.TherapyCase)
            .WithMany(tc => tc.PtvForms)
            .HasForeignKey(e => e.TherapyCaseId);

        builder.HasIndex(e => new { e.TherapyCaseId, e.FormType });
    }
}
