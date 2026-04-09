using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class DmpEnrollmentConfiguration : IEntityTypeConfiguration<DmpEnrollment>
{
    public void Configure(EntityTypeBuilder<DmpEnrollment> builder)
    {
        builder.ToTable("dmp_enrollments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.DmpType).HasMaxLength(100).IsRequired();

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId);

        builder.HasMany(e => e.Documentations)
            .WithOne(d => d.DmpEnrollment)
            .HasForeignKey(d => d.DmpEnrollmentId);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.Status);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class DmpDocumentationConfiguration : IEntityTypeConfiguration<DmpDocumentation>
{
    public void Configure(EntityTypeBuilder<DmpDocumentation> builder)
    {
        builder.ToTable("dmp_documentations");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(d => d.Quarter).HasMaxLength(10).IsRequired();
        builder.Property(d => d.FormData).HasColumnType("jsonb");

        builder.HasOne(d => d.DmpEnrollment)
            .WithMany(e => e.Documentations)
            .HasForeignKey(d => d.DmpEnrollmentId);

        builder.HasIndex(d => d.DmpEnrollmentId);
        builder.HasIndex(d => d.Quarter);
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
