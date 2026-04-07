using MediPrax.Application.Data;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class ReportService(DbContext context) : IReportService
{
    public async Task<DailyReportDto> GetDailyReportAsync(DateOnly date, CancellationToken ct = default)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var dayEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var appointments = await context.Set<Appointment>()
            .Include(a => a.Doctor)
            .Where(a => a.StartTime >= dayStart && a.StartTime < dayEnd)
            .ToListAsync(ct);

        var encounters = await context.Set<Encounter>()
            .Include(e => e.Doctor)
            .Where(e => e.EncounterDate == date)
            .ToListAsync(ct);

        var documents = await context.Set<Document>()
            .Where(d => d.CreatedAt >= dayStart && d.CreatedAt < dayEnd)
            .CountAsync(ct);

        var doctorBreakdown = appointments
            .GroupBy(a => a.Doctor.FullName)
            .Select(g => new DoctorDayDto
            {
                DoctorName = g.Key,
                Appointments = g.Count(),
                Encounters = encounters.Count(e => e.Doctor.FullName == g.Key)
            })
            .OrderByDescending(d => d.Appointments)
            .ToList();

        return new DailyReportDto
        {
            Date = date,
            TotalAppointments = appointments.Count,
            CompletedAppointments = appointments.Count(a => a.Status == AppointmentStatus.Completed),
            EncountersCreated = encounters.Count,
            DocumentsCreated = documents,
            DoctorBreakdown = doctorBreakdown
        };
    }

    public async Task<QuarterlyReportDto> GetQuarterlyReportAsync(string quarter, CancellationToken ct = default)
    {
        // Parse quarter "2026-Q2" => date range
        var parts = quarter.Split("-Q");
        if (parts.Length != 2 || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var q))
            return new QuarterlyReportDto { Quarter = quarter };

        var startMonth = (q - 1) * 3 + 1;
        var qStart = new DateOnly(year, startMonth, 1);
        var qEnd = qStart.AddMonths(3);
        var qStartDt = qStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var qEndDt = qEnd.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var encounters = await context.Set<Encounter>()
            .Include(e => e.Doctor)
            .Where(e => e.EncounterDate >= qStart && e.EncounterDate < qEnd)
            .ToListAsync(ct);

        var patientsSeen = encounters.Select(e => e.PatientId).Distinct().Count();

        var documents = await context.Set<Document>()
            .Where(d => d.CreatedAt >= qStartDt && d.CreatedAt < qEndDt)
            .CountAsync(ct);

        var newPatients = await context.Set<Patient>()
            .Where(p => p.CreatedAt >= qStartDt && p.CreatedAt < qEndDt)
            .CountAsync(ct);

        var billingItems = await context.Set<BillingItem>()
            .Where(b => b.Quarter == quarter)
            .ToListAsync(ct);

        var estimatedRevenue = billingItems.Sum(b =>
            b.BillingType == BillingType.GOA
                ? GopCatalog.CalculateGoaeAmount(b.GopCode, 2.3m) * b.Quantity
                : GopCatalog.CalculateEbmAmount(b.GopCode) * b.Quantity);

        // Top diagnoses
        var allCodes = encounters.SelectMany(e => e.Icd10Codes).ToList();
        var topDiagnoses = allCodes
            .GroupBy(c => c)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new TopDiagnosisDto
            {
                Code = g.Key,
                Description = Icd10Catalog.GetDescription(g.Key),
                Count = g.Count()
            })
            .ToList();

        var doctorBreakdown = encounters
            .GroupBy(e => e.Doctor.FullName)
            .Select(g => new DoctorQuarterDto
            {
                DoctorName = g.Key,
                Encounters = g.Count(),
                BillingItems = billingItems.Count(b => g.Any(e => e.Id == b.EncounterId))
            })
            .OrderByDescending(d => d.Encounters)
            .ToList();

        return new QuarterlyReportDto
        {
            Quarter = quarter,
            TotalPatientsSeen = patientsSeen,
            TotalEncounters = encounters.Count,
            TotalDocuments = documents,
            NewPatients = newPatients,
            BillingItems = billingItems.Count,
            EstimatedRevenue = estimatedRevenue,
            TopDiagnoses = topDiagnoses,
            DoctorBreakdown = doctorBreakdown
        };
    }
}
