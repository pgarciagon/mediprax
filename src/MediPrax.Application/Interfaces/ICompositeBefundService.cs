using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

/// <summary>
/// Composes the full Befund from structured findings (AMDP, Neuro) and free-text sections.
/// Read-time composition — each data source stays in its own entity.
/// </summary>
public interface ICompositeBefundService
{
    /// <summary>
    /// Composes the full Befund text from structured findings + free-text section.
    /// Used by the EncounterFormular Befund tab and ArztbriefService.
    /// </summary>
    Task<CompositeBefundDto> GetAsync(Guid encounterId, CancellationToken ct = default);
}
