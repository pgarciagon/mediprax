using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

public class BillingServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly BillingService _sut;
    private readonly Guid _patientId;
    private readonly Guid _encounterId;

    public BillingServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new BillingService(_factory.Context);

        var patient = new Patient { FirstName = "Billing", LastName = "Test", DateOfBirth = new DateOnly(1980, 5, 10) };
        var doctor = new User { FirstName = "Dr", LastName = "Bill", Email = "bill@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();

        var encounter = new Encounter { PatientId = patient.Id, DoctorId = doctor.Id, EncounterDate = DateOnly.FromDateTime(DateTime.Today) };
        _factory.Context.Set<Encounter>().Add(encounter);
        _factory.Context.SaveChanges();

        _patientId = patient.Id;
        _encounterId = encounter.Id;
    }

    [Fact]
    public async Task AddAsync_CreatesBillingItem()
    {
        var result = await _sut.AddAsync(new CreateBillingItemDto
        {
            EncounterId = _encounterId,
            PatientId = _patientId,
            GopCode = "16220",
            BillingType = BillingType.EBM
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("16220", result.GopCode);
        Assert.True(result.Amount > 0);
    }

    [Fact]
    public async Task GetByEncounterAsync_ReturnsItems()
    {
        await _sut.AddAsync(new CreateBillingItemDto
        {
            EncounterId = _encounterId, PatientId = _patientId,
            GopCode = "21220", BillingType = BillingType.EBM
        });
        await _sut.AddAsync(new CreateBillingItemDto
        {
            EncounterId = _encounterId, PatientId = _patientId,
            GopCode = "21210", BillingType = BillingType.EBM
        });

        var items = await _sut.GetByEncounterAsync(_encounterId);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBillingItem()
    {
        var item = await _sut.AddAsync(new CreateBillingItemDto
        {
            EncounterId = _encounterId, PatientId = _patientId,
            GopCode = "16210", BillingType = BillingType.EBM
        });

        await _sut.DeleteAsync(item.Id);

        var items = await _sut.GetByEncounterAsync(_encounterId);
        Assert.Empty(items);
    }

    [Fact]
    public async Task GetQuarterOverviewAsync_GroupsByPatient()
    {
        await _sut.AddAsync(new CreateBillingItemDto
        {
            EncounterId = _encounterId, PatientId = _patientId,
            GopCode = "857", BillingType = BillingType.GOA
        });

        var quarter = _sut.GetCurrentQuarter();
        var overview = await _sut.GetQuarterOverviewAsync(quarter);

        Assert.Equal(1, overview.Patients.Count);
        Assert.True(overview.TotalAmount > 0);
    }

    [Fact]
    public void GetCurrentQuarter_ReturnsValidFormat()
    {
        var quarter = _sut.GetCurrentQuarter();
        Assert.Matches(@"^\d{4}-Q[1-4]$", quarter);
    }

    public void Dispose() => _factory.Dispose();
}
