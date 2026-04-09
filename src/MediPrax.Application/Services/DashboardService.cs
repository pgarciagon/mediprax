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
            .Include(a => a.Patient).Include(a => a.Doctor)
            .Where(a => a.StartTime >= today && a.StartTime < todayEnd)
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.NoShow)
            .OrderBy(a => a.StartTime).Take(15)
            .Select(a => new DashboardAppointmentDto
            {
                Id = a.Id, PatientName = a.Patient.LastName + ", " + a.Patient.FirstName,
                PatientId = a.PatientId, DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                StartTime = a.StartTime, DurationMinutes = a.DurationMinutes,
                Status = a.Status, Notes = a.Notes
            }).ToListAsync(ct);

        // Recent activity
        var recentEncounters = await context.Set<Encounter>()
            .Include(e => e.Patient).OrderByDescending(e => e.CreatedAt).Take(5)
            .Select(e => new DashboardActivityDto
            {
                Type = "encounter", Description = "Dokumentation: " + e.Patient.LastName + ", " + e.Patient.FirstName,
                PatientId = e.PatientId, Timestamp = e.CreatedAt
            }).ToListAsync(ct);

        var recentDocs = await context.Set<Document>()
            .Include(d => d.Patient).Where(d => d.DocType == DocumentType.Arztbrief)
            .OrderByDescending(d => d.CreatedAt).Take(5)
            .Select(d => new DashboardActivityDto
            {
                Type = "arztbrief", Description = "Arztbrief: " + d.Patient.LastName + ", " + d.Patient.FirstName,
                PatientId = d.PatientId, Timestamp = d.CreatedAt
            }).ToListAsync(ct);

        var recentActivity = recentEncounters.Concat(recentDocs)
            .OrderByDescending(a => a.Timestamp).Take(8).ToList();

        // --- Arzt data ---
        var openBriefe = await context.Set<Document>()
            .CountAsync(d => d.DocType == DocumentType.Arztbrief
                && (d.ArztbriefStatus == ArztbriefStatus.Entwurf || d.ArztbriefStatus == null), ct);

        var todayDate = DateOnly.FromDateTime(DateTime.Today);
        var dueRecalls = await context.Set<Recall>()
            .CountAsync(r => r.Status == RecallStatus.Open && r.DueDate <= todayDate.AddDays(7), ct);
        var overdueRecalls = await context.Set<Recall>()
            .CountAsync(r => r.Status == RecallStatus.Open && r.DueDate < todayDate, ct);

        // Warnings: suicidality, therapy limits, lab due
        var warnings = new List<DashboardWarningDto>();

        var suicideRisks = await context.Set<Patient>()
            .Where(p => p.CurrentSuicidalityRisk >= SuicidalityRiskLevel.Moderate)
            .Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName, p.CurrentSuicidalityRisk })
            .ToListAsync(ct);
        foreach (var r in suicideRisks)
        {
            warnings.Add(new DashboardWarningDto
            {
                Level = r.CurrentSuicidalityRisk >= SuicidalityRiskLevel.High ? "danger" : "warning",
                Message = $"Suizidalitätsrisiko: {r.CurrentSuicidalityRisk}",
                PatientId = r.Id, PatientName = r.Name
            });
        }

        var therapyLimits = await context.Set<TherapyCase>()
            .Include(t => t.Patient)
            .Where(t => t.Status == TherapyCaseStatus.InBehandlung && t.ApprovedSessions > 0
                && t.ApprovedSessions - t.CompletedSessions <= 4)
            .ToListAsync(ct);
        foreach (var t in therapyLimits)
        {
            warnings.Add(new DashboardWarningDto
            {
                Level = t.ApprovedSessions - t.CompletedSessions <= 2 ? "danger" : "warning",
                Message = $"Therapie-Kontingent: {t.CompletedSessions}/{t.ApprovedSessions} Sitzungen ({t.ApprovedSessions - t.CompletedSessions} übrig)",
                PatientId = t.PatientId, PatientName = t.Patient?.FullName
            });
        }

        // --- MFA data ---
        var waitingRoom = await context.Set<Appointment>()
            .CountAsync(a => a.StartTime >= today && a.StartTime < todayEnd && a.Status == AppointmentStatus.CheckedIn, ct);
        var inTreatment = await context.Set<Appointment>()
            .CountAsync(a => a.StartTime >= today && a.StartTime < todayEnd && a.Status == AppointmentStatus.InProgress, ct);

        var waitlistTotal = await context.Set<WaitlistEntry>()
            .CountAsync(w => w.Status == WaitlistStatus.Waiting, ct);
        var waitlistUrgent = await context.Set<WaitlistEntry>()
            .CountAsync(w => w.Status == WaitlistStatus.Waiting && w.Priority == WaitlistPriority.Urgent, ct);
        var waitlistEntries = await context.Set<WaitlistEntry>()
            .Where(w => w.Status == WaitlistStatus.Waiting).Select(w => w.RequestDate).ToListAsync(ct);
        var waitlistAvgDays = waitlistEntries.Count > 0
            ? waitlistEntries.Average(d => (DateTime.Today - d.ToDateTime(TimeOnly.MinValue)).Days)
            : 0;

        var missingData = await context.Set<Patient>()
            .CountAsync(p => string.IsNullOrEmpty(p.Kvnr) || string.IsNullOrEmpty(p.InsuranceProvider), ct);

        // --- Admin data ---
        var now = DateTime.UtcNow;
        var quarter = $"{now.Year}-Q{(now.Month - 1) / 3 + 1}";
        var revenueQuarter = await context.Set<BillingItem>()
            .Where(b => b.Quarter == quarter).SumAsync(b => (decimal?)0, ct) ?? 0; // Simplified — no Amount field

        var openInvoices = 0;
        var openInvoiceAmount = 0m;
        var overdueInvoices = 0;
        try
        {
            openInvoices = await context.Set<PrivateInvoice>()
                .CountAsync(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled, ct);
            openInvoiceAmount = await context.Set<PrivateInvoice>()
                .Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
                .SumAsync(i => (decimal?)i.TotalGross, ct) ?? 0;
            overdueInvoices = await context.Set<PrivateInvoice>()
                .CountAsync(i => i.Status == InvoiceStatus.Overdue, ct);
        }
        catch { /* PrivateInvoice table might not have data */ }

        return new DashboardDto
        {
            TotalPatients = totalPatients,
            AppointmentsToday = appointmentsToday,
            EncountersThisWeek = encountersThisWeek,
            PendingArztbriefe = pendingArztbriefe,
            UpcomingAppointments = upcoming,
            RecentActivity = recentActivity,
            // Arzt
            OpenArztbriefe = openBriefe,
            DueRecalls = dueRecalls,
            OverdueRecalls = overdueRecalls,
            Warnings = warnings,
            // MFA
            WaitingRoomCount = waitingRoom,
            InTreatmentCount = inTreatment,
            WaitlistTotal = waitlistTotal,
            WaitlistUrgent = waitlistUrgent,
            WaitlistAvgDays = waitlistAvgDays,
            PatientsWithMissingData = missingData,
            // Admin
            RevenueQuarter = revenueQuarter,
            OpenInvoices = openInvoices,
            OpenInvoiceAmount = openInvoiceAmount,
            OverdueInvoices = overdueInvoices
        };
    }
}
