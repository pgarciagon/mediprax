using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class PsychometricTestServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly PsychometricTestService _sut;
    private readonly Guid _patientId;
    private readonly Guid _userId;

    public PsychometricTestServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new PsychometricTestService(_factory.Context);

        var patient = new Patient { FirstName = "Test", LastName = "Patient", DateOfBirth = new DateOnly(1985, 3, 15) };
        var user = new User { FirstName = "Dr", LastName = "Tester", Email = "test@praxis.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(user);
        _factory.Context.SaveChanges();
        _patientId = patient.Id;
        _userId = user.Id;
    }

    [Fact]
    public async Task CreateAsync_CreatesTestInProgressStatus()
    {
        var result = await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.PHQ9,
            TestDate = DateOnly.FromDateTime(DateTime.Today)
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TestStatus.InProgress, result.Status);
        Assert.Equal(PsychometricTestType.PHQ9, result.TestType);
        Assert.Equal("PHQ-9", result.TestTypeName);
    }

    [Fact]
    public async Task SubmitResponsesAsync_CalculatesScoreAndInterpretation()
    {
        var test = await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.PHQ9,
            TestDate = DateOnly.FromDateTime(DateTime.Today)
        });

        // Score = 12 => Mittelgradig
        var responses = Enumerable.Range(1, 9).Select(i => new TestResponse
        {
            ItemNumber = i,
            ItemText = $"Item {i}",
            ResponseValue = i <= 4 ? 2 : 1,
            ResponseText = "Test"
        }).ToList();

        var result = await _sut.SubmitResponsesAsync(new SubmitTestResponsesDto
        {
            TestId = test.Id,
            Responses = responses
        });

        Assert.Equal(TestStatus.Completed, result.Status);
        Assert.Equal(13, result.TotalScore); // 4*2 + 5*1 = 13
        Assert.Equal("Mittelgradig", result.Interpretation);
        Assert.Equal(9, result.Responses.Count);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsTestsFilteredByType()
    {
        await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.PHQ9,
            TestDate = DateOnly.FromDateTime(DateTime.Today)
        });

        await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.GAD7,
            TestDate = DateOnly.FromDateTime(DateTime.Today)
        });

        var allTests = await _sut.GetByPatientAsync(_patientId);
        Assert.Equal(2, allTests.Count);

        var phqOnly = await _sut.GetByPatientAsync(_patientId, PsychometricTestType.PHQ9);
        Assert.Single(phqOnly);
        Assert.Equal(PsychometricTestType.PHQ9, phqOnly[0].TestType);
    }

    [Fact]
    public async Task GetScoreHistoryAsync_ReturnsScoreHistoryPoints()
    {
        // Create two completed PHQ-9 tests
        var test1 = await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.PHQ9,
            TestDate = new DateOnly(2026, 1, 1)
        });

        await _sut.SubmitResponsesAsync(new SubmitTestResponsesDto
        {
            TestId = test1.Id,
            Responses = Enumerable.Range(1, 9).Select(i => new TestResponse
            {
                ItemNumber = i, ItemText = $"Item {i}", ResponseValue = 2
            }).ToList()
        });

        var test2 = await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.PHQ9,
            TestDate = new DateOnly(2026, 2, 1)
        });

        await _sut.SubmitResponsesAsync(new SubmitTestResponsesDto
        {
            TestId = test2.Id,
            Responses = Enumerable.Range(1, 9).Select(i => new TestResponse
            {
                ItemNumber = i, ItemText = $"Item {i}", ResponseValue = 1
            }).ToList()
        });

        var history = await _sut.GetScoreHistoryAsync(_patientId, PsychometricTestType.PHQ9);

        Assert.Equal(2, history.Count);
        Assert.Equal(18, history[0].Score); // First test: 9*2
        Assert.Equal(9, history[1].Score);  // Second test: 9*1
        Assert.Equal(new DateOnly(2026, 1, 1), history[0].Date);
        Assert.Equal(new DateOnly(2026, 2, 1), history[1].Date);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTest()
    {
        var test = await _sut.CreateAsync(new CreatePsychometricTestDto
        {
            PatientId = _patientId,
            AdministeredById = _userId,
            TestType = PsychometricTestType.GAD7,
            TestDate = DateOnly.FromDateTime(DateTime.Today)
        });

        await _sut.DeleteAsync(test.Id);

        var result = await _sut.GetByIdAsync(test.Id);
        Assert.Null(result); // Soft-deleted, not visible via query filter
    }

    public void Dispose() => _factory.Dispose();
}
