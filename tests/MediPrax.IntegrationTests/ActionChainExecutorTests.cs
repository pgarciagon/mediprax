using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class ActionChainExecutorTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly ActionChainService _chainService;
    private readonly ActionChainExecutor _executor;
    private readonly User _doctor;
    private readonly Patient _patient;
    private readonly Encounter _encounter;

    public ActionChainExecutorTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        var ctx = _factory.Context;

        _chainService = new ActionChainService(ctx);
        var diagnosisService = new PatientDiagnosisService(ctx);
        var billingService = new BillingService(ctx);
        var sectionService = new EncounterSectionService(ctx);
        var recallService = new RecallService(ctx);

        _executor = new ActionChainExecutor(
            ctx, _chainService, diagnosisService, billingService, sectionService, recallService);

        _doctor = new User
        {
            FirstName = "Dr. Exec", LastName = "Test",
            Email = $"exec-{Guid.NewGuid():N}@test.de", PasswordHash = "hash",
            Role = UserRole.Arzt, IsActive = true
        };
        _patient = new Patient
        {
            FirstName = "Test", LastName = "Patient",
            DateOfBirth = new DateOnly(1985, 6, 15)
        };
        ctx.Users.Add(_doctor);
        ctx.Patients.Add(_patient);
        ctx.SaveChanges();

        _encounter = new Encounter
        {
            PatientId = _patient.Id,
            DoctorId = _doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        };
        ctx.Set<Encounter>().Add(_encounter);
        ctx.SaveChanges();
    }

    [Fact]
    public async Task ExecuteAsync_AddDiagnosis_CreatesDiagnosis()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-diag",
            Title = "Diagnosis Test",
            Category = "Test",
            CreatedById = _doctor.Id,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.AddDiagnosis,
                    SortOrder = 0,
                    Configuration = """{"icd10Code":"F32.1","certainty":"G","diagnosisType":"Encounterdiagnose"}"""
                }
            ]
        });

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.True(result.Success);
        Assert.Equal(1, result.StepsExecuted);
        Assert.Contains("F32.1", result.Results[0].Detail!);

        // Verify diagnosis was created
        var diagnoses = await _factory.Context.Set<PatientDiagnosis>()
            .Where(d => d.PatientId == _patient.Id && d.Icd10Code == "F32.1")
            .ToListAsync();
        Assert.NotEmpty(diagnoses);
    }

    [Fact]
    public async Task ExecuteAsync_AddBillingCode_CreatesBillingItem()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-bill",
            Title = "Billing Test",
            Category = "Test",
            CreatedById = _doctor.Id,
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

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.True(result.Success);
        Assert.Equal(1, result.StepsExecuted);

        var items = await _factory.Context.Set<BillingItem>()
            .Where(b => b.EncounterId == _encounter.Id && b.GopCode == "21220")
            .ToListAsync();
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task ExecuteAsync_SetNoteTemplate_CreatesSections()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-tmpl",
            Title = "Template Test",
            Category = "Test",
            CreatedById = _doctor.Id,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.SetNoteTemplate,
                    SortOrder = 0,
                    Configuration = """{"template":"psych"}"""
                }
            ]
        });

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.True(result.Success);

        var sections = await _factory.Context.Set<EncounterSection>()
            .Where(s => s.EncounterId == _encounter.Id)
            .ToListAsync();
        Assert.NotEmpty(sections);
        Assert.Contains(sections, s => s.SectionType == EncounterSectionType.Befund);
    }

    [Fact]
    public async Task ExecuteAsync_SetDuration_UpdatesEncounter()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-dur",
            Title = "Duration Test",
            Category = "Test",
            CreatedById = _doctor.Id,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.SetDuration,
                    SortOrder = 0,
                    Configuration = """{"durationMinutes":50}"""
                }
            ]
        });

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.True(result.Success);

        var enc = await _factory.Context.Set<Encounter>().FindAsync(_encounter.Id);
        Assert.Equal(50, enc!.DurationMinutes);
    }

    [Fact]
    public async Task ExecuteAsync_CreateRecall_CreatesRecall()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-recall",
            Title = "Recall Test",
            Category = "Test",
            CreatedById = _doctor.Id,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.CreateRecall,
                    SortOrder = 0,
                    Configuration = """{"reason":"Kontrolle","daysFromNow":90}"""
                }
            ]
        });

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.True(result.Success);

        var recalls = await _factory.Context.Set<Recall>()
            .Where(r => r.PatientId == _patient.Id)
            .ToListAsync();
        Assert.NotEmpty(recalls);
        Assert.Contains(recalls, r => r.Reason == "Kontrolle");
    }

    [Fact]
    public async Task ExecuteAsync_MultipleSteps_ExecutesAll()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-multi",
            Title = "Multi Test",
            Category = "Test",
            CreatedById = _doctor.Id,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.AddDiagnosis,
                    SortOrder = 0,
                    Configuration = """{"icd10Code":"G40.9","certainty":"G"}"""
                },
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.AddBillingCode,
                    SortOrder = 1,
                    Configuration = """{"gopCode":"16220","quantity":1}"""
                },
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.SetDuration,
                    SortOrder = 2,
                    Configuration = """{"durationMinutes":25}"""
                }
            ]
        });

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.True(result.Success);
        Assert.Equal(3, result.StepsExecuted);
        Assert.Equal(0, result.StepsFailed);
    }

    [Fact]
    public async Task ExecuteAsync_ContinuesOnFailure()
    {
        var chain = await _chainService.CreateAsync(new CreateActionChainDto
        {
            Shortcut = "exec-fail",
            Title = "Fail Test",
            Category = "Test",
            CreatedById = _doctor.Id,
            Steps =
            [
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.SetNoteTemplate,
                    SortOrder = 0,
                    Configuration = """{"template":"nonexistent"}"""
                },
                new CreateActionChainStepDto
                {
                    StepType = ActionStepType.SetDuration,
                    SortOrder = 1,
                    Configuration = """{"durationMinutes":30}"""
                }
            ]
        });

        var result = await _executor.ExecuteAsync(chain.Id, _encounter.Id);

        Assert.False(result.Success); // One step failed
        Assert.Equal(1, result.StepsExecuted); // Duration succeeded
        Assert.Equal(1, result.StepsFailed); // Template failed
        Assert.NotEmpty(result.Warnings);

        // Duration still applied
        var enc = await _factory.Context.Set<Encounter>().FindAsync(_encounter.Id);
        Assert.Equal(30, enc!.DurationMinutes);
    }

    public void Dispose() => _factory.Dispose();
}
