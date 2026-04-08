using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.IntegrationTests;

public class NeurologicalExamServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly NeurologicalExamService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;
    private readonly Guid _encounterId;

    public NeurologicalExamServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new NeurologicalExamService(_factory.Context);

        var patient = new Patient { FirstName = "Neuro", LastName = "Test", DateOfBirth = new DateOnly(1975, 3, 15) };
        var doctor = new User { FirstName = "Dr", LastName = "Neuro", Email = "neuro@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        var encounter = new Encounter { PatientId = patient.Id, DoctorId = doctor.Id, EncounterDate = DateOnly.FromDateTime(DateTime.Today) };

        // Need to set the navigation properties for EF tracking
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
    public async Task CreateAsync_CreatesExaminationWithAllFindings()
    {
        var dto = new CreateNeurologicalExamDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today),
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings { StrengthRightArm = 3 },
            Reflexes = new ReflexFindings { BabinskiRight = true },
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            Gait = new GaitFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var result = await _sut.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(_encounterId, result.EncounterId);
        Assert.Equal(3, result.MotorSystem.StrengthRightArm);
        Assert.True(result.Reflexes.BabinskiRight);
        Assert.NotNull(result.NarrativeText);
        Assert.Contains("Babinski", result.NarrativeText);
    }

    [Fact]
    public async Task GetByEncounterAsync_ReturnsExamination()
    {
        await _sut.CreateAsync(new CreateNeurologicalExamDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today)
        });

        var result = await _sut.GetByEncounterAsync(_encounterId);

        Assert.NotNull(result);
        Assert.Equal(_encounterId, result.EncounterId);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsAllExamsOrderedByDate()
    {
        // Create a second encounter for the same patient
        var encounter2 = new Encounter
        {
            PatientId = _patientId,
            DoctorId = _doctorId,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
            Patient = _factory.Context.Set<Patient>().Find(_patientId)!,
            Doctor = _factory.Context.Set<User>().Find(_doctorId)!
        };
        _factory.Context.Set<Encounter>().Add(encounter2);
        await _factory.Context.SaveChangesAsync();

        await _sut.CreateAsync(new CreateNeurologicalExamDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today)
        });

        await _sut.CreateAsync(new CreateNeurologicalExamDto
        {
            EncounterId = encounter2.Id,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7))
        });

        var results = await _sut.GetByPatientAsync(_patientId);

        Assert.Equal(2, results.Count);
        Assert.True(results[0].ExamDate >= results[1].ExamDate); // Ordered desc
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFindings()
    {
        var created = await _sut.CreateAsync(new CreateNeurologicalExamDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today),
            MotorSystem = new MotorFindings { StrengthRightArm = 5 }
        });

        var updated = await _sut.UpdateAsync(new UpdateNeurologicalExamDto
        {
            Id = created.Id,
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = created.ExamDate,
            MotorSystem = new MotorFindings { StrengthRightArm = 2 },
            Notes = "Verschlechterung"
        });

        Assert.Equal(2, updated.MotorSystem.StrengthRightArm);
        Assert.Equal("Verschlechterung", updated.Notes);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletes()
    {
        var created = await _sut.CreateAsync(new CreateNeurologicalExamDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            ExaminedById = _doctorId,
            ExamDate = DateOnly.FromDateTime(DateTime.Today)
        });

        await _sut.DeleteAsync(created.Id);

        var result = await _sut.GetByIdAsync(created.Id);
        Assert.Null(result); // Soft-deleted, not visible via query filter
    }

    public void Dispose() => _factory.Dispose();
}
