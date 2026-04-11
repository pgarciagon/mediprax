using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class EncounterServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly EncounterService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;

    public EncounterServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new EncounterService(_factory.Context);

        var patient = new Patient { FirstName = "Enc", LastName = "Test", DateOfBirth = new DateOnly(1980, 1, 1) };
        var doctor = new User { FirstName = "Dr", LastName = "Enc", Email = "enc@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();
        _patientId = patient.Id;
        _doctorId = doctor.Id;
    }

    [Fact]
    public async Task CreateAsync_CreatesEncounter()
    {
        var result = await _sut.CreateAsync(new CreateEncounterDto
        {
            PatientId = _patientId,
            DoctorId = _doctorId,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Icd10Codes = ["F32.1"],
            DurationMinutes = 20
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Contains(result.Icd10Codes, c => c.Code == "F32.1");
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsEncounters()
    {
        await _sut.CreateAsync(new CreateEncounterDto
        {
            PatientId = _patientId, DoctorId = _doctorId,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Icd10Codes = ["F41.0"]
        });

        var list = await _sut.GetByPatientAsync(_patientId);
        Assert.Single(list);
        Assert.Equal("F41.0", list[0].Icd10Codes[0]);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEncounter()
    {
        var created = await _sut.CreateAsync(new CreateEncounterDto
        {
            PatientId = _patientId, DoctorId = _doctorId,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Notes = "Initial"
        });

        await _sut.UpdateAsync(new UpdateEncounterDto
        {
            Id = created.Id,
            PatientId = _patientId, DoctorId = _doctorId,
            EncounterDate = created.EncounterDate,
            Notes = "Updated",
            Status = EncounterStatus.Completed
        });

        var updated = await _sut.GetByIdAsync(created.Id);
        Assert.Equal("Updated", updated!.Notes);
        Assert.Equal(EncounterStatus.Completed, updated.Status);
    }

    public void Dispose() => _factory.Dispose();
}
