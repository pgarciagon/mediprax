using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class TextModuleConfiguration : IEntityTypeConfiguration<TextModule>
{
    public void Configure(EntityTypeBuilder<TextModule> builder)
    {
        builder.ToTable("text_modules");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(t => t.Shortcut).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Content).HasMaxLength(10000).IsRequired();
        builder.Property(t => t.Category).HasMaxLength(100);

        builder.HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById);

        builder.HasIndex(t => t.Shortcut);
        builder.HasIndex(t => t.Category);
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
