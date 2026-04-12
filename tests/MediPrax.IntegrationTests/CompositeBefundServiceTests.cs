using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class CompositeBefundServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly CompositeBefundService _sut;
    private readonly PsychopathFindingService _psychopathService;
    private readonly NeurologicalExamService _neuroService;
    private readonly EncounterSectionService _sectionService;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;
    private readonly Guid _encounterId;

    public CompositeBefundServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);

        _psychopathService = new PsychopathFindingService(_factory.Context);
        _neuroService = new NeurologicalExamService(_factory.Context);
        _sectionService = new EncounterSectionService(_factory.Context);
        _sut = new CompositeBefundService(_psychopathService, _neuroService, _sectionService);

        var patient = new Patient
        {
            FirstName = "Composite", LastName = "Test",
            DateOfBirth = new DateOnly(1980, 6, 15)
        };
        var doctor = new User
        {
            FirstName = "Dr", LastName = "Composite",
            Email = $"composite-{Guid.NewGuid():N}@test.de",
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
    public async Task GetAsync_WithAmdpAndNeuroAndFreitext_ComposesAll()
    {
        // Arrange: create AMDP finding
        await _psychopathService.CreateAsync(new CreatePsychopathFindingDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings = new List<SymptomFinding>
            {
                new() { CategoryCode = "BEW", SymptomCode = "BEW-1", SymptomName = "Bewusstseinsverminderung", Severity = 0 }
            },
            NarrativeText = "Bewusstsein klar, allseits orientiert."
        });

        // Arrange: create neurological exam
        await _neuroService.CreateAsync(new CreateNeurologicalExamDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today),
            Reflexes = new ReflexFindings { BabinskiRight = true }
        });

        // Arrange: create free-text Befund section
        await _sectionService.CreateAsync(_encounterId, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Befund,
            Content = "Zusaetzlich: RR 130/80",
            AuthorId = _doctorId
        });

        // Act
        var result = await _sut.GetAsync(_encounterId);

        // Assert: all three sources present
        Assert.NotNull(result.PsychopathNarrative);
        Assert.Contains("Bewusstsein klar", result.PsychopathNarrative);
        Assert.True(result.PsychopathFindingId.HasValue);
        Assert.True(result.PsychopathAssessmentDate.HasValue);

        Assert.NotNull(result.NeuroNarrative);
        Assert.True(result.NeuroExamId.HasValue);
        Assert.True(result.NeuroExamDate.HasValue);

        Assert.Equal("Zusaetzlich: RR 130/80", result.FreitextBefund);

        // ComposedText should contain all three parts
        Assert.Contains("Psychopathologischer Befund:", result.ComposedText);
        Assert.Contains("Neurologischer Befund:", result.ComposedText);
        Assert.Contains("Zusaetzlich: RR 130/80", result.ComposedText);
        Assert.True(result.HasStructuredFindings);
    }

    [Fact]
    public async Task GetAsync_WithOnlyFreitext_ReturnsFreitextOnly()
    {
        await _sectionService.CreateAsync(_encounterId, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Befund,
            Content = "Blutdruck 120/80, Puls 72",
            AuthorId = _doctorId
        });

        var result = await _sut.GetAsync(_encounterId);

        Assert.Null(result.PsychopathNarrative);
        Assert.Null(result.PsychopathFindingId);
        Assert.Null(result.NeuroNarrative);
        Assert.Null(result.NeuroExamId);
        Assert.Equal("Blutdruck 120/80, Puls 72", result.FreitextBefund);
        Assert.False(result.HasStructuredFindings);
        Assert.DoesNotContain("Psychopathologischer Befund:", result.ComposedText);
        Assert.DoesNotContain("Neurologischer Befund:", result.ComposedText);
    }

    [Fact]
    public async Task GetAsync_WithOnlyAmdp_ReturnsAmdpOnly()
    {
        await _psychopathService.CreateAsync(new CreatePsychopathFindingDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings = new List<SymptomFinding>
            {
                new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 2 }
            },
            NarrativeText = "Deutlich depressive Stimmung."
        });

        var result = await _sut.GetAsync(_encounterId);

        Assert.NotNull(result.PsychopathNarrative);
        Assert.True(result.PsychopathFindingId.HasValue);
        Assert.Null(result.NeuroNarrative);
        Assert.Null(result.FreitextBefund);
        Assert.True(result.HasStructuredFindings);
        Assert.StartsWith("Psychopathologischer Befund:", result.ComposedText);
        Assert.DoesNotContain("Neurologischer Befund:", result.ComposedText);
    }

    [Fact]
    public async Task GetAsync_EmptyEncounter_ReturnsEmptyDto()
    {
        var result = await _sut.GetAsync(_encounterId);

        Assert.Null(result.PsychopathNarrative);
        Assert.Null(result.PsychopathFindingId);
        Assert.Null(result.NeuroNarrative);
        Assert.Null(result.NeuroExamId);
        Assert.Null(result.FreitextBefund);
        Assert.False(result.HasStructuredFindings);
        Assert.Equal(string.Empty, result.ComposedText);
    }

    public void Dispose() => _factory.Dispose();
}
