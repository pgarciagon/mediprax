using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

public class MedicationServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly MedicationService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;

    public MedicationServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new MedicationService(_factory.Context);

        var patient = new Patient { FirstName = "Med", LastName = "Test", DateOfBirth = new DateOnly(1990, 5, 5) };
        var doctor = new User { FirstName = "Dr", LastName = "Med", Email = "med@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();
        _patientId = patient.Id;
        _doctorId = doctor.Id;
    }

    [Fact]
    public async Task AddAsync_CreatesMedication()
    {
        var result = await _sut.AddAsync(new CreateMedicationDto
        {
            PatientId = _patientId, PrescribedById = _doctorId,
            Name = "Sertralin", Staerke = "50mg", Dosierung = "1-0-0-0",
            SeitDatum = DateOnly.FromDateTime(DateTime.Today)
        });

        Assert.Equal("Sertralin", result.Name);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsActiveOnly()
    {
        await _sut.AddAsync(new CreateMedicationDto
        {
            PatientId = _patientId, PrescribedById = _doctorId,
            Name = "Active Med", SeitDatum = DateOnly.FromDateTime(DateTime.Today)
        });

        var active = await _sut.GetByPatientAsync(_patientId, activeOnly: true);
        Assert.Single(active);

        var all = await _sut.GetByPatientAsync(_patientId, activeOnly: false);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeactivateAsync_SetsBisDatum()
    {
        var med = await _sut.AddAsync(new CreateMedicationDto
        {
            PatientId = _patientId, PrescribedById = _doctorId,
            Name = "ToDeactivate", SeitDatum = DateOnly.FromDateTime(DateTime.Today)
        });

        await _sut.DeactivateAsync(med.Id);

        var active = await _sut.GetByPatientAsync(_patientId, activeOnly: true);
        Assert.Empty(active);
    }

    public void Dispose() => _factory.Dispose();
}
