using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface ITextModuleService
{
    Task<IReadOnlyList<TextModuleDto>> GetAllAsync(string? category = null, CancellationToken ct = default);
    Task<IReadOnlyList<TextModuleDto>> SearchByShortcutAsync(string shortcut, CancellationToken ct = default);
    Task<TextModuleDto> CreateAsync(CreateTextModuleDto dto, CancellationToken ct = default);
    Task<TextModuleDto> UpdateAsync(Guid id, CreateTextModuleDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<string> ResolveVariablesAsync(string content, Guid patientId, CancellationToken ct = default);
}
