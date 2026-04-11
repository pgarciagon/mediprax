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

        var pattern = $"%{query}%";
        var results = new List<SearchResultItemDto>();

        // Patients
        var patients = await context.Set<Patient>()
            .Where(p => EF.Functions.Like(p.FirstName + " " + p.LastName, pattern)
                     || EF.Functions.Like(p.LastName + ", " + p.FirstName, pattern)
                     || (p.Kvnr != null && EF.Functions.Like(p.Kvnr, pattern))
                     || (p.Phone != null && EF.Functions.Like(p.Phone, pattern)))
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
            .Where(a => EF.Functions.Like(a.Patient.FirstName + " " + a.Patient.LastName, pattern)
                     || EF.Functions.Like(a.Patient.LastName + ", " + a.Patient.FirstName, pattern))
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

        // Encounters (by patient name)
        var encountersByName = await context.Set<Encounter>()
            .Include(e => e.Patient)
            .Include(e => e.Doctor)
            .Where(e => EF.Functions.Like(e.Patient.FirstName + " " + e.Patient.LastName, pattern))
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
        results.AddRange(encountersByName);

        // Encounters (by ICD code — evaluated client-side for JSON collection compatibility)
        if (encountersByName.Count < 5)
        {
            var encountersByIcd = await context.Set<Encounter>()
                .Include(e => e.Patient)
                .Include(e => e.Doctor)
                .OrderByDescending(e => e.EncounterDate)
                .Take(50)
                .ToListAsync(ct);
            var icdResults = encountersByIcd
                .Where(e => e.Icd10Codes.Any(c => c.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Take(5 - encountersByName.Count)
                .Select(e => new SearchResultItemDto
                {
                    Category = "Dokumentation",
                    Title = e.Patient.LastName + ", " + e.Patient.FirstName,
                    Subtitle = e.EncounterDate.ToString("dd.MM.yyyy") + " · " + e.Doctor.LastName,
                    Url = "/dokumentation/" + e.Id + "/bearbeiten"
                });
            results.AddRange(icdResults);
        }

        return new GlobalSearchResultDto
        {
            Results = results,
            TotalCount = results.Count
        };
    }
}
