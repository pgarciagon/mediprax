using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPrax.Infrastructure.Persistence.Configurations;

public class ActionChainStepConfiguration : IEntityTypeConfiguration<ActionChainStep>
{
    public void Configure(EntityTypeBuilder<ActionChainStep> builder)
    {
        builder.ToTable("action_chain_steps");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Configuration).HasMaxLength(4000).IsRequired();

        builder.HasIndex(x => new { x.ActionChainId, x.SortOrder });
    }
}
