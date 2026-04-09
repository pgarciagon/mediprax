using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.IntegrationTests;

/// <summary>
/// Validates that ALL entity DbSets can be queried without schema errors.
/// This catches column mismatches (e.g., new properties added to entities
/// without a corresponding database migration).
///
/// Each test creates the schema from the EF model, inserts a minimal entity,
/// then reads it back — ensuring columns, types, and relationships are valid.
/// </summary>
public class SchemaValidationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly MediPrax.Infrastructure.Persistence.MediPraxDbContext _db;

    public SchemaValidationTests()
    {
        _factory = new TestDbContextFactory();
        _db = _factory.Context;
    }

    // --- Helper: seed a patient + user (many entities require these FKs) ---
    private (Patient patient, User user) SeedBaseData()
    {
        var user = new User
        {
            FirstName = "Test", LastName = "Arzt", Email = "test@test.de",
            PasswordHash = "hash", Role = MediPrax.Core.Enums.UserRole.Arzt
        };
        _db.Users.Add(user);

        var patient = new Patient
        {
            FirstName = "Max", LastName = "Muster",
            DateOfBirth = new DateOnly(1990, 1, 1)
        };
        _db.Patients.Add(patient);
        _db.SaveChanges();
        return (patient, user);
    }

    [Fact]
    public async Task Patient_CanBeQueried_WithAllColumns()
    {
        var (patient, _) = SeedBaseData();
        // Set the new fields that caused the original bug
        patient.CurrentSuicidalityRisk = MediPrax.Core.Enums.SuicidalityRiskLevel.Low;
        patient.SuicidalityRiskUpdatedAt = DateTime.UtcNow;
        patient.HasLegalGuardian = true;
        patient.GuardianName = "Betreuer Test";
        patient.GuardianContact = "0421-123456";
        patient.GuardianScope = "Gesundheitsfürsorge";
        await _db.SaveChangesAsync();

        var loaded = await _db.Patients.FirstOrDefaultAsync(p => p.Id == patient.Id);
        Assert.NotNull(loaded);
        Assert.Equal(MediPrax.Core.Enums.SuicidalityRiskLevel.Low, loaded.CurrentSuicidalityRisk);
        Assert.True(loaded.HasLegalGuardian);
        Assert.Equal("Betreuer Test", loaded.GuardianName);
    }

    [Fact]
    public async Task Appointment_CanBeQueried_WithVideoAndSeriesFields()
    {
        var (patient, user) = SeedBaseData();
        var appt = new Appointment
        {
            PatientId = patient.Id, DoctorId = user.Id,
            StartTime = DateTime.UtcNow, DurationMinutes = 50,
            IsVideoConsultation = true, VideoLink = "https://video.example.com",
            VideoConsentGiven = true
        };
        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();

        var loaded = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == appt.Id);
        Assert.NotNull(loaded);
        Assert.True(loaded.IsVideoConsultation);
        Assert.Equal("https://video.example.com", loaded.VideoLink);
    }

    [Fact]
    public async Task AppointmentSeries_CanBeCreatedAndQueried()
    {
        var (patient, user) = SeedBaseData();
        var series = new AppointmentSeries
        {
            PatientId = patient.Id, DoctorId = user.Id,
            RecurrencePattern = MediPrax.Core.Enums.RecurrencePattern.Weekly,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0), DurationMinutes = 50,
            SeriesStartDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _db.AppointmentSeries.Add(series);
        await _db.SaveChangesAsync();

        var loaded = await _db.AppointmentSeries.FirstOrDefaultAsync(s => s.Id == series.Id);
        Assert.NotNull(loaded);
    }

    [Fact]
    public async Task WaitlistEntry_CanBeCreatedAndQueried()
    {
        var (patient, user) = SeedBaseData();
        var entry = new WaitlistEntry
        {
            PatientId = patient.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today),
            Priority = MediPrax.Core.Enums.WaitlistPriority.Urgent,
            PreferredDays = [DayOfWeek.Monday, DayOfWeek.Wednesday]
        };
        _db.WaitlistEntries.Add(entry);
        await _db.SaveChangesAsync();

        var loaded = await _db.WaitlistEntries.FirstOrDefaultAsync(w => w.Id == entry.Id);
        Assert.NotNull(loaded);
        Assert.Equal(MediPrax.Core.Enums.WaitlistPriority.Urgent, loaded.Priority);
    }

    [Fact]
    public async Task SeizureDiary_CanBeCreatedAndQueried()
    {
        var (patient, _) = SeedBaseData();
        _db.SeizureDiaries.Add(new SeizureDiary
        {
            PatientId = patient.Id, SeizureDate = DateTime.UtcNow,
            SeizureType = "Fokal", AuraPresent = true
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.SeizureDiaries.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal("Fokal", loaded.SeizureType);
    }

    [Fact]
    public async Task HeadacheDiary_CanBeCreatedAndQueried()
    {
        var (patient, _) = SeedBaseData();
        _db.HeadacheDiaries.Add(new HeadacheDiary
        {
            PatientId = patient.Id, Date = DateOnly.FromDateTime(DateTime.Today),
            Type = "Migräne", Intensity = 7,
            Triggers = ["Stress", "Schlafmangel"]
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.HeadacheDiaries.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.Triggers!.Count);
    }

    [Fact]
    public async Task MsDocumentation_CanBeCreatedAndQueried()
    {
        var (patient, _) = SeedBaseData();
        _db.MsDocumentations.Add(new MsDocumentation
        {
            PatientId = patient.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today),
            EdssScore = 3.5m, IsRelapse = true
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.MsDocumentations.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal(3.5m, loaded.EdssScore);
    }

    [Fact]
    public async Task ParkinsonDocumentation_CanBeCreatedAndQueried()
    {
        var (patient, _) = SeedBaseData();
        _db.ParkinsonDocumentations.Add(new ParkinsonDocumentation
        {
            PatientId = patient.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today),
            HoehnYahrStage = 2, Tremor = 2, Rigidity = 1, Bradykinesia = 3,
            NonMotorSymptoms = ["Depression", "Schlafstörung"]
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.ParkinsonDocumentations.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.NonMotorSymptoms!.Count);
    }

    [Fact]
    public async Task SuicidalityAssessment_CanBeCreatedAndQueried()
    {
        var (patient, user) = SeedBaseData();
        var encounter = new Encounter
        {
            PatientId = patient.Id, DoctorId = user.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _db.Encounters.Add(encounter);
        await _db.SaveChangesAsync();

        _db.SuicidalityAssessments.Add(new SuicidalityAssessment
        {
            PatientId = patient.Id, EncounterId = encounter.Id,
            AssessedById = user.Id, AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            RiskLevel = MediPrax.Core.Enums.SuicidalityRiskLevel.Moderate,
            SuicidalIdeation = true,
            RiskFactors = ["Vorherige Suizidversuche", "Isolation"],
            ProtectiveFactors = ["Soziales Netzwerk"],
            ActionsTaken = ["Krisenintervention durchgeführt"]
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.SuicidalityAssessments.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.RiskFactors.Count);
        Assert.Single(loaded.ProtectiveFactors);
    }

    [Fact]
    public async Task TextModule_CanBeCreatedAndQueried()
    {
        var (_, user) = SeedBaseData();
        _db.TextModules.Add(new TextModule
        {
            CreatedById = user.Id, Shortcut = "#test",
            Title = "Test", Content = "Test content", Category = "Test"
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.TextModules.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal("#test", loaded.Shortcut);
    }

    [Fact]
    public async Task DmpEnrollment_CanBeCreatedAndQueried()
    {
        var (patient, _) = SeedBaseData();
        var enrollment = new DmpEnrollment
        {
            PatientId = patient.Id, DmpType = "Depression",
            EnrollmentDate = DateOnly.FromDateTime(DateTime.Today),
            Status = MediPrax.Core.Enums.DmpStatus.Active
        };
        _db.DmpEnrollments.Add(enrollment);
        await _db.SaveChangesAsync();

        _db.DmpDocumentations.Add(new DmpDocumentation
        {
            DmpEnrollmentId = enrollment.Id, Quarter = "2026-Q2",
            DocumentationDate = DateOnly.FromDateTime(DateTime.Today),
            FormData = new Dictionary<string, string> { ["PHQ9"] = "12" }
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.DmpDocumentations.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal("12", loaded.FormData["PHQ9"]);
    }

    [Fact]
    public async Task PrivateInvoice_CanBeCreatedAndQueried()
    {
        var (patient, _) = SeedBaseData();
        _db.PrivateInvoices.Add(new PrivateInvoice
        {
            PatientId = patient.Id, InvoiceNumber = "RE-2026-0001",
            InvoiceDate = DateOnly.FromDateTime(DateTime.Today),
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            Status = MediPrax.Core.Enums.InvoiceStatus.Draft,
            Items = [new InvoiceItem { GoaePosition = "1", Description = "Beratung", Factor = 2.3m, UnitPrice = 10m, Quantity = 1 }],
            TotalNet = 23m, TotalGross = 23m
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.PrivateInvoices.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Single(loaded.Items);
    }

    [Fact]
    public async Task BtmPrescription_CanBeCreatedAndQueried()
    {
        var (patient, user) = SeedBaseData();
        _db.BtmPrescriptions.Add(new BtmPrescription
        {
            PatientId = patient.Id, PrescribedById = user.Id,
            PrescriptionDate = DateOnly.FromDateTime(DateTime.Today),
            MedicationName = "Methylphenidat", Substance = "Methylphenidat",
            Amount = "30 Tbl.", Dosierung = "1-0-0",
            BtmRecipeNumber = "1234567", PrescriberBtmNumber = "BtM-12345"
        });
        await _db.SaveChangesAsync();

        var loaded = await _db.BtmPrescriptions.FirstOrDefaultAsync();
        Assert.NotNull(loaded);
        Assert.Equal("Methylphenidat", loaded.Substance);
    }

    [Fact]
    public async Task AllDbSets_CanBeQueried_WithoutSchemaErrors()
    {
        // Catch-all: query every DbSet to ensure no column mismatches.
        // If an entity has a property that doesn't exist in the DB schema,
        // this test will fail with a column-not-found error.
        Assert.True(await _db.Users.CountAsync() >= 0);
        Assert.True(await _db.Patients.CountAsync() >= 0);
        Assert.True(await _db.Appointments.CountAsync() >= 0);
        Assert.True(await _db.Encounters.CountAsync() >= 0);
        Assert.True(await _db.Prescriptions.CountAsync() >= 0);
        Assert.True(await _db.Documents.CountAsync() >= 0);
        Assert.True(await _db.BillingItems.CountAsync() >= 0);
        Assert.True(await _db.AuditLogs.CountAsync() >= 0);
        Assert.True(await _db.Medications.CountAsync() >= 0);
        Assert.True(await _db.Recalls.CountAsync() >= 0);
        Assert.True(await _db.PsychopathologicalFindings.CountAsync() >= 0);
        Assert.True(await _db.PsychometricTests.CountAsync() >= 0);
        Assert.True(await _db.TherapyCases.CountAsync() >= 0);
        Assert.True(await _db.TherapySessions.CountAsync() >= 0);
        Assert.True(await _db.PtvForms.CountAsync() >= 0);
        Assert.True(await _db.NeurologicalExaminations.CountAsync() >= 0);
        Assert.True(await _db.Icd10Codes.CountAsync() >= 0);
        Assert.True(await _db.LabResults.CountAsync() >= 0);
        Assert.True(await _db.AppointmentSeries.CountAsync() >= 0);
        Assert.True(await _db.WaitlistEntries.CountAsync() >= 0);
        Assert.True(await _db.SeizureDiaries.CountAsync() >= 0);
        Assert.True(await _db.HeadacheDiaries.CountAsync() >= 0);
        Assert.True(await _db.MsDocumentations.CountAsync() >= 0);
        Assert.True(await _db.ParkinsonDocumentations.CountAsync() >= 0);
        Assert.True(await _db.SuicidalityAssessments.CountAsync() >= 0);
        Assert.True(await _db.TextModules.CountAsync() >= 0);
        Assert.True(await _db.DmpEnrollments.CountAsync() >= 0);
        Assert.True(await _db.DmpDocumentations.CountAsync() >= 0);
        Assert.True(await _db.PrivateInvoices.CountAsync() >= 0);
        Assert.True(await _db.BtmPrescriptions.CountAsync() >= 0);
    }

    public void Dispose() => _factory.Dispose();
}
