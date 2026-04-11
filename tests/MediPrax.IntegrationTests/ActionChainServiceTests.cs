using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class ActionChainServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly ActionChainService _sut;
    private readonly User _doctor;

    public ActionChainServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new ActionChainService(_factory.Context);

        _doctor = new User
        {
            FirstName = "Dr. Test", LastName = "Arzt",
            Email = $"test-{Guid.NewGuid():N}@test.de", PasswordHash = "hash",
            Role = UserRole.Arzt, IsActive = true
        };
        _factory.Context.Users.Add(_doctor);
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_CreatesChainWithSteps()
    {
        var dto = new CreateActionChainDto
        {
            Shortcut = "test1",
            Title = "Test Chain",
            Description = "A test chain",
            Category = "Psychiatrie",
            CreatedById = _doctor.Id,
            IsGlobal = true,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.AddDiagnosis,
                    SortOrder = 0,
                    Configuration = """{"icd10Code":"F32.1","certainty":"G"}"""
                },
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.AddBillingCode,
                    SortOrder = 1,
                    Configuration = """{"gopCode":"21220","quantity":1}"""
                }
            ]
        };

        var result = await _sut.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("test1", result.Shortcut);
        Assert.Equal("Test Chain", result.Title);
        Assert.Equal(2, result.StepCount);
        Assert.Equal(2, result.Steps.Count);
        Assert.Equal(ActionStepType.AddDiagnosis, result.Steps[0].StepType);
        Assert.Equal(ActionStepType.AddBillingCode, result.Steps[1].StepType);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsChain()
    {
        var chain = await CreateTestChain("getbyid");
        var result = await _sut.GetByIdAsync(chain.Id);

        Assert.NotNull(result);
        Assert.Equal("getbyid", result.Shortcut);
    }

    [Fact]
    public async Task GetByShortcutAsync_ReturnsActiveChain()
    {
        await CreateTestChain("shortcut1");
        var result = await _sut.GetByShortcutAsync("shortcut1");

        Assert.NotNull(result);
        Assert.Equal("shortcut1", result.Shortcut);
    }

    [Fact]
    public async Task GetByShortcutAsync_ReturnsNull_WhenInactive()
    {
        var chain = await CreateTestChain("inactive1");

        // Deactivate
        var update = new UpdateActionChainDto
        {
            Id = chain.Id,
            Shortcut = chain.Shortcut,
            Title = chain.Title,
            Category = chain.Category,
            CreatedById = _doctor.Id,
            IsActive = false,
            Steps = []
        };
        await _sut.UpdateAsync(update);

        var result = await _sut.GetByShortcutAsync("inactive1");
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_FindsByShortcut()
    {
        await CreateTestChain("depress");
        var results = await _sut.SearchAsync("#dep");

        Assert.True(results.Count >= 1);
        Assert.Contains(results, r => r.Shortcut == "depress");
    }

    [Fact]
    public async Task SearchAsync_FindsByTitle()
    {
        await CreateTestChain("xyz123", title: "Epilepsie Kontrolle");
        var results = await _sut.SearchAsync("Epilepsie");

        Assert.True(results.Count >= 1);
        Assert.Contains(results, r => r.Title == "Epilepsie Kontrolle");
    }

    [Fact]
    public async Task GetAllAsync_FiltersByCategory()
    {
        await CreateTestChain("cat1", category: "Psychiatrie");
        await CreateTestChain("cat2", category: "Neurologie");

        var psych = await _sut.GetAllAsync("Psychiatrie");
        var neuro = await _sut.GetAllAsync("Neurologie");

        Assert.Contains(psych, r => r.Shortcut == "cat1");
        Assert.DoesNotContain(psych, r => r.Shortcut == "cat2");
        Assert.Contains(neuro, r => r.Shortcut == "cat2");
    }

    [Fact]
    public async Task UpdateAsync_ReplacesSteps()
    {
        var chain = await CreateTestChain("update1");
        Assert.Single(chain.Steps);

        var update = new UpdateActionChainDto
        {
            Id = chain.Id,
            Shortcut = "update1",
            Title = "Updated",
            Category = "Allgemein",
            CreatedById = _doctor.Id,
            IsActive = true,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.SetDuration,
                    SortOrder = 0,
                    Configuration = """{"durationMinutes":30}"""
                },
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.CreateRecall,
                    SortOrder = 1,
                    Configuration = """{"reason":"Kontrolle","daysFromNow":90}"""
                }
            ]
        };

        var result = await _sut.UpdateAsync(update);

        Assert.Equal("Updated", result.Title);
        Assert.Equal(2, result.StepCount);
        Assert.Equal(ActionStepType.SetDuration, result.Steps[0].StepType);
        Assert.Equal(ActionStepType.CreateRecall, result.Steps[1].StepType);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletes()
    {
        var chain = await CreateTestChain("delete1");
        await _sut.DeleteAsync(chain.Id);

        var result = await _sut.GetByIdAsync(chain.Id);
        Assert.Null(result); // Soft-deleted, not visible
    }

    private async Task<ActionChainDto> CreateTestChain(
        string shortcut, string title = "Test", string category = "Test")
    {
        return await _sut.CreateAsync(new CreateActionChainDto
        {
            Shortcut = shortcut,
            Title = title,
            Category = category,
            CreatedById = _doctor.Id,
            IsGlobal = true,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.AddBillingCode,
                    SortOrder = 0,
                    Configuration = """{"gopCode":"21220","quantity":1}"""
                }
            ]
        });
    }

    public void Dispose() => _factory.Dispose();
}
