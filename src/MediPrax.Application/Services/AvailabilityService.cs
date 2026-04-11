using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class AvailabilityService(DbContext context) : IAvailabilityService
{
    private DbSet<DoctorScheduleTemplate> Schedules => context.Set<DoctorScheduleTemplate>();
    private DbSet<DoctorAbsence> Absences => context.Set<DoctorAbsence>();
    private DbSet<Appointment> Appointments => context.Set<Appointment>();

    // --- Schedule Templates ---

    public async Task<IReadOnlyList<ScheduleBlockDto>> GetScheduleAsync(Guid doctorId, CancellationToken ct = default)
    {
        return await Schedules
            .Where(s => s.DoctorId == doctorId && s.IsActive)
            .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
            .Select(s => new ScheduleBlockDto
            {
                Id = s.Id, DoctorId = s.DoctorId, DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime, EndTime = s.EndTime,
                SlotDurationMinutes = s.SlotDurationMinutes, IsActive = s.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<ScheduleBlockDto> CreateScheduleBlockAsync(CreateScheduleBlockDto dto, CancellationToken ct = default)
    {
        var block = new DoctorScheduleTemplate
        {
            DoctorId = dto.DoctorId, DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime, EndTime = dto.EndTime,
            SlotDurationMinutes = dto.SlotDurationMinutes, IsActive = true
        };
        Schedules.Add(block);
        await context.SaveChangesAsync(ct);
        return new ScheduleBlockDto
        {
            Id = block.Id, DoctorId = block.DoctorId, DayOfWeek = block.DayOfWeek,
            StartTime = block.StartTime, EndTime = block.EndTime,
            SlotDurationMinutes = block.SlotDurationMinutes, IsActive = true
        };
    }

    public async Task DeleteScheduleBlockAsync(Guid id, CancellationToken ct = default)
    {
        var block = await Schedules.FindAsync([id], ct);
        if (block is not null) { Schedules.Remove(block); await context.SaveChangesAsync(ct); }
    }

    // --- Absences ---

    public async Task<IReadOnlyList<AbsenceDto>> GetAbsencesAsync(Guid? doctorId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var query = Absences.Include(a => a.Doctor).Include(a => a.Substitute).AsQueryable();
        if (doctorId.HasValue) query = query.Where(a => a.DoctorId == doctorId.Value);
        return await query
            .Where(a => a.StartDate <= to && a.EndDate >= from)
            .OrderBy(a => a.StartDate)
            .Select(a => new AbsenceDto
            {
                Id = a.Id, DoctorId = a.DoctorId, DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                StartDate = a.StartDate, EndDate = a.EndDate,
                StartTime = a.StartTime, EndTime = a.EndTime,
                AbsenceType = a.AbsenceType, Reason = a.Reason,
                SubstituteName = a.Substitute != null ? a.Substitute.FirstName + " " + a.Substitute.LastName : null
            })
            .ToListAsync(ct);
    }

    public async Task<AbsenceDto> CreateAbsenceAsync(CreateAbsenceDto dto, CancellationToken ct = default)
    {
        var absence = new DoctorAbsence
        {
            DoctorId = dto.DoctorId, StartDate = dto.StartDate, EndDate = dto.EndDate,
            StartTime = dto.StartTime, EndTime = dto.EndTime,
            AbsenceType = dto.AbsenceType, Reason = dto.Reason, SubstituteId = dto.SubstituteId
        };
        Absences.Add(absence);
        await context.SaveChangesAsync(ct);
        return new AbsenceDto
        {
            Id = absence.Id, DoctorId = absence.DoctorId,
            StartDate = absence.StartDate, EndDate = absence.EndDate,
            StartTime = absence.StartTime, EndTime = absence.EndTime,
            AbsenceType = absence.AbsenceType, Reason = absence.Reason
        };
    }

    public async Task DeleteAbsenceAsync(Guid id, CancellationToken ct = default)
    {
        var absence = await Absences.FindAsync([id], ct);
        if (absence is not null) { Absences.Remove(absence); await context.SaveChangesAsync(ct); }
    }

    // --- Availability Calculation ---

    public async Task<IReadOnlyList<FreeSlotDto>> GetFreeSlotsAsync(Guid doctorId, DateOnly date, int durationMinutes, CancellationToken ct = default)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var schedule = await Schedules
            .Where(s => s.DoctorId == doctorId && s.IsActive && s.DayOfWeek == date.DayOfWeek)
            .ToListAsync(ct);

        if (schedule.Count == 0) return [];

        // Check if doctor is absent on this date
        var absences = await Absences
            .Where(a => a.DoctorId == doctorId && a.StartDate <= date && a.EndDate >= date)
            .ToListAsync(ct);

        var fullDayAbsent = absences.Any(a => a.IsFullDay);
        if (fullDayAbsent) return [];

        // Get existing appointments for this doctor on this date
        var dayStart = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var dayEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        // Convert local date boundaries to UTC for query
        var localDayStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
        var utcDayStart = TimeZoneInfo.ConvertTimeToUtc(localDayStart, tz);
        var utcDayEnd = utcDayStart.AddDays(1);

        var existingAppts = await Appointments
            .Where(a => a.DoctorId == doctorId && a.StartTime >= utcDayStart && a.StartTime < utcDayEnd
                     && a.Status != AppointmentStatus.Cancelled)
            .Select(a => new { a.StartTime, a.DurationMinutes })
            .ToListAsync(ct);

        var slots = new List<FreeSlotDto>();
        var doctor = await context.Set<User>().FindAsync([doctorId], ct);
        var doctorName = doctor?.FullName ?? "";

        foreach (var block in schedule)
        {
            var current = block.StartTime;
            while (current.AddMinutes(durationMinutes) <= block.EndTime)
            {
                var slotLocalStart = new DateTime(date.Year, date.Month, date.Day, current.Hour, current.Minute, 0, DateTimeKind.Unspecified);
                var slotUtcStart = TimeZoneInfo.ConvertTimeToUtc(slotLocalStart, tz);
                var slotUtcEnd = slotUtcStart.AddMinutes(durationMinutes);

                // Check partial absences (Sperrzeiten)
                var blocked = absences.Any(a => !a.IsFullDay
                    && a.StartTime.HasValue && a.EndTime.HasValue
                    && current < a.EndTime.Value && current.AddMinutes(durationMinutes) > a.StartTime.Value);

                // Check existing appointments
                var occupied = existingAppts.Any(a =>
                    slotUtcStart < a.StartTime.AddMinutes(a.DurationMinutes) && slotUtcEnd > a.StartTime);

                if (!blocked && !occupied)
                {
                    slots.Add(new FreeSlotDto
                    {
                        Start = slotUtcStart, End = slotUtcEnd,
                        DurationMinutes = durationMinutes,
                        DoctorId = doctorId, DoctorName = doctorName
                    });
                }

                current = current.AddMinutes(block.SlotDurationMinutes > 0 ? block.SlotDurationMinutes : 25);
            }
        }

        return slots;
    }

    public async Task<IReadOnlyList<FreeSlotDto>> FindNextFreeSlotAsync(Guid doctorId, int durationMinutes, DateOnly searchFrom, int maxResults = 5, DayOfWeek? preferredDay = null, TimeOnly? preferredTime = null, CancellationToken ct = default)
    {
        var allSlots = new List<FreeSlotDto>();
        var currentDate = searchFrom;
        var maxSearchDays = 56; // 8 weeks

        for (var i = 0; i < maxSearchDays; i++)
        {
            if (currentDate.DayOfWeek == DayOfWeek.Sunday) { currentDate = currentDate.AddDays(1); continue; }

            var daySlots = await GetFreeSlotsAsync(doctorId, currentDate, durationMinutes, ct);
            allSlots.AddRange(daySlots);

            currentDate = currentDate.AddDays(1);

            // Stop early if we have enough and no preferences to sort by
            if (!preferredDay.HasValue && !preferredTime.HasValue && allSlots.Count >= maxResults)
                break;
        }

        // Sort: preferred day/time first, then chronological
        IEnumerable<FreeSlotDto> sorted = allSlots.OrderBy(s => s.Start);
        if (preferredDay.HasValue || preferredTime.HasValue)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
            sorted = allSlots
                .OrderBy(s => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(s.Start, DateTimeKind.Utc), tz).DayOfWeek == preferredDay ? 0 : 1)
                .ThenBy(s => preferredTime.HasValue
                    ? Math.Abs((TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(s.Start, DateTimeKind.Utc), tz)) - preferredTime.Value).Ticks)
                    : 0)
                .ThenBy(s => s.Start);
        }

        return sorted.Take(maxResults).ToList();
    }

    public async Task<AvailabilityCheckResult> CheckAvailabilityAsync(Guid doctorId, DateTime start, int durationMinutes, CancellationToken ct = default)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(start, DateTimeKind.Utc), tz);
        var date = DateOnly.FromDateTime(localStart);
        var time = TimeOnly.FromDateTime(localStart);
        var endTime = time.AddMinutes(durationMinutes);

        // Check absences (hard block)
        var absences = await Absences
            .Where(a => a.DoctorId == doctorId && a.StartDate <= date && a.EndDate >= date)
            .ToListAsync(ct);

        var fullDayAbsence = absences.FirstOrDefault(a => a.IsFullDay);
        if (fullDayAbsence is not null)
        {
            return new AvailabilityCheckResult
            {
                IsAvailable = false,
                ConflictReason = $"Arzt ist {fullDayAbsence.AbsenceType} ({fullDayAbsence.StartDate:dd.MM.} – {fullDayAbsence.EndDate:dd.MM.yyyy})"
            };
        }

        var partialAbsence = absences.FirstOrDefault(a => !a.IsFullDay && a.StartTime.HasValue && a.EndTime.HasValue
            && time < a.EndTime.Value && endTime > a.StartTime.Value);
        if (partialAbsence is not null)
        {
            return new AvailabilityCheckResult
            {
                IsAvailable = false,
                ConflictReason = $"Sperrzeit: {partialAbsence.StartTime:HH:mm}–{partialAbsence.EndTime:HH:mm} ({partialAbsence.Reason})"
            };
        }

        // Check Sprechzeiten (warning only)
        var schedule = await Schedules
            .Where(s => s.DoctorId == doctorId && s.IsActive && s.DayOfWeek == date.DayOfWeek)
            .ToListAsync(ct);

        var withinSprechzeit = schedule.Any(s => time >= s.StartTime && endTime <= s.EndTime);
        if (!withinSprechzeit && schedule.Count > 0)
        {
            return new AvailabilityCheckResult
            {
                IsAvailable = true, IsWarningOnly = true,
                ConflictReason = "Termin liegt außerhalb der Sprechzeiten"
            };
        }

        // Check overlapping appointments (warning)
        var utcStart = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var utcEnd = utcStart.AddMinutes(durationMinutes);
        var overlap = await Appointments.AnyAsync(a =>
            a.DoctorId == doctorId && a.Status != AppointmentStatus.Cancelled
            && a.StartTime < utcEnd && a.StartTime.AddMinutes(a.DurationMinutes) > utcStart, ct);

        if (overlap)
        {
            return new AvailabilityCheckResult
            {
                IsAvailable = true, IsWarningOnly = true,
                ConflictReason = "Es gibt bereits einen Termin in diesem Zeitraum"
            };
        }

        return new AvailabilityCheckResult { IsAvailable = true };
    }
}
