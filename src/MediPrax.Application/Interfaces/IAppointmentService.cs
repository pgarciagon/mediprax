using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WeekScheduleDto> GetWeekScheduleAsync(DateOnly weekStart, Guid? doctorId, CancellationToken ct = default);
    Task<IReadOnlyList<WaitingRoomEntryDto>> GetWaitingRoomAsync(DateOnly date, CancellationToken ct = default);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, AppointmentStatus status, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
