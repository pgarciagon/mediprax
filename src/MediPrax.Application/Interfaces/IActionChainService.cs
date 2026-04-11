using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IActionChainService
{
    Task<IReadOnlyList<ActionChainDto>> GetAllAsync(string? category = null, CancellationToken ct = default);
    Task<ActionChainDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ActionChainDto?> GetByShortcutAsync(string shortcut, CancellationToken ct = default);
    Task<IReadOnlyList<ActionChainDto>> SearchAsync(string term, CancellationToken ct = default);
    Task<ActionChainDto> CreateAsync(CreateActionChainDto dto, CancellationToken ct = default);
    Task<ActionChainDto> UpdateAsync(UpdateActionChainDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
