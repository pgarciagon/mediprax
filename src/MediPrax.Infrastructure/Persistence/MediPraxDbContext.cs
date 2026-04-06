using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Infrastructure.Persistence;

public class MediPraxDbContext : DbContext
{
    public MediPraxDbContext(DbContextOptions<MediPraxDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<BillingItem> BillingItems => Set<BillingItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediPraxDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
