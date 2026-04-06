using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class DashboardService(DbContext context) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var todayEnd = today.AddDays(1);
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(7);

        var totalPatients = await context.Set<Patient>().CountAsync(ct);

        var appointmentsToday = await context.Set<Appointment>()
            .CountAsync(a => a.StartTime >= today && a.StartTime < todayEnd, ct);

        var encountersThisWeek = await context.Set<Encounter>()
            .CountAsync(e => e.CreatedAt >= weekStart && e.CreatedAt < weekEnd, ct);

        var pendingArztbriefe = await context.Set<Document>()
            .CountAsync(d => d.DocType == DocumentType.Arztbrief && d.PdfData != null, ct);

        var upcoming = await context.Set<Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.StartTime >= today && a.StartTime < todayEnd)
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.NoShow)
            .OrderBy(a => a.StartTime)
            .Take(10)
            .Select(a => new DashboardAppointmentDto
            {
                Id = a.Id,
                PatientName = a.Patient.LastName + ", " + a.Patient.FirstName,
                PatientId = a.PatientId,
                DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                StartTime = a.StartTime,
                DurationMinutes = a.DurationMinutes,
                Status = a.Status
            })
            .ToListAsync(ct);

        // Recent activity: last 10 encounters + documents
        var recentEncounters = await context.Set<Encounter>()
            .Include(e => e.Patient)
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .Select(e => new DashboardActivityDto
            {
                Type = "encounter",
                Description = "Dokumentation: " + e.Patient.LastName + ", " + e.Patient.FirstName,
                PatientId = e.PatientId,
                Timestamp = e.CreatedAt
            })
            .ToListAsync(ct);

        var recentDocs = await context.Set<Document>()
            .Include(d => d.Patient)
            .Where(d => d.DocType == DocumentType.Arztbrief)
            .OrderByDescending(d => d.CreatedAt)
            .Take(5)
            .Select(d => new DashboardActivityDto
            {
                Type = "arztbrief",
                Description = "Arztbrief: " + d.Patient.LastName + ", " + d.Patient.FirstName,
                PatientId = d.PatientId,
                Timestamp = d.CreatedAt
            })
            .ToListAsync(ct);

        var recentActivity = recentEncounters
            .Concat(recentDocs)
            .OrderByDescending(a => a.Timestamp)
            .Take(8)
            .ToList();

        return new DashboardDto
        {
            TotalPatients = totalPatients,
            AppointmentsToday = appointmentsToday,
            EncountersThisWeek = encountersThisWeek,
            PendingArztbriefe = pendingArztbriefe,
            UpcomingAppointments = upcoming,
            RecentActivity = recentActivity
        };
    }
}
