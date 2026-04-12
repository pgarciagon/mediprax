using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface ITextModuleService
{
    Task<IReadOnlyList<TextModuleDto>> GetAllAsync(string? category = null, string? sortBy = null, CancellationToken ct = default);
    Task<TextModuleDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TextModuleDto>> SearchByShortcutAsync(string shortcut, CancellationToken ct = default);
    Task<IReadOnlyList<TextModuleDto>> SearchAsync(string term, EncounterSectionType? section = null, CancellationToken ct = default);
    Task<TextModuleDto> CreateAsync(CreateTextModuleDto dto, CancellationToken ct = default);
    Task<TextModuleDto> UpdateAsync(Guid id, CreateTextModuleDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<string> ResolveVariablesAsync(string content, Guid patientId, Guid? encounterId = null, Guid? doctorId = null, CancellationToken ct = default);
    Task IncrementUsageAsync(Guid id, CancellationToken ct = default);
    Task<byte[]> ExportAsync(string? category = null, CancellationToken ct = default);
    Task<int> ImportAsync(byte[] jsonData, Guid createdById, CancellationToken ct = default);
}
