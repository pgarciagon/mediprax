using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class SeizureDiaryConfiguration : IEntityTypeConfiguration<SeizureDiary>
{
    public void Configure(EntityTypeBuilder<SeizureDiary> builder)
    {
        builder.ToTable("seizure_diaries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.SeizureType).IsRequired();

        builder.HasOne(e => e.Patient).WithMany().HasForeignKey(e => e.PatientId);
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.SeizureDate);
    }
}

public class HeadacheDiaryConfiguration : IEntityTypeConfiguration<HeadacheDiary>
{
    public void Configure(EntityTypeBuilder<HeadacheDiary> builder)
    {
        builder.ToTable("headache_diaries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.Type).IsRequired();
        builder.Property(e => e.Triggers).HasColumnType("jsonb");

        builder.HasOne(e => e.Patient).WithMany().HasForeignKey(e => e.PatientId);
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.Date);
    }
}

public class MsDocumentationConfiguration : IEntityTypeConfiguration<MsDocumentation>
{
    public void Configure(EntityTypeBuilder<MsDocumentation> builder)
    {
        builder.ToTable("ms_documentations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.EdssScore).HasPrecision(3, 1);

        builder.HasOne(e => e.Patient).WithMany().HasForeignKey(e => e.PatientId);
        builder.HasIndex(e => e.PatientId);
    }
}

public class ParkinsonDocumentationConfiguration : IEntityTypeConfiguration<ParkinsonDocumentation>
{
    public void Configure(EntityTypeBuilder<ParkinsonDocumentation> builder)
    {
        builder.ToTable("parkinson_documentations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.HoehnYahrStage).HasPrecision(2, 1);
        builder.Property(e => e.NonMotorSymptoms).HasColumnType("jsonb");

        builder.HasOne(e => e.Patient).WithMany().HasForeignKey(e => e.PatientId);
        builder.HasIndex(e => e.PatientId);
    }
}
