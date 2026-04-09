using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface ISuicidalityAssessmentService
{
    Task<IReadOnlyList<SuicidalityAssessmentDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<SuicidalityAssessmentDto?> GetLatestByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<SuicidalityAssessmentDto> CreateAsync(CreateSuicidalityAssessmentDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<SuicidalityRiskLevel?> GetCurrentRiskLevelAsync(Guid patientId, CancellationToken ct = default);
}
