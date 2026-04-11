using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class TherapyCaseServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TherapyCaseService _sut;
    private readonly Guid _patientId;
    private readonly Guid _therapistId;

    public TherapyCaseServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new TherapyCaseService(_factory.Context);

        var patient = new Patient { FirstName = "Max", LastName = "Mustermann", DateOfBirth = new DateOnly(1985, 3, 15) };
        var therapist = new User { FirstName = "Dr. Anna", LastName = "Therapeut", Email = "anna@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(therapist);
        _factory.Context.SaveChanges();
        _patientId = patient.Id;
        _therapistId = therapist.Id;
    }

    [Fact]
    public async Task CreateAsync_CreatesTherapyCase()
    {
        var result = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50,
            Diagnoses = ["F32.1", "F41.0"]
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Kurzzeittherapie (KZT1)", result.TherapyTypeName);
        Assert.Equal(12, result.ApprovedSessions);
        Assert.Equal(0, result.CompletedSessions);
        Assert.Equal(TherapyCaseStatus.SprechstundePhase, result.Status);
        Assert.Equal(2, result.Diagnoses.Count);
    }

    [Fact]
    public async Task AddSessionAsync_IncrementsCompletedSessions()
    {
        var tc = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        await _sut.AddSessionAsync(new CreateTherapySessionDto
        {
            TherapyCaseId = tc.Id,
            SessionDate = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 50,
            SessionType = SessionType.Regular
        });

        var updated = await _sut.GetByIdAsync(tc.Id);
        Assert.Equal(1, updated!.CompletedSessions);
    }

    [Fact]
    public async Task AddSessionAsync_AssignsCorrectSessionNumber()
    {
        var tc = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        var s1 = await _sut.AddSessionAsync(new CreateTherapySessionDto
        {
            TherapyCaseId = tc.Id,
            SessionDate = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 50,
            SessionType = SessionType.Regular
        });

        var s2 = await _sut.AddSessionAsync(new CreateTherapySessionDto
        {
            TherapyCaseId = tc.Id,
            SessionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            DurationMinutes = 50,
            SessionType = SessionType.Regular
        });

        Assert.Equal(1, s1.SessionNumber);
        Assert.Equal(2, s2.SessionNumber);
    }

    [Fact]
    public async Task GetContingentStatusAsync_ReturnsCorrectRemainingSessions()
    {
        var tc = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        // Add 3 sessions
        for (int i = 0; i < 3; i++)
        {
            await _sut.AddSessionAsync(new CreateTherapySessionDto
            {
                TherapyCaseId = tc.Id,
                SessionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(i * 7)),
                DurationMinutes = 50,
                SessionType = SessionType.Regular
            });
        }

        var status = await _sut.GetContingentStatusAsync(tc.Id);
        Assert.Equal(12, status.ApprovedSessions);
        Assert.Equal(3, status.CompletedSessions);
        Assert.Equal(9, status.RemainingSessions);
        Assert.Equal("None", status.WarningLevel);
    }

    [Fact]
    public async Task GetContingentStatusAsync_ReturnsYellowWarningAt80Percent()
    {
        var tc = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        // Add 10 of 12 sessions (>= 80% threshold which is ceil(12*0.8)=10)
        for (int i = 0; i < 10; i++)
        {
            await _sut.AddSessionAsync(new CreateTherapySessionDto
            {
                TherapyCaseId = tc.Id,
                SessionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(i * 7)),
                DurationMinutes = 50,
                SessionType = SessionType.Regular
            });
        }

        var status = await _sut.GetContingentStatusAsync(tc.Id);
        Assert.Equal("Yellow", status.WarningLevel);
        Assert.Equal(2, status.RemainingSessions);
    }

    [Fact]
    public async Task GetContingentStatusAsync_ReturnsRedWarningAt100Percent()
    {
        var tc = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        // Add all 12 sessions
        for (int i = 0; i < 12; i++)
        {
            await _sut.AddSessionAsync(new CreateTherapySessionDto
            {
                TherapyCaseId = tc.Id,
                SessionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(i * 7)),
                DurationMinutes = 50,
                SessionType = SessionType.Regular
            });
        }

        var status = await _sut.GetContingentStatusAsync(tc.Id);
        Assert.Equal("Red", status.WarningLevel);
        Assert.Equal(0, status.RemainingSessions);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsAllCasesForPatient()
    {
        await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.PsychotherapeutischeSprechstunde,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 6,
            SessionDurationMinutes = 25
        });

        await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        var cases = await _sut.GetByPatientAsync(_patientId);
        Assert.Equal(2, cases.Count);
    }

    [Fact]
    public async Task GetActiveByTherapistAsync_FiltersByTherapist()
    {
        var otherTherapist = new User { FirstName = "Dr. B", LastName = "Other", Email = "other@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<User>().Add(otherTherapist);
        _factory.Context.SaveChanges();

        await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.KurzzeittherapieKZT1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 12,
            SessionDurationMinutes = 50
        });

        await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = otherTherapist.Id,
            TherapyType = TherapyType.Probatorik,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 4,
            SessionDurationMinutes = 50
        });

        var myCases = await _sut.GetActiveByTherapistAsync(_therapistId);
        Assert.Single(myCases);
    }

    [Fact]
    public async Task UpdateAsync_ChangesStatus()
    {
        var tc = await _sut.CreateAsync(new CreateTherapyCaseDto
        {
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.LangzeittherapieVT,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ApprovedSessions = 60,
            SessionDurationMinutes = 50,
            Diagnoses = ["F33.1"]
        });

        var updated = await _sut.UpdateAsync(new UpdateTherapyCaseDto
        {
            Id = tc.Id,
            PatientId = _patientId,
            TherapistId = _therapistId,
            TherapyType = TherapyType.LangzeittherapieVT,
            StartDate = tc.StartDate,
            ApprovedSessions = 60,
            SessionDurationMinutes = 50,
            Diagnoses = ["F33.1"],
            Status = TherapyCaseStatus.Bewilligt,
            InsuranceApprovalRef = "BEW-2026-001"
        });

        Assert.Equal(TherapyCaseStatus.Bewilligt, updated.Status);
    }

    public void Dispose() => _factory.Dispose();
}
