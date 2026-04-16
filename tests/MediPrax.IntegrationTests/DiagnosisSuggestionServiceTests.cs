using MediPrax.Application.Catalogs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class DiagnosisSuggestionServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly DiagnosisSuggestionService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;
    private readonly Guid _encounterId;

    public DiagnosisSuggestionServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new DiagnosisSuggestionService(_factory.Context);

        var patient = new Patient
        {
            FirstName = "DiagSugg", LastName = "Test",
            DateOfBirth = new DateOnly(1975, 3, 10)
        };
        var doctor = new User
        {
            FirstName = "Dr", LastName = "Suggest",
            Email = $"diagsugg-{Guid.NewGuid():N}@test.de",
            PasswordHash = "x", Role = UserRole.Arzt, IsActive = true
        };
        var encounter = new Encounter
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        encounter.Patient = patient;
        encounter.Doctor = doctor;

        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.Set<Encounter>().Add(encounter);
        _factory.Context.SaveChanges();

        _patientId = patient.Id;
        _doctorId = doctor.Id;
        _encounterId = encounter.Id;
    }

    [Fact]
    public async Task GetSuggestions_NoFindings_ReturnsEmpty()
    {
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSuggestions_WithDepressiveFindings_ReturnDepressionDiagnosis()
    {
        // Arrange: create AMDP finding with depressive symptoms
        var finding = new PsychopathologicalFinding
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings =
            [
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 2 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 2 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 1 },
                new SymptomFinding { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 2 },
                new SymptomFinding { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 1 },
            ]
        };
        _factory.Context.Set<PsychopathologicalFinding>().Add(finding);
        _factory.Context.SaveChanges();

        // Act
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, s => s.Icd10Code == "F32.1"); // Mittelgradige Depression
    }

    [Fact]
    public async Task GetSuggestions_WithParkinsonFindings_ReturnG200()
    {
        // Arrange: neurological exam with Parkinson features
        var neuro = new NeurologicalExamination
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today),
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings
            {
                ToneRightArm = "rigide", ToneLeftArm = "rigide",
                ToneRightLeg = "normal", ToneLeftLeg = "normal",
                StrengthRightArm = 5, StrengthLeftArm = 5,
                StrengthRightLeg = 5, StrengthLeftLeg = 5
            },
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            Gait = new GaitFindings
            {
                GaitPattern = "kleinschrittig",
                ArmSwing = "vermindert rechts"
            },
            MeningealSigns = new MeningealFindings()
        };
        _factory.Context.Set<NeurologicalExamination>().Add(neuro);
        _factory.Context.SaveChanges();

        // Act
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, s => s.Icd10Code == "G20.0");
    }

    [Fact]
    public async Task GetSuggestions_WithSuicidality_ReturnR458()
    {
        // Arrange: suicidality assessment for the patient
        var assessment = new SuicidalityAssessment
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            SuicidalIdeation = true,
            SuicidalPlans = true,
            SuicidalIntent = false,
            PriorAttempts = false,
            RiskLevel = SuicidalityRiskLevel.High
        };
        _factory.Context.Set<SuicidalityAssessment>().Add(assessment);
        _factory.Context.SaveChanges();

        // Act
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, s => s.Icd10Code == "R45.8");
        var suicSuggestion = result.First(s => s.Icd10Code == "R45.8");
        Assert.Equal(SuggestionConfidence.High, suicSuggestion.Confidence);
    }

    [Fact]
    public async Task GetSuggestions_CombinesAllSources_ReturnMultipleSuggestions()
    {
        // Arrange: depressive AMDP + suicidality → should get both F32.x and R45.8
        var finding = new PsychopathologicalFinding
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings =
            [
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 1 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 1 },
                new SymptomFinding { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 1 },
            ]
        };
        var suicAssessment = new SuicidalityAssessment
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            SuicidalIdeation = true,
            SuicidalPlans = false,
            SuicidalIntent = false,
            PriorAttempts = false,
            RiskLevel = SuicidalityRiskLevel.Moderate
        };
        _factory.Context.Set<PsychopathologicalFinding>().Add(finding);
        _factory.Context.Set<SuicidalityAssessment>().Add(suicAssessment);
        _factory.Context.SaveChanges();

        // Act
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert: both depression and suicidality suggestions
        Assert.True(result.Count >= 2);
        Assert.Contains(result, s => s.Icd10Code.StartsWith("F32"));
        Assert.Contains(result, s => s.Icd10Code == "R45.8");
    }

    [Fact]
    public async Task GetSuggestions_IgnoresDeletedFindings_ReturnsEmpty()
    {
        // Arrange: deleted finding should be ignored
        var finding = new PsychopathologicalFinding
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow,
            Findings =
            [
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 3 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 3 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 2 },
                new SymptomFinding { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 3 },
                new SymptomFinding { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 2 },
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-6", SymptomName = "Hoffnungslosigkeit", Severity = 2 },
            ]
        };
        _factory.Context.Set<PsychopathologicalFinding>().Add(finding);
        _factory.Context.SaveChanges();

        // Act
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSuggestions_DifferentEncounter_ReturnsEmpty()
    {
        // Arrange: finding on a different encounter
        var patient = _factory.Context.Set<Patient>().Find(_patientId)!;
        var doctor = _factory.Context.Set<User>().Find(_doctorId)!;
        var otherEncounter = new Encounter
        {
            PatientId = _patientId,
            DoctorId = _doctorId,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
            Patient = patient,
            Doctor = doctor
        };
        _factory.Context.Set<Encounter>().Add(otherEncounter);
        _factory.Context.SaveChanges();

        var finding = new PsychopathologicalFinding
        {
            EncounterId = otherEncounter.Id,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings =
            [
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 3 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 2 },
                new SymptomFinding { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 2 },
            ]
        };
        _factory.Context.Set<PsychopathologicalFinding>().Add(finding);
        _factory.Context.SaveChanges();

        // Act: query the original encounter (not the one with findings)
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSuggestions_SortedByConfidenceDescending()
    {
        // Arrange: severe depression (High confidence) + suicidality without plans (Medium)
        var finding = new PsychopathologicalFinding
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings =
            [
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 3 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 3 },
                new SymptomFinding { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 2 },
                new SymptomFinding { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 3 },
                new SymptomFinding { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 2 },
                new SymptomFinding { CategoryCode = "AFF", SymptomCode = "AFF-6", SymptomName = "Hoffnungslosigkeit", Severity = 2 },
            ]
        };
        var suicAssessment = new SuicidalityAssessment
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            SuicidalIdeation = true,
            SuicidalPlans = false,
            SuicidalIntent = false,
            PriorAttempts = false,
            RiskLevel = SuicidalityRiskLevel.Moderate
        };
        _factory.Context.Set<PsychopathologicalFinding>().Add(finding);
        _factory.Context.Set<SuicidalityAssessment>().Add(suicAssessment);
        _factory.Context.SaveChanges();

        // Act
        var result = await _sut.GetSuggestionsForEncounterAsync(_encounterId, _patientId);

        // Assert: High confidence suggestions come first
        Assert.True(result.Count >= 2);
        var first = result[0];
        Assert.Equal(SuggestionConfidence.High, first.Confidence);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
