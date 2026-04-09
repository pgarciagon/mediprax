using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface ISlotSuggestionService
{
    Task<IReadOnlyList<FreeSlotDto>> SuggestNextSlotAsync(Guid doctorId, int durationMinutes, DayOfWeek? preferredDay = null, TimeOnly? preferredTime = null, CancellationToken ct = default);
    Task<IReadOnlyList<FreeSlotDto>> SuggestNextTherapySessionAsync(Guid therapyCaseId, CancellationToken ct = default);
}
