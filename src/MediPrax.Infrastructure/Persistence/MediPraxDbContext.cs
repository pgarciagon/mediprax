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
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Recall> Recalls => Set<Recall>();
    public DbSet<PsychopathologicalFinding> PsychopathologicalFindings => Set<PsychopathologicalFinding>();
    public DbSet<PsychometricTest> PsychometricTests => Set<PsychometricTest>();
    public DbSet<TherapyCase> TherapyCases => Set<TherapyCase>();
    public DbSet<TherapySession> TherapySessions => Set<TherapySession>();
    public DbSet<PtvForm> PtvForms => Set<PtvForm>();
    public DbSet<NeurologicalExamination> NeurologicalExaminations => Set<NeurologicalExamination>();
    public DbSet<Icd10Code> Icd10Codes => Set<Icd10Code>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediPraxDbContext).Assembly);

        // Global query filters for soft delete
        modelBuilder.Entity<Patient>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Encounter>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Prescription>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Document>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BillingItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PsychopathologicalFinding>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PsychometricTest>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TherapyCase>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TherapySession>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PtvForm>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<NeurologicalExamination>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    // Convert hard delete to soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
