using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class SlotSuggestionService(IAvailabilityService availabilityService, DbContext context) : ISlotSuggestionService
{
    public async Task<IReadOnlyList<FreeSlotDto>> SuggestNextSlotAsync(Guid doctorId, int durationMinutes, DayOfWeek? preferredDay = null, TimeOnly? preferredTime = null, CancellationToken ct = default)
    {
        var searchFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        return await availabilityService.FindNextFreeSlotAsync(doctorId, durationMinutes, searchFrom, 5, preferredDay, preferredTime, ct);
    }

    public async Task<IReadOnlyList<FreeSlotDto>> SuggestNextTherapySessionAsync(Guid therapyCaseId, CancellationToken ct = default)
    {
        var tc = await context.Set<TherapyCase>().FindAsync([therapyCaseId], ct);
        if (tc is null) return [];

        // Find last appointment for this therapy (same patient + therapist)
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var lastAppt = await context.Set<Appointment>()
            .Where(a => a.PatientId == tc.PatientId && a.DoctorId == tc.TherapistId
                     && a.Status != AppointmentStatus.Cancelled)
            .OrderByDescending(a => a.StartTime)
            .FirstOrDefaultAsync(ct);

        DayOfWeek? preferredDay = null;
        TimeOnly? preferredTime = null;

        if (lastAppt is not null)
        {
            var localLast = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(lastAppt.StartTime, DateTimeKind.Utc), tz);
            preferredDay = localLast.DayOfWeek;
            preferredTime = TimeOnly.FromDateTime(localLast);
        }

        var searchFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        return await availabilityService.FindNextFreeSlotAsync(
            tc.TherapistId, tc.SessionDurationMinutes, searchFrom, 5, preferredDay, preferredTime, ct);
    }
}
