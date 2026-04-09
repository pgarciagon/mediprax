using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IDrugInteractionService
{
    Task<IReadOnlyList<InteractionWarningDto>> CheckPatientInteractionsAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<InteractionWarningDto>> CheckNewMedicationAsync(Guid patientId, string newWirkstoff, CancellationToken ct = default);
}
