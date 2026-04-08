using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IPsychometricTestService
{
    Task<PsychometricTestDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<PsychometricTestListItemDto>> GetByPatientAsync(Guid patientId, PsychometricTestType? type = null, CancellationToken ct = default);
    Task<IReadOnlyList<ScoreHistoryPointDto>> GetScoreHistoryAsync(Guid patientId, PsychometricTestType type, CancellationToken ct = default);
    Task<PsychometricTestDto> CreateAsync(CreatePsychometricTestDto dto, CancellationToken ct = default);
    Task<PsychometricTestDto> SubmitResponsesAsync(SubmitTestResponsesDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
