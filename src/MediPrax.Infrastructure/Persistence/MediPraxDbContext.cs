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
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<AppointmentSeries> AppointmentSeries => Set<AppointmentSeries>();
    public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();
    public DbSet<SeizureDiary> SeizureDiaries => Set<SeizureDiary>();
    public DbSet<HeadacheDiary> HeadacheDiaries => Set<HeadacheDiary>();
    public DbSet<MsDocumentation> MsDocumentations => Set<MsDocumentation>();
    public DbSet<ParkinsonDocumentation> ParkinsonDocumentations => Set<ParkinsonDocumentation>();
    public DbSet<SuicidalityAssessment> SuicidalityAssessments => Set<SuicidalityAssessment>();

    // M30: Text Modules
    public DbSet<TextModule> TextModules => Set<TextModule>();

    // M31: DMP
    public DbSet<DmpEnrollment> DmpEnrollments => Set<DmpEnrollment>();
    public DbSet<DmpDocumentation> DmpDocumentations => Set<DmpDocumentation>();

    // M33: Private Invoices
    public DbSet<PrivateInvoice> PrivateInvoices => Set<PrivateInvoice>();

    // M35: BtM Prescriptions
    public DbSet<BtmPrescription> BtmPrescriptions => Set<BtmPrescription>();

    // M40: Availability
    public DbSet<DoctorScheduleTemplate> DoctorScheduleTemplates => Set<DoctorScheduleTemplate>();
    public DbSet<DoctorAbsence> DoctorAbsences => Set<DoctorAbsence>();

    // M43: Encounter Sections (ABDTP)
    public DbSet<EncounterSection> EncounterSections => Set<EncounterSection>();

    // M44: Advanced Diagnosis Management
    public DbSet<PatientDiagnosis> PatientDiagnoses => Set<PatientDiagnosis>();
    public DbSet<EncounterDiagnosis> EncounterDiagnoses => Set<EncounterDiagnosis>();

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
        modelBuilder.Entity<LabResult>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AppointmentSeries>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WaitlistEntry>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SeizureDiary>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<HeadacheDiary>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MsDocumentation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ParkinsonDocumentation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SuicidalityAssessment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DoctorScheduleTemplate>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DoctorAbsence>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EncounterSection>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PatientDiagnosis>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EncounterDiagnosis>().HasQueryFilter(e => !e.IsDeleted);
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
