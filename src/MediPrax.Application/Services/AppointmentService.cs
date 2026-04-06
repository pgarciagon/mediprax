using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class AppointmentService(DbContext context) : IAppointmentService
{
    private DbSet<Appointment> Appointments => context.Set<Appointment>();
    private DbSet<User> Users => context.Set<User>();

    public async Task<AppointmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.Id == id)
            .Select(a => MapToDto(a))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<WeekScheduleDto> GetWeekScheduleAsync(DateOnly weekStart, Guid? doctorId, CancellationToken ct = default)
    {
        var start = weekStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = weekStart.AddDays(7).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var query = Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.StartTime >= start && a.StartTime < end);

        if (doctorId.HasValue)
            query = query.Where(a => a.DoctorId == doctorId.Value);

        var appointments = await query
            .OrderBy(a => a.StartTime)
            .ToListAsync(ct);

        var doctors = await Users
            .Where(u => u.Role == UserRole.Arzt && u.IsActive)
            .OrderBy(u => u.LastName)
            .ToListAsync(ct);

        var days = new List<DayScheduleDto>();
        for (int i = 0; i < 7; i++)
        {
            var date = weekStart.AddDays(i);
            var dayStart = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var dayEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var dayAppointments = appointments
                .Where(a => a.StartTime >= dayStart && a.StartTime < dayEnd)
                .ToList();

            if (doctorId.HasValue)
            {
                var doctor = doctors.FirstOrDefault(d => d.Id == doctorId.Value);
                days.Add(new DayScheduleDto
                {
                    Date = date,
                    DoctorId = doctorId.Value,
                    DoctorName = doctor?.FullName ?? "—",
                    Appointments = dayAppointments.Select(a => MapToDto(a)).ToList()
                });
            }
            else
            {
                // When no doctor filter, group per doctor but flatten into one day
                days.Add(new DayScheduleDto
                {
                    Date = date,
                    Appointments = dayAppointments.Select(a => MapToDto(a)).ToList()
                });
            }
        }

        return new WeekScheduleDto
        {
            WeekStart = weekStart,
            Days = days
        };
    }

    public async Task<IReadOnlyList<WaitingRoomEntryDto>> GetWaitingRoomAsync(DateOnly date, CancellationToken ct = default)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        return await Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.StartTime >= start && a.StartTime < end)
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.NoShow)
            .OrderBy(a => a.Status == AppointmentStatus.InProgress ? 0 :
                          a.Status == AppointmentStatus.CheckedIn ? 1 :
                          a.Status == AppointmentStatus.Scheduled ? 2 : 3)
            .ThenBy(a => a.StartTime)
            .Select(a => new WaitingRoomEntryDto
            {
                AppointmentId = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.LastName + ", " + a.Patient.FirstName,
                StartTime = a.StartTime,
                DurationMinutes = a.DurationMinutes,
                DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync(ct);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default)
    {
        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc),
            DurationMinutes = dto.DurationMinutes,
            Notes = dto.Notes
        };

        Appointments.Add(appointment);
        await context.SaveChangesAsync(ct);

        // Reload with includes
        var created = await Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstAsync(a => a.Id == appointment.Id, ct);

        return MapToDto(created);
    }

    public async Task UpdateStatusAsync(Guid id, AppointmentStatus status, CancellationToken ct = default)
    {
        var appointment = await Appointments.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Appointment {id} not found");

        appointment.Status = status;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var appointment = await Appointments.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Appointment {id} not found");

        Appointments.Remove(appointment);
        await context.SaveChangesAsync(ct);
    }

    private static AppointmentDto MapToDto(Appointment a) => new()
    {
        Id = a.Id,
        PatientId = a.PatientId,
        PatientName = a.Patient?.LastName + ", " + a.Patient?.FirstName,
        DoctorId = a.DoctorId,
        DoctorName = a.Doctor?.FirstName + " " + a.Doctor?.LastName,
        StartTime = a.StartTime,
        DurationMinutes = a.DurationMinutes,
        Status = a.Status,
        Notes = a.Notes
    };
}
