using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class SearchService(DbContext context) : ISearchService
{
    public async Task<GlobalSearchResultDto> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return new GlobalSearchResultDto();

        var lower = query.ToLower();
        var results = new List<SearchResultItemDto>();

        // Patients
        var patients = await context.Set<Patient>()
            .Where(p => (p.FirstName + " " + p.LastName).ToLower().Contains(lower)
                     || (p.LastName + ", " + p.FirstName).ToLower().Contains(lower)
                     || (p.Kvnr != null && p.Kvnr.ToLower().Contains(lower))
                     || (p.Phone != null && p.Phone.Contains(query)))
            .Take(5)
            .Select(p => new SearchResultItemDto
            {
                Category = "Patient",
                Title = p.LastName + ", " + p.FirstName,
                Subtitle = p.DateOfBirth.ToString("dd.MM.yyyy") + (p.Kvnr != null ? " · " + p.Kvnr : ""),
                Url = "/patienten/" + p.Id
            })
            .ToListAsync(ct);
        results.AddRange(patients);

        // Appointments (by patient name, today+future)
        var appointments = await context.Set<Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.StartTime >= DateTime.UtcNow.Date)
            .Where(a => (a.Patient.FirstName + " " + a.Patient.LastName).ToLower().Contains(lower)
                     || (a.Patient.LastName + ", " + a.Patient.FirstName).ToLower().Contains(lower))
            .OrderBy(a => a.StartTime)
            .Take(5)
            .Select(a => new SearchResultItemDto
            {
                Category = "Termin",
                Title = a.Patient.LastName + ", " + a.Patient.FirstName,
                Subtitle = a.StartTime.ToString("dd.MM.yyyy HH:mm") + " · " + a.Doctor.LastName,
                Url = "/termine/" + a.Id
            })
            .ToListAsync(ct);
        results.AddRange(appointments);

        // Encounters (by ICD code or patient name)
        var encounters = await context.Set<Encounter>()
            .Include(e => e.Patient)
            .Include(e => e.Doctor)
            .Where(e => (e.Patient.FirstName + " " + e.Patient.LastName).ToLower().Contains(lower)
                     || e.Icd10Codes.Any(c => c.ToLower().Contains(lower)))
            .OrderByDescending(e => e.EncounterDate)
            .Take(5)
            .Select(e => new SearchResultItemDto
            {
                Category = "Dokumentation",
                Title = e.Patient.LastName + ", " + e.Patient.FirstName,
                Subtitle = e.EncounterDate.ToString("dd.MM.yyyy") + " · " + e.Doctor.LastName,
                Url = "/dokumentation/" + e.Id + "/bearbeiten"
            })
            .ToListAsync(ct);
        results.AddRange(encounters);

        return new GlobalSearchResultDto
        {
            Results = results,
            TotalCount = results.Count
        };
    }
}
