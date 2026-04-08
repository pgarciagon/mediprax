using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.IntegrationTests;

public class PsychopathFindingServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly PsychopathFindingService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;
    private readonly Guid _encounterId;

    public PsychopathFindingServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new PsychopathFindingService(_factory.Context);

        var patient = new Patient { FirstName = "Psych", LastName = "Test", DateOfBirth = new DateOnly(1985, 3, 15) };
        var doctor = new User { FirstName = "Dr", LastName = "Psych", Email = "psych@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        var encounter = new Encounter
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        // Must set patient/doctor first so IDs are available
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();

        encounter.PatientId = patient.Id;
        encounter.DoctorId = doctor.Id;
        _factory.Context.Set<Encounter>().Add(encounter);
        _factory.Context.SaveChanges();

        _patientId = patient.Id;
        _doctorId = doctor.Id;
        _encounterId = encounter.Id;
    }

    [Fact]
    public async Task CreateAsync_CreatesFindingWithSymptoms()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "BEW", SymptomCode = "BEW-1", SymptomName = "Bewusstseinsverminderung", Severity = 0 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert/Traurig", Severity = 2, Comment = "seit 3 Wochen" },
        };

        var result = await _sut.CreateAsync(new CreatePsychopathFindingDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings = findings,
            NarrativeText = "Bewusstsein klar, deutlich deprimiert.",
            Notes = "Erstbefund"
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(2, result.Findings.Count);
        Assert.Equal("Bewusstsein klar, deutlich deprimiert.", result.NarrativeText);
    }

    [Fact]
    public async Task GetByEncounterAsync_ReturnsFinding()
    {
        await CreateTestFinding();

        var result = await _sut.GetByEncounterAsync(_encounterId);

        Assert.NotNull(result);
        Assert.Equal(_encounterId, result.EncounterId);
        Assert.NotEmpty(result.Findings);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsAllFindingsOrderedByDate()
    {
        // Create a second encounter for the same patient
        var encounter2 = new Encounter
        {
            PatientId = _patientId,
            DoctorId = _doctorId,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7))
        };
        _factory.Context.Set<Encounter>().Add(encounter2);
        _factory.Context.SaveChanges();

        // Create finding for first encounter (today)
        await _sut.CreateAsync(new CreatePsychopathFindingDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings = [new() { CategoryCode = "BEW", SymptomCode = "BEW-1", SymptomName = "Test", Severity = 0 }],
            NarrativeText = "Befund 1"
        });

        // Create finding for second encounter (last week)
        await _sut.CreateAsync(new CreatePsychopathFindingDto
        {
            EncounterId = encounter2.Id,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
            Findings = [new() { CategoryCode = "BEW", SymptomCode = "BEW-1", SymptomName = "Test", Severity = 0 }],
            NarrativeText = "Befund 2"
        });

        var list = await _sut.GetByPatientAsync(_patientId);

        Assert.Equal(2, list.Count);
        // Should be ordered by date descending (most recent first)
        Assert.True(list[0].AssessmentDate >= list[1].AssessmentDate);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFinding()
    {
        var created = await CreateTestFinding();

        var updatedFindings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert/Traurig", Severity = 3 },
        };

        var result = await _sut.UpdateAsync(new UpdatePsychopathFindingDto
        {
            Id = created.Id,
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = created.AssessmentDate,
            Findings = updatedFindings,
            NarrativeText = "Schwer deprimiert.",
            Notes = "Verschlechterung"
        });

        Assert.Equal("Schwer deprimiert.", result.NarrativeText);
        Assert.Equal("Verschlechterung", result.Notes);
        Assert.Single(result.Findings);
        Assert.Equal(3, result.Findings[0].Severity);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesFinding()
    {
        var created = await CreateTestFinding();

        await _sut.DeleteAsync(created.Id);

        var result = await _sut.GetByIdAsync(created.Id);
        Assert.Null(result);
    }

    private async Task<PsychopathFindingDto> CreateTestFinding()
    {
        return await _sut.CreateAsync(new CreatePsychopathFindingDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            AssessedById = _doctorId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            Findings =
            [
                new() { CategoryCode = "BEW", SymptomCode = "BEW-1", SymptomName = "Bewusstseinsverminderung", Severity = 0 },
                new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert/Traurig", Severity = 2 },
            ],
            NarrativeText = "Bewusstsein klar, deutlich deprimiert.",
            Notes = "Testbefund"
        });
    }

    public void Dispose() => _factory.Dispose();
}
