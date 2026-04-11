using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class EncounterSectionServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly EncounterSectionService _sut;
    private readonly User _doctor;
    private readonly Patient _patient;
    private readonly Encounter _encounter;

    public EncounterSectionServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new EncounterSectionService(_factory.Context);

        _doctor = new User
        {
            FirstName = "Dr. Test", LastName = "Arzt",
            Email = $"test-{Guid.NewGuid():N}@test.de", PasswordHash = "hash",
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

        _encounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Notes = null
        };
        _factory.Context.Set<Encounter>().Add(_encounter);
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_CreatesSection()
    {
        var result = await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Anamnese,
            Content = "Patient berichtet über Kopfschmerzen seit 3 Wochen.",
            AuthorId = _doctor.Id
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(EncounterSectionType.Anamnese, result.SectionType);
        Assert.Equal("Patient berichtet über Kopfschmerzen seit 3 Wochen.", result.Content);
        Assert.Equal(0, result.SortOrder);
    }

    [Fact]
    public async Task GetByEncounterAsync_ReturnsSectionsInOrder()
    {
        await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Therapie,
            Content = "Ibuprofen 400mg bei Bedarf"
        });
        await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Anamnese,
            Content = "Kopfschmerzen"
        });

        var sections = await _sut.GetByEncounterAsync(_encounter.Id);

        Assert.Equal(2, sections.Count);
        Assert.Equal(EncounterSectionType.Therapie, sections[0].SectionType);
        Assert.Equal(EncounterSectionType.Anamnese, sections[1].SectionType);
        Assert.True(sections[0].SortOrder < sections[1].SortOrder);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesSectionContent()
    {
        var created = await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Befund,
            Content = "Unauffällig"
        });

        var updated = await _sut.UpdateAsync(new UpdateEncounterSectionDto
        {
            Id = created.Id,
            Content = "Leichter Druckschmerz temporal links"
        });

        Assert.Equal("Leichter Druckschmerz temporal links", updated.Content);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesSection()
    {
        var created = await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Procedere,
            Content = "Wiedervorstellung in 2 Wochen"
        });

        await _sut.DeleteAsync(created.Id);

        var sections = await _sut.GetByEncounterAsync(_encounter.Id);
        Assert.Empty(sections);
    }

    [Fact]
    public async Task GetCombinedTextAsync_ConcatenatesInOrder()
    {
        await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Anamnese,
            Content = "Kopfschmerzen seit 3 Wochen"
        });
        await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Befund,
            Content = "Druckschmerz temporal"
        });

        var combined = await _sut.GetCombinedTextAsync(_encounter.Id);

        Assert.Contains("Anamnese:", combined);
        Assert.Contains("Kopfschmerzen seit 3 Wochen", combined);
        Assert.Contains("Befund:", combined);
        Assert.Contains("Druckschmerz temporal", combined);
        Assert.True(combined.IndexOf("Anamnese:") < combined.IndexOf("Befund:"));
    }

    [Fact]
    public async Task MigrateNotesToSectionsAsync_ParsesStructuredNotes()
    {
        _encounter.Notes = "Anamnese:\nKopfschmerzen\n\nBefund:\nDruckschmerz\n\nTherapie:\nIbuprofen";
        _factory.Context.SaveChanges();

        await _sut.MigrateNotesToSectionsAsync(_encounter.Id, _doctor.Id);

        var sections = await _sut.GetByEncounterAsync(_encounter.Id);
        Assert.Equal(3, sections.Count);
        Assert.Contains(sections, s => s.SectionType == EncounterSectionType.Anamnese && s.Content == "Kopfschmerzen");
        Assert.Contains(sections, s => s.SectionType == EncounterSectionType.Befund && s.Content == "Druckschmerz");
        Assert.Contains(sections, s => s.SectionType == EncounterSectionType.Therapie && s.Content == "Ibuprofen");

        // Notes should be cleared
        await _factory.Context.Entry(_encounter).ReloadAsync();
        Assert.Null(_encounter.Notes);
    }

    [Fact]
    public async Task MigrateNotesToSectionsAsync_UnstructuredNotes_GoesToSonstiges()
    {
        _encounter.Notes = "Patient hat über diverse Beschwerden berichtet. Allgemeinzustand gut.";
        _factory.Context.SaveChanges();

        await _sut.MigrateNotesToSectionsAsync(_encounter.Id);

        var sections = await _sut.GetByEncounterAsync(_encounter.Id);
        Assert.Single(sections);
        Assert.Equal(EncounterSectionType.Sonstiges, sections[0].SectionType);
        Assert.Contains("diverse Beschwerden", sections[0].Content);
    }

    [Fact]
    public async Task SaveAllAsync_ReplacesExistingSections()
    {
        await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Anamnese,
            Content = "Old content"
        });

        await _sut.SaveAllAsync(_encounter.Id, new List<CreateEncounterSectionDto>
        {
            new() { SectionType = EncounterSectionType.Anamnese, Content = "New anamnese" },
            new() { SectionType = EncounterSectionType.Befund, Content = "New befund" }
        });

        var sections = await _sut.GetByEncounterAsync(_encounter.Id);
        Assert.Equal(2, sections.Count);
        Assert.Equal("New anamnese", sections.First(s => s.SectionType == EncounterSectionType.Anamnese).Content);
    }

    [Fact]
    public async Task EncounterService_GetById_IncludesSections()
    {
        // Add sections to the encounter
        await _sut.SaveAllAsync(_encounter.Id, new List<CreateEncounterSectionDto>
        {
            new() { SectionType = EncounterSectionType.Anamnese, Content = "Kopfschmerzen", AuthorId = _doctor.Id },
            new() { SectionType = EncounterSectionType.Befund, Content = "Druckschmerz", AuthorId = _doctor.Id }
        });

        // Now use EncounterService to get the encounter
        var encounterService = new EncounterService(_factory.Context);
        var encounter = await encounterService.GetByIdAsync(_encounter.Id);

        Assert.NotNull(encounter);
        Assert.Equal(2, encounter!.Sections.Count);
        Assert.Contains(encounter.Sections, s => s.SectionType == EncounterSectionType.Anamnese);
        Assert.Contains(encounter.Sections, s => s.SectionType == EncounterSectionType.Befund);
    }

    [Fact]
    public async Task BackwardCompat_EncounterWithNotes_NoSections_StillWorks()
    {
        // Create encounter with old-style Notes, no sections
        var legacyEncounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Notes = "Old style unstructured notes here"
        };
        _factory.Context.Set<Encounter>().Add(legacyEncounter);
        await _factory.Context.SaveChangesAsync();

        var encounterService = new EncounterService(_factory.Context);
        var result = await encounterService.GetByIdAsync(legacyEncounter.Id);

        Assert.NotNull(result);
        Assert.Equal("Old style unstructured notes here", result!.Notes);
        Assert.Empty(result.Sections);
    }

    [Fact]
    public async Task SearchService_FindsSectionContent()
    {
        await _sut.SaveAllAsync(_encounter.Id, new List<CreateEncounterSectionDto>
        {
            new() { SectionType = EncounterSectionType.Befund, Content = "Tremor der rechten Hand" }
        });

        var searchService = new SearchService(_factory.Context);
        var results = await searchService.SearchAsync("Tremor");

        Assert.Contains(results.Results, r => r.Category == "Dokumentation" && r.Subtitle.Contains("Befund"));
    }

    [Fact]
    public async Task SearchService_FiltersBySectionType()
    {
        await _sut.SaveAllAsync(_encounter.Id, new List<CreateEncounterSectionDto>
        {
            new() { SectionType = EncounterSectionType.Anamnese, Content = "Tremor seit 3 Monaten" },
            new() { SectionType = EncounterSectionType.Befund, Content = "Tremor rechte Hand" }
        });

        var searchService = new SearchService(_factory.Context);

        // Search with section filter — should only find in Befund
        var results = await searchService.SearchAsync("Tremor in:befund");
        var docResults = results.Results.Where(r => r.Category == "Dokumentation").ToList();

        Assert.All(docResults, r => Assert.Contains("Befund", r.Subtitle));
    }

    [Fact]
    public async Task MigrateNotes_DoesNothing_WhenSectionsAlreadyExist()
    {
        // First add sections
        await _sut.CreateAsync(_encounter.Id, new CreateEncounterSectionDto
        {
            SectionType = EncounterSectionType.Anamnese,
            Content = "Existing section"
        });

        // Set Notes on the encounter
        _encounter.Notes = "Some old notes";
        await _factory.Context.SaveChangesAsync();

        // Migration should skip since sections already exist
        await _sut.MigrateNotesToSectionsAsync(_encounter.Id);

        var sections = await _sut.GetByEncounterAsync(_encounter.Id);
        Assert.Single(sections);
        Assert.Equal("Existing section", sections[0].Content);

        // Notes should NOT be cleared
        await _factory.Context.Entry(_encounter).ReloadAsync();
        Assert.Equal("Some old notes", _encounter.Notes);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
