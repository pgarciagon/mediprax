using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class ActionChainConfiguration : IEntityTypeConfiguration<ActionChain>
{
    public void Configure(EntityTypeBuilder<ActionChain> builder)
    {
        builder.ToTable("action_chains");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Shortcut).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Category).HasMaxLength(100);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

        builder.HasMany(x => x.Steps)
            .WithOne(s => s.ActionChain)
            .HasForeignKey(s => s.ActionChainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Shortcut).IsUnique();
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.IsActive);
    }
}
