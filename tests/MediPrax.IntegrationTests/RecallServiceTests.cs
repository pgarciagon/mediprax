using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

public class RecallServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly RecallService _sut;
    private readonly Guid _patientId;
    private readonly Guid _userId;

    public RecallServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new RecallService(_factory.Context);

        var patient = new Patient { FirstName = "Recall", LastName = "Test", DateOfBirth = new DateOnly(1985, 1, 1) };
        var user = new User { FirstName = "Dr", LastName = "Recall", Email = "recall@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(user);
        _factory.Context.SaveChanges();
        _patientId = patient.Id;
        _userId = user.Id;
    }

    [Fact]
    public async Task CreateAsync_CreatesRecall()
    {
        var result = await _sut.CreateAsync(new CreateRecallDto
        {
            PatientId = _patientId, CreatedById = _userId,
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(3)),
            Reason = "Kontrolltermin"
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Kontrolltermin", result.Reason);
        Assert.Equal(RecallStatus.Open, result.Status);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsRecalls()
    {
        await _sut.CreateAsync(new CreateRecallDto
        {
            PatientId = _patientId, CreatedById = _userId,
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
            Reason = "Blutwerte"
        });

        var list = await _sut.GetByPatientAsync(_patientId);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsOpenAndScheduled()
    {
        await _sut.CreateAsync(new CreateRecallDto
        {
            PatientId = _patientId, CreatedById = _userId,
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            Reason = "Pending"
        });

        var pending = await _sut.GetPendingAsync();
        Assert.True(pending.Count >= 1);
    }

    [Fact]
    public async Task UpdateStatusAsync_CompletesRecall()
    {
        var recall = await _sut.CreateAsync(new CreateRecallDto
        {
            PatientId = _patientId, CreatedById = _userId,
            DueDate = DateOnly.FromDateTime(DateTime.Today),
            Reason = "ToComplete"
        });

        await _sut.UpdateStatusAsync(recall.Id, RecallStatus.Completed);

        var list = await _sut.GetByPatientAsync(_patientId);
        Assert.Equal(RecallStatus.Completed, list.First(r => r.Id == recall.Id).Status);
    }

    [Fact]
    public async Task IsOverdue_TrueForPastDue()
    {
        var recall = await _sut.CreateAsync(new CreateRecallDto
        {
            PatientId = _patientId, CreatedById = _userId,
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            Reason = "Overdue test"
        });

        Assert.True(recall.IsOverdue);
    }

    public void Dispose() => _factory.Dispose();
}
