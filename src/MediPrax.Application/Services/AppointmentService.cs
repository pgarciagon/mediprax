using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class AppointmentService(DbContext context) : IAppointmentService
{
    private DbSet<Appointment> Appointments => context.Set<Appointment>();
    private DbSet<AppointmentSeries> Series => context.Set<AppointmentSeries>();
    private DbSet<WaitlistEntry> WaitlistEntries => context.Set<WaitlistEntry>();
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
                DoctorId = a.DoctorId,
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
            AppointmentType = dto.AppointmentType,
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

    public async Task UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken ct = default)
    {
        var appointment = await Appointments.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Appointment {id} not found");

        appointment.PatientId = dto.PatientId;
        appointment.DoctorId = dto.DoctorId;
        appointment.StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);
        appointment.DurationMinutes = dto.DurationMinutes;
        appointment.AppointmentType = dto.AppointmentType;
        appointment.Notes = dto.Notes;
        await context.SaveChangesAsync(ct);
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

    // --- Recurring Appointment Series ---

    public async Task<AppointmentSeriesDto> CreateSeriesAsync(CreateAppointmentSeriesDto dto, CancellationToken ct = default)
    {
        var series = new AppointmentSeries
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            TherapyCaseId = dto.TherapyCaseId,
            RecurrencePattern = dto.RecurrencePattern,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            DurationMinutes = dto.DurationMinutes,
            SeriesStartDate = dto.SeriesStartDate,
            SeriesEndDate = dto.SeriesEndDate,
            MaxOccurrences = dto.MaxOccurrences,
            Notes = dto.Notes
        };

        Series.Add(series);
        await context.SaveChangesAsync(ct);

        // Generate individual appointments
        var appointments = GenerateAppointments(series);
        Appointments.AddRange(appointments);
        await context.SaveChangesAsync(ct);

        var created = await Series
            .Include(s => s.Patient)
            .Include(s => s.Doctor)
            .Include(s => s.Appointments)
            .FirstAsync(s => s.Id == series.Id, ct);

        return MapSeriesToDto(created);
    }

    public async Task<IReadOnlyList<AppointmentSeriesDto>> GetSeriesAsync(Guid? patientId = null, CancellationToken ct = default)
    {
        var query = Series
            .Include(s => s.Patient)
            .Include(s => s.Doctor)
            .Include(s => s.Appointments)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(s => s.PatientId == patientId.Value);

        return await query
            .OrderByDescending(s => s.SeriesStartDate)
            .Select(s => MapSeriesToDto(s))
            .ToListAsync(ct);
    }

    public async Task DeleteSeriesAsync(Guid seriesId, bool deleteFutureAppointments = true, CancellationToken ct = default)
    {
        var series = await Series
            .Include(s => s.Appointments)
            .FirstOrDefaultAsync(s => s.Id == seriesId, ct)
            ?? throw new KeyNotFoundException($"AppointmentSeries {seriesId} not found");

        if (deleteFutureAppointments)
        {
            var futureAppointments = series.Appointments
                .Where(a => a.StartTime > DateTime.UtcNow && a.Status == AppointmentStatus.Scheduled);
            Appointments.RemoveRange(futureAppointments);
        }

        Series.Remove(series);
        await context.SaveChangesAsync(ct);
    }

    private static List<Appointment> GenerateAppointments(AppointmentSeries series)
    {
        var appointments = new List<Appointment>();
        var current = series.SeriesStartDate;
        var count = 0;

        // Advance to the first occurrence on the correct day of week
        while (current.DayOfWeek != series.DayOfWeek)
            current = current.AddDays(1);

        while (true)
        {
            if (series.SeriesEndDate.HasValue && current > series.SeriesEndDate.Value)
                break;
            if (series.MaxOccurrences.HasValue && count >= series.MaxOccurrences.Value)
                break;
            if (!series.SeriesEndDate.HasValue && !series.MaxOccurrences.HasValue && count >= 52)
                break; // Safety: max 1 year of weekly appointments if open-ended

            appointments.Add(new Appointment
            {
                PatientId = series.PatientId,
                DoctorId = series.DoctorId,
                StartTime = current.ToDateTime(series.StartTime, DateTimeKind.Utc),
                DurationMinutes = series.DurationMinutes,
                Notes = series.Notes,
                AppointmentSeriesId = series.Id
            });

            count++;
            current = series.RecurrencePattern switch
            {
                RecurrencePattern.Weekly => current.AddDays(7),
                RecurrencePattern.BiWeekly => current.AddDays(14),
                RecurrencePattern.Monthly => current.AddMonths(1),
                _ => current.AddDays(7)
            };
        }

        return appointments;
    }

    // --- Waitlist ---

    public async Task<WaitlistEntryDto> AddToWaitlistAsync(CreateWaitlistEntryDto dto, CancellationToken ct = default)
    {
        var entry = new WaitlistEntry
        {
            PatientId = dto.PatientId,
            PreferredTherapistId = dto.PreferredTherapistId,
            RequestDate = DateOnly.FromDateTime(DateTime.Today),
            TherapyTypeRequested = dto.TherapyTypeRequested,
            Priority = dto.Priority,
            PreferredDays = dto.PreferredDays,
            PreferredTimeSlot = dto.PreferredTimeSlot,
            Notes = dto.Notes,
            Status = WaitlistStatus.Waiting
        };

        WaitlistEntries.Add(entry);
        await context.SaveChangesAsync(ct);

        var created = await WaitlistEntries
            .Include(w => w.Patient)
            .Include(w => w.PreferredTherapist)
            .FirstAsync(w => w.Id == entry.Id, ct);

        return MapWaitlistToDto(created);
    }

    public async Task<IReadOnlyList<WaitlistEntryDto>> GetWaitlistAsync(WaitlistStatus? status = null, Guid? therapistId = null, CancellationToken ct = default)
    {
        var query = WaitlistEntries
            .Include(w => w.Patient)
            .Include(w => w.PreferredTherapist)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(w => w.Status == status.Value);
        if (therapistId.HasValue)
            query = query.Where(w => w.PreferredTherapistId == therapistId.Value);

        return await query
            .OrderByDescending(w => w.Priority)
            .ThenBy(w => w.RequestDate)
            .Select(w => MapWaitlistToDto(w))
            .ToListAsync(ct);
    }

    public async Task UpdateWaitlistEntryAsync(Guid id, UpdateWaitlistEntryDto dto, CancellationToken ct = default)
    {
        var entry = await WaitlistEntries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"WaitlistEntry {id} not found");

        entry.PreferredTherapistId = dto.PreferredTherapistId;
        entry.TherapyTypeRequested = dto.TherapyTypeRequested;
        entry.Priority = dto.Priority;
        entry.PreferredDays = dto.PreferredDays;
        entry.PreferredTimeSlot = dto.PreferredTimeSlot;
        entry.Notes = dto.Notes;
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateWaitlistStatusAsync(Guid id, WaitlistStatus status, CancellationToken ct = default)
    {
        var entry = await WaitlistEntries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"WaitlistEntry {id} not found");

        entry.Status = status;
        if (status == WaitlistStatus.Offered)
            entry.OfferedDate = DateOnly.FromDateTime(DateTime.Today);
        await context.SaveChangesAsync(ct);
    }

    public async Task<WaitlistStatisticsDto> GetWaitlistStatisticsAsync(CancellationToken ct = default)
    {
        var waiting = await WaitlistEntries
            .Include(w => w.PreferredTherapist)
            .Where(w => w.Status == WaitlistStatus.Waiting)
            .ToListAsync(ct);

        var therapistQueues = waiting
            .Where(w => w.PreferredTherapistId.HasValue)
            .GroupBy(w => new { w.PreferredTherapistId, Name = w.PreferredTherapist!.FullName })
            .Select(g => new TherapistQueueDto
            {
                TherapistId = g.Key.PreferredTherapistId!.Value,
                TherapistName = g.Key.Name,
                QueueLength = g.Count()
            })
            .ToList();

        return new WaitlistStatisticsDto
        {
            TotalWaiting = waiting.Count,
            AverageWaitDays = waiting.Count > 0
                ? waiting.Average(w => (DateTime.Today - w.RequestDate.ToDateTime(TimeOnly.MinValue)).Days)
                : 0,
            UrgentCount = waiting.Count(w => w.Priority == WaitlistPriority.Urgent),
            TherapistQueues = therapistQueues
        };
    }

    // --- Mapping ---

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
        AppointmentType = a.AppointmentType,
        Notes = a.Notes
    };

    private static AppointmentSeriesDto MapSeriesToDto(AppointmentSeries s) => new()
    {
        Id = s.Id,
        PatientId = s.PatientId,
        PatientName = s.Patient?.LastName + ", " + s.Patient?.FirstName ?? string.Empty,
        DoctorId = s.DoctorId,
        DoctorName = s.Doctor?.FirstName + " " + s.Doctor?.LastName ?? string.Empty,
        TherapyCaseId = s.TherapyCaseId,
        RecurrencePattern = s.RecurrencePattern,
        DayOfWeek = s.DayOfWeek,
        StartTime = s.StartTime,
        DurationMinutes = s.DurationMinutes,
        SeriesStartDate = s.SeriesStartDate,
        SeriesEndDate = s.SeriesEndDate,
        MaxOccurrences = s.MaxOccurrences,
        Notes = s.Notes,
        GeneratedAppointments = s.Appointments?.Count ?? 0
    };

    private static WaitlistEntryDto MapWaitlistToDto(WaitlistEntry w) => new()
    {
        Id = w.Id,
        PatientId = w.PatientId,
        PatientName = w.Patient?.LastName + ", " + w.Patient?.FirstName ?? string.Empty,
        PreferredTherapistId = w.PreferredTherapistId,
        PreferredTherapistName = w.PreferredTherapist != null
            ? w.PreferredTherapist.FirstName + " " + w.PreferredTherapist.LastName
            : null,
        RequestDate = w.RequestDate,
        TherapyTypeRequested = w.TherapyTypeRequested,
        Priority = w.Priority,
        PreferredDays = w.PreferredDays,
        PreferredTimeSlot = w.PreferredTimeSlot,
        Notes = w.Notes,
        Status = w.Status,
        OfferedDate = w.OfferedDate
    };
}
