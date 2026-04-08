using MediPrax.Application.Catalogs;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class PsychometricTestService(DbContext context) : IPsychometricTestService
{
    private DbSet<PsychometricTest> Tests => context.Set<PsychometricTest>();

    public async Task<PsychometricTestDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var t = await Tests
            .Include(x => x.Patient)
            .Include(x => x.AdministeredBy)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return t is null ? null : MapToDto(t);
    }

    public async Task<IReadOnlyList<PsychometricTestListItemDto>> GetByPatientAsync(
        Guid patientId, PsychometricTestType? type = null, CancellationToken ct = default)
    {
        var query = Tests.Where(x => x.PatientId == patientId);

        if (type.HasValue)
            query = query.Where(x => x.TestType == type.Value);

        var entities = await query
            .OrderByDescending(x => x.TestDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x =>
        {
            var def = PsychometricTestCatalog.GetDefinition(x.TestType);
            return new PsychometricTestListItemDto
            {
                Id = x.Id,
                TestType = x.TestType,
                TestTypeName = def.Abbreviation,
                TestDate = x.TestDate,
                TotalScore = x.TotalScore,
                MaxScore = def.MaxScore,
                Interpretation = x.Interpretation,
                Status = x.Status
            };
        }).ToList();
    }

    public async Task<IReadOnlyList<ScoreHistoryPointDto>> GetScoreHistoryAsync(
        Guid patientId, PsychometricTestType type, CancellationToken ct = default)
    {
        return await Tests
            .Where(x => x.PatientId == patientId && x.TestType == type && x.Status == TestStatus.Completed)
            .OrderBy(x => x.TestDate)
            .Select(x => new ScoreHistoryPointDto
            {
                Date = x.TestDate,
                Score = x.TotalScore,
                Interpretation = x.Interpretation
            })
            .ToListAsync(ct);
    }

    public async Task<PsychometricTestDto> CreateAsync(CreatePsychometricTestDto dto, CancellationToken ct = default)
    {
        var test = new PsychometricTest
        {
            PatientId = dto.PatientId,
            EncounterId = dto.EncounterId,
            AdministeredById = dto.AdministeredById,
            TestType = dto.TestType,
            TestDate = dto.TestDate,
            Status = TestStatus.InProgress
        };

        Tests.Add(test);
        await context.SaveChangesAsync(ct);

        var created = await Tests
            .Include(x => x.Patient)
            .Include(x => x.AdministeredBy)
            .FirstAsync(x => x.Id == test.Id, ct);

        return MapToDto(created);
    }

    public async Task<PsychometricTestDto> SubmitResponsesAsync(SubmitTestResponsesDto dto, CancellationToken ct = default)
    {
        var test = await Tests
            .Include(x => x.Patient)
            .Include(x => x.AdministeredBy)
            .FirstOrDefaultAsync(x => x.Id == dto.TestId, ct)
            ?? throw new KeyNotFoundException($"PsychometricTest {dto.TestId} not found");

        test.Responses = dto.Responses;
        test.TotalScore = dto.Responses.Sum(r => r.ResponseValue);
        test.Interpretation = PsychometricTestCatalog.CalculateInterpretation(test.TestType, test.TotalScore);
        test.Status = TestStatus.Completed;

        await context.SaveChangesAsync(ct);
        return MapToDto(test);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var test = await Tests.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"PsychometricTest {id} not found");

        Tests.Remove(test); // Triggers soft-delete via SaveChangesAsync override
        await context.SaveChangesAsync(ct);
    }

    private static PsychometricTestDto MapToDto(PsychometricTest t)
    {
        var def = PsychometricTestCatalog.GetDefinition(t.TestType);
        return new PsychometricTestDto
        {
            Id = t.Id,
            PatientId = t.PatientId,
            EncounterId = t.EncounterId,
            TestType = t.TestType,
            TestTypeName = def.Abbreviation,
            TestDate = t.TestDate,
            TotalScore = t.TotalScore,
            MaxScore = def.MaxScore,
            Interpretation = t.Interpretation,
            Notes = t.Notes,
            Status = t.Status,
            Responses = t.Responses,
            AdministeredByName = t.AdministeredBy?.FullName ?? string.Empty
        };
    }
}
