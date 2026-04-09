using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IAvailabilityService
{
    // Schedule templates
    Task<IReadOnlyList<ScheduleBlockDto>> GetScheduleAsync(Guid doctorId, CancellationToken ct = default);
    Task<ScheduleBlockDto> CreateScheduleBlockAsync(CreateScheduleBlockDto dto, CancellationToken ct = default);
    Task DeleteScheduleBlockAsync(Guid id, CancellationToken ct = default);

    // Absences
    Task<IReadOnlyList<AbsenceDto>> GetAbsencesAsync(Guid? doctorId, DateOnly from, DateOnly to, CancellationToken ct = default);
    Task<AbsenceDto> CreateAbsenceAsync(CreateAbsenceDto dto, CancellationToken ct = default);
    Task DeleteAbsenceAsync(Guid id, CancellationToken ct = default);

    // Availability calculation
    Task<IReadOnlyList<FreeSlotDto>> GetFreeSlotsAsync(Guid doctorId, DateOnly date, int durationMinutes, CancellationToken ct = default);
    Task<IReadOnlyList<FreeSlotDto>> FindNextFreeSlotAsync(Guid doctorId, int durationMinutes, DateOnly searchFrom, int maxResults = 5, DayOfWeek? preferredDay = null, TimeOnly? preferredTime = null, CancellationToken ct = default);
    Task<AvailabilityCheckResult> CheckAvailabilityAsync(Guid doctorId, DateTime start, int durationMinutes, CancellationToken ct = default);
}
