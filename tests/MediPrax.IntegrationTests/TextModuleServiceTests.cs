using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class TextModuleServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TextModuleService _sut;
    private readonly User _doctor;
    private readonly Patient _patient;

    public TextModuleServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);

        _doctor = new User
        {
            FirstName = "Dr. Test", LastName = "Arzt",
            Email = $"tm-test-{Guid.NewGuid():N}@test.de", PasswordHash = "hash",
            Role = UserRole.Arzt, IsActive = true, AcademicTitle = "Dr. med."
        };
        _factory.Context.Users.Add(_doctor);

        _patient = new Patient
        {
            FirstName = "Max", LastName = "Mustermann",
            DateOfBirth = new DateOnly(1985, 3, 15),
            Gender = "M",
            InsuranceType = InsuranceType.GKV
        };
        _factory.Context.Patients.Add(_patient);
        _factory.Context.SaveChanges();

        // Create service with real dependencies
        var diagService = new PatientDiagnosisService(_factory.Context);
        var medService = new MedicationService(_factory.Context);
        var psychService = new PsychopathFindingService(_factory.Context);
        var neuroService = new NeurologicalExamService(_factory.Context);
        _sut = new TextModuleService(_factory.Context, diagService, medService, psychService, neuroService);
    }

    [Fact]
    public async Task CreateAsync_CreatesModuleWithTargetSection()
    {
        var dto = new CreateTextModuleDto
        {
            Shortcut = "tst1",
            Title = "Test Module",
            Content = "Test content",
            Category = "Test",
            IsGlobal = true,
            CreatedById = _doctor.Id,
            TargetSection = EncounterSectionType.Befund
        };

        var result = await _sut.CreateAsync(dto);

        Assert.Equal("tst1", result.Shortcut);
        Assert.Equal(EncounterSectionType.Befund, result.TargetSection);
        Assert.Equal("Befund", result.TargetSectionDisplay);
    }

    [Fact]
    public async Task SearchAsync_FindsByShortcutContains()
    {
        await CreateTestModule("searchme", "Search Module", category: "Test");

        var results = await _sut.SearchAsync("arch");

        Assert.Contains(results, r => r.Shortcut == "searchme");
    }

    [Fact]
    public async Task SearchAsync_FindsByTitleContains()
    {
        await CreateTestModule("xyz1", "Normaler Befund", category: "Test");

        var results = await _sut.SearchAsync("Normal");

        Assert.Contains(results, r => r.Title == "Normaler Befund");
    }

    [Fact]
    public async Task SearchAsync_FiltersByTargetSection()
    {
        await CreateTestModule("sect1", "Befund Only", section: EncounterSectionType.Befund);
        await CreateTestModule("sect2", "Therapie Only", section: EncounterSectionType.Therapie);
        await CreateTestModule("sect3", "All Sections"); // null target

        var befundResults = await _sut.SearchAsync("sect", EncounterSectionType.Befund);

        // Should include sect1 (Befund) and sect3 (null = all), but NOT sect2 (Therapie)
        Assert.Contains(befundResults, r => r.Shortcut == "sect1");
        Assert.Contains(befundResults, r => r.Shortcut == "sect3");
        Assert.DoesNotContain(befundResults, r => r.Shortcut == "sect2");
    }

    [Fact]
    public async Task SearchAsync_OrdersByUsageCountDescending()
    {
        var m1 = await CreateTestModule("ord1", "Low Usage");
        var m2 = await CreateTestModule("ord2", "High Usage");

        // Bump usage of m2
        await _sut.IncrementUsageAsync(m2.Id);
        await _sut.IncrementUsageAsync(m2.Id);
        await _sut.IncrementUsageAsync(m2.Id);

        var results = await _sut.SearchAsync("ord");

        var idx1 = results.ToList().FindIndex(r => r.Shortcut == "ord1");
        var idx2 = results.ToList().FindIndex(r => r.Shortcut == "ord2");
        Assert.True(idx2 < idx1, "Higher usage should come first");
    }

    [Fact]
    public async Task IncrementUsageAsync_BumpsCountAndSetsLastUsedAt()
    {
        var module = await CreateTestModule("inc1", "Increment Test");
        Assert.Equal(0, module.UsageCount);
        Assert.Null(module.LastUsedAt);

        await _sut.IncrementUsageAsync(module.Id);

        var all = await _sut.GetAllAsync();
        var updated = all.First(m => m.Id == module.Id);
        Assert.Equal(1, updated.UsageCount);
        Assert.NotNull(updated.LastUsedAt);
    }

    [Fact]
    public async Task ResolveVariablesAsync_ResolvesPatientVariables()
    {
        var content = "Patient: {Patient.Name}, geb. {Patient.Geburtsdatum}, {Patient.Alter} Jahre, {Patient.Geschlecht}";

        var result = await _sut.ResolveVariablesAsync(content, _patient.Id);

        Assert.Contains("Max Mustermann", result);
        Assert.Contains("15.03.1985", result);
        Assert.Contains("Herr", result);
        Assert.DoesNotContain("{Patient.", result);
    }

    [Fact]
    public async Task ResolveVariablesAsync_ResolvesDoctorVariables()
    {
        var content = "{Arzt.Titel} {Arzt.Name}";

        var result = await _sut.ResolveVariablesAsync(content, _patient.Id, doctorId: _doctor.Id);

        Assert.Contains("Dr. med.", result);
        Assert.Contains("Dr. Test Arzt", result);
    }

    [Fact]
    public async Task ResolveVariablesAsync_ResolvesDateVariables()
    {
        var content = "Datum: {Datum}, Lang: {Datum.Lang}";

        var result = await _sut.ResolveVariablesAsync(content, _patient.Id);

        Assert.Contains(DateTime.Today.ToString("dd.MM.yyyy"), result);
        Assert.DoesNotContain("{Datum}", result);
    }

    [Fact]
    public async Task ResolveVariablesAsync_SkipsUnknownVariables()
    {
        var content = "Text with {Unknown.Variable} stays unchanged";

        var result = await _sut.ResolveVariablesAsync(content, _patient.Id);

        Assert.Contains("{Unknown.Variable}", result);
    }

    [Fact]
    public async Task ExportAsync_ReturnsValidJson()
    {
        await CreateTestModule("exp1", "Export Test", category: "TestExport");

        var data = await _sut.ExportAsync("TestExport");

        Assert.NotEmpty(data);
        var json = System.Text.Encoding.UTF8.GetString(data);
        Assert.Contains("\"version\"", json);
        Assert.Contains("\"exp1\"", json);
        Assert.Contains("Export Test", json);
    }

    [Fact]
    public async Task ImportAsync_CreatesNewModules()
    {
        var json = """
        {
            "version": "1.0",
            "exportedAt": "2026-04-12",
            "modules": [
                {
                    "shortcut": "imp1",
                    "title": "Imported Module",
                    "content": "Imported content",
                    "category": "Import",
                    "isGlobal": true
                }
            ]
        }
        """;

        var count = await _sut.ImportAsync(System.Text.Encoding.UTF8.GetBytes(json), _doctor.Id);

        Assert.Equal(1, count);
        var results = await _sut.SearchAsync("imp1");
        Assert.Single(results);
        Assert.Equal("Imported Module", results[0].Title);
    }

    [Fact]
    public async Task ImportAsync_SkipsDuplicateShortcuts()
    {
        await CreateTestModule("dup1", "Existing Module");

        var json = """
        {
            "version": "1.0",
            "modules": [
                {
                    "shortcut": "dup1",
                    "title": "Duplicate",
                    "content": "Should be skipped",
                    "category": "Test",
                    "isGlobal": true
                }
            ]
        }
        """;

        var count = await _sut.ImportAsync(System.Text.Encoding.UTF8.GetBytes(json), _doctor.Id);

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetAllAsync_SortsByUsage()
    {
        var m1 = await CreateTestModule("sort1", "Low", category: "SortTest");
        var m2 = await CreateTestModule("sort2", "High", category: "SortTest");

        await _sut.IncrementUsageAsync(m2.Id);
        await _sut.IncrementUsageAsync(m2.Id);

        var results = await _sut.GetAllAsync("SortTest", "usage");
        var list = results.ToList();

        if (list.Count >= 2)
        {
            var idx1 = list.FindIndex(r => r.Shortcut == "sort1");
            var idx2 = list.FindIndex(r => r.Shortcut == "sort2");
            Assert.True(idx2 < idx1, "Higher usage should come first with usage sort");
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTargetSection()
    {
        var module = await CreateTestModule("upd1", "Update Test");
        Assert.Null(module.TargetSection);

        var updateDto = new CreateTextModuleDto
        {
            Shortcut = "upd1",
            Title = "Updated",
            Content = "Updated content",
            Category = "Test",
            IsGlobal = true,
            TargetSection = EncounterSectionType.Therapie
        };

        var updated = await _sut.UpdateAsync(module.Id, updateDto);

        Assert.Equal(EncounterSectionType.Therapie, updated.TargetSection);
    }

    private async Task<TextModuleDto> CreateTestModule(
        string shortcut, string title,
        string category = "Test",
        EncounterSectionType? section = null)
    {
        return await _sut.CreateAsync(new CreateTextModuleDto
        {
            Shortcut = shortcut,
            Title = title,
            Content = $"Content for {title}",
            Category = category,
            IsGlobal = true,
            CreatedById = _doctor.Id,
            TargetSection = section
        });
    }

    public void Dispose() => _factory.Dispose();
}
