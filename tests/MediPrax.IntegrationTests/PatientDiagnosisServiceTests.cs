using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class PatientDiagnosisServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly PatientDiagnosisService _sut;
    private readonly User _doctor;
    private readonly Patient _patient;

    public PatientDiagnosisServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new PatientDiagnosisService(_factory.Context);

        _doctor = new User
        {
            FirstName = "Dr. Test", LastName = "Arzt",
            Email = $"diag-{Guid.NewGuid():N}@test.de", PasswordHash = "hash",
            Role = UserRole.Arzt, IsActive = true
        };
        _patient = new Patient
        {
            FirstName = "Max", LastName = "Muster",
            DateOfBirth = new DateOnly(1990, 1, 1)
        };
        _factory.Context.Users.Add(_doctor);
        _factory.Context.Patients.Add(_patient);
        _factory.Context.SaveChanges();
    }

    // --- AC: Diagnoses can be created with metadata (certainty, laterality, type) ---

    [Fact]
    public async Task CreateAsync_CreatesDiagnosis_WithAllMetadata()
    {
        var result = await _sut.CreateAsync(new CreatePatientDiagnosisDto
        {
            PatientId = _patient.Id,
            Icd10Code = "F32.1",
            Certainty = DiagnosisCertainty.G,
            Laterality = null,
            DiagnosisType = DiagnosisType.Dauerdiagnose,
            OnsetDate = new DateOnly(2025, 6, 15),
            Notes = "Seit Sommer 2025",
            CreatedByDoctorId = _doctor.Id
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("F32.1", result.Icd10Code);
        Assert.Equal(DiagnosisCertainty.G, result.Certainty);
        Assert.Equal(DiagnosisType.Dauerdiagnose, result.DiagnosisType);
        Assert.Equal(DiagnosisStatus.Active, result.Status);
        Assert.Equal(new DateOnly(2025, 6, 15), result.OnsetDate);
    }

    [Fact]
    public async Task CreateAsync_WithLaterality_StoresCorrectly()
    {
        var result = await _sut.CreateAsync(new CreatePatientDiagnosisDto
        {
            PatientId = _patient.Id,
            Icd10Code = "G56.0",
            Certainty = DiagnosisCertainty.V,
            Laterality = DiagnosisLaterality.R,
            DiagnosisType = DiagnosisType.Encounterdiagnose,
            CreatedByDoctorId = _doctor.Id
        });

        Assert.Equal(DiagnosisLaterality.R, result.Laterality);
        Assert.Equal(DiagnosisCertainty.V, result.Certainty);
    }

    // --- AC: GetByPatient returns active diagnoses, optionally inactive ---

    [Fact]
    public async Task GetByPatientAsync_ReturnsActiveDiagnoses()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("F41.0", DiagnosisStatus.Active, DiagnosisType.Encounterdiagnose);
        await CreateDiagnosis("F10.2", DiagnosisStatus.Inactive, DiagnosisType.Dauerdiagnose);

        var active = await _sut.GetByPatientAsync(_patient.Id, includeInactive: false);
        Assert.Equal(2, active.Count);
        Assert.All(active, d => Assert.NotEqual(DiagnosisStatus.Inactive, d.Status));
    }

    [Fact]
    public async Task GetByPatientAsync_WithIncludeInactive_ReturnsAll()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("F10.2", DiagnosisStatus.Inactive, DiagnosisType.Dauerdiagnose);

        var all = await _sut.GetByPatientAsync(_patient.Id, includeInactive: true);
        Assert.Equal(2, all.Count);
    }

    // --- AC: GetDauerdiagnosen returns only active Dauerdiagnosen ---

    [Fact]
    public async Task GetDauerdiagnosenAsync_ReturnsOnlyActiveDauerdiagnosen()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("F41.0", DiagnosisStatus.Active, DiagnosisType.Encounterdiagnose);
        await CreateDiagnosis("F10.2", DiagnosisStatus.Inactive, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("G43.0", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);

        var dauer = await _sut.GetDauerdiagnosenAsync(_patient.Id);
        Assert.Equal(2, dauer.Count);
        Assert.All(dauer, d =>
        {
            Assert.Equal(DiagnosisType.Dauerdiagnose, d.DiagnosisType);
            Assert.Equal(DiagnosisStatus.Active, d.Status);
        });
    }

    // --- AC: UpdateAsync updates metadata ---

    [Fact]
    public async Task UpdateAsync_UpdatesMetadata()
    {
        var created = await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Encounterdiagnose);

        var updated = await _sut.UpdateAsync(new UpdatePatientDiagnosisDto
        {
            Id = created.Id,
            Certainty = DiagnosisCertainty.Z,
            Laterality = null,
            DiagnosisType = DiagnosisType.Dauerdiagnose,
            Status = DiagnosisStatus.Active,
            Notes = "Promoted to Dauerdiagnose"
        });

        Assert.Equal(DiagnosisCertainty.Z, updated.Certainty);
        Assert.Equal(DiagnosisType.Dauerdiagnose, updated.DiagnosisType);
        Assert.Equal("Promoted to Dauerdiagnose", updated.Notes);
    }

    // --- AC: DeactivateAsync sets Inactive and ResolvedDate ---

    [Fact]
    public async Task DeactivateAsync_SetsInactiveAndResolvedDate()
    {
        var created = await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);

        await _sut.DeactivateAsync(created.Id);

        var diagnoses = await _sut.GetByPatientAsync(_patient.Id, includeInactive: true);
        var deactivated = diagnoses.First(d => d.Id == created.Id);
        Assert.Equal(DiagnosisStatus.Inactive, deactivated.Status);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), deactivated.ResolvedDate);
    }

    // --- AC: Dauerdiagnosen auto-inherit to new encounters ---

    [Fact]
    public async Task InheritDauerdiagnosenForEncounterAsync_CreatesEncounterDiagnosisRecords()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("G43.0", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);

        var encounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _factory.Context.Set<Encounter>().Add(encounter);
        await _factory.Context.SaveChangesAsync();

        await _sut.InheritDauerdiagnosenForEncounterAsync(encounter.Id);

        var links = await _factory.Context.Set<EncounterDiagnosis>()
            .Where(ed => ed.EncounterId == encounter.Id)
            .ToListAsync();

        Assert.Equal(2, links.Count);
        Assert.All(links, l => Assert.False(l.IsNewInThisEncounter));
    }

    [Fact]
    public async Task InheritDauerdiagnosenForEncounterAsync_IsIdempotent()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);

        var encounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _factory.Context.Set<Encounter>().Add(encounter);
        await _factory.Context.SaveChangesAsync();

        await _sut.InheritDauerdiagnosenForEncounterAsync(encounter.Id);
        await _sut.InheritDauerdiagnosenForEncounterAsync(encounter.Id); // second call

        var links = await _factory.Context.Set<EncounterDiagnosis>()
            .Where(ed => ed.EncounterId == encounter.Id)
            .ToListAsync();

        Assert.Single(links);
    }

    [Fact]
    public async Task InheritDauerdiagnosenForEncounterAsync_SkipsInactiveDauerdiagnosen()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("F10.2", DiagnosisStatus.Inactive, DiagnosisType.Dauerdiagnose);

        var encounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _factory.Context.Set<Encounter>().Add(encounter);
        await _factory.Context.SaveChangesAsync();

        await _sut.InheritDauerdiagnosenForEncounterAsync(encounter.Id);

        var links = await _factory.Context.Set<EncounterDiagnosis>()
            .Where(ed => ed.EncounterId == encounter.Id)
            .ToListAsync();

        Assert.Single(links);
    }

    // --- AC: Migration converts legacy List<string> to PatientDiagnosis records ---

    [Fact]
    public async Task MigrateLegacyCodesAsync_CreatesPatientDiagnoses()
    {
        var encounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Icd10Codes = ["F32.1", "F41.0"]
        };
        _factory.Context.Set<Encounter>().Add(encounter);
        await _factory.Context.SaveChangesAsync();

        await _sut.MigrateLegacyCodesAsync(_patient.Id, _doctor.Id);

        var diagnoses = await _sut.GetByPatientAsync(_patient.Id, includeInactive: true);
        Assert.Equal(2, diagnoses.Count);
        Assert.Contains(diagnoses, d => d.Icd10Code == "F32.1");
        Assert.Contains(diagnoses, d => d.Icd10Code == "F41.0");
    }

    [Fact]
    public async Task MigrateLegacyCodesAsync_PromotesFrequentCodesToDauerdiagnose()
    {
        // F32.1 appears in 3 encounters → Dauerdiagnose
        // F41.0 appears in 1 encounter → Encounterdiagnose
        for (int i = 0; i < 3; i++)
        {
            _factory.Context.Set<Encounter>().Add(new Encounter
            {
                PatientId = _patient.Id,
                DoctorId = _doctor.Id,
                EncounterDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-i * 30)),
                Icd10Codes = ["F32.1"]
            });
        }
        _factory.Context.Set<Encounter>().Add(new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Icd10Codes = ["F41.0"]
        });
        await _factory.Context.SaveChangesAsync();

        await _sut.MigrateLegacyCodesAsync(_patient.Id, _doctor.Id);

        var diagnoses = await _sut.GetByPatientAsync(_patient.Id, includeInactive: true);
        var f321 = diagnoses.First(d => d.Icd10Code == "F32.1");
        var f410 = diagnoses.First(d => d.Icd10Code == "F41.0");

        Assert.Equal(DiagnosisType.Dauerdiagnose, f321.DiagnosisType);
        Assert.Equal(DiagnosisType.Encounterdiagnose, f410.DiagnosisType);
    }

    [Fact]
    public async Task MigrateLegacyCodesAsync_IsIdempotent()
    {
        _factory.Context.Set<Encounter>().Add(new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Icd10Codes = ["F32.1"]
        });
        await _factory.Context.SaveChangesAsync();

        await _sut.MigrateLegacyCodesAsync(_patient.Id, _doctor.Id);
        await _sut.MigrateLegacyCodesAsync(_patient.Id, _doctor.Id); // second call

        var diagnoses = await _sut.GetByPatientAsync(_patient.Id, includeInactive: true);
        Assert.Single(diagnoses);
    }

    // --- AC: Anamnestisch status preserved ---

    [Fact]
    public async Task GetByPatientAsync_AnamnestischIncludedByDefault()
    {
        await CreateDiagnosis("F32.1", DiagnosisStatus.Active, DiagnosisType.Dauerdiagnose);
        await CreateDiagnosis("F10.2", DiagnosisStatus.Anamnestisch, DiagnosisType.Dauerdiagnose);

        var result = await _sut.GetByPatientAsync(_patient.Id, includeInactive: false);
        Assert.Equal(2, result.Count); // Anamnestisch is not Inactive
    }

    // --- Helper ---

    private async Task<PatientDiagnosisDto> CreateDiagnosis(string code, DiagnosisStatus status, DiagnosisType type)
    {
        var dto = await _sut.CreateAsync(new CreatePatientDiagnosisDto
        {
            PatientId = _patient.Id,
            Icd10Code = code,
            Certainty = DiagnosisCertainty.G,
            DiagnosisType = type,
            CreatedByDoctorId = _doctor.Id
        });

        if (status != DiagnosisStatus.Active)
        {
            await _sut.UpdateAsync(new UpdatePatientDiagnosisDto
            {
                Id = dto.Id,
                Certainty = dto.Certainty,
                DiagnosisType = dto.DiagnosisType,
                Status = status,
                ResolvedDate = status == DiagnosisStatus.Inactive ? DateOnly.FromDateTime(DateTime.Today) : null
            });
        }

        return dto;
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
