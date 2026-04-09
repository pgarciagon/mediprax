using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WeekScheduleDto> GetWeekScheduleAsync(DateOnly weekStart, Guid? doctorId, CancellationToken ct = default);
    Task<IReadOnlyList<WaitingRoomEntryDto>> GetWaitingRoomAsync(DateOnly date, CancellationToken ct = default);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, AppointmentStatus status, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    // Recurring appointment series
    Task<AppointmentSeriesDto> CreateSeriesAsync(CreateAppointmentSeriesDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<AppointmentSeriesDto>> GetSeriesAsync(Guid? patientId = null, CancellationToken ct = default);
    Task DeleteSeriesAsync(Guid seriesId, bool deleteFutureAppointments = true, CancellationToken ct = default);

    // Waitlist
    Task<WaitlistEntryDto> AddToWaitlistAsync(CreateWaitlistEntryDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<WaitlistEntryDto>> GetWaitlistAsync(WaitlistStatus? status = null, Guid? therapistId = null, CancellationToken ct = default);
    Task UpdateWaitlistEntryAsync(Guid id, UpdateWaitlistEntryDto dto, CancellationToken ct = default);
    Task UpdateWaitlistStatusAsync(Guid id, WaitlistStatus status, CancellationToken ct = default);
    Task<WaitlistStatisticsDto> GetWaitlistStatisticsAsync(CancellationToken ct = default);
}
