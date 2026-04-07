using System.Globalization;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

/// <summary>
/// Imports patient data from CSV files.
/// Expected CSV format (semicolon-separated, UTF-8):
/// Nachname;Vorname;Geburtsdatum;Geschlecht;KVNR;Versichertennr;Krankenkasse;Versicherungsart;Straße;PLZ;Ort;Telefon;Email
/// </summary>
public class ImportService(DbContext context) : IImportService
{
    public async Task<ImportResultDto> ImportPatientsCsvAsync(Stream csvStream, CancellationToken ct = default)
    {
        using var reader = new StreamReader(csvStream);
        var errors = new List<string>();
        int imported = 0, skipped = 0, lineNr = 0;

        var header = await reader.ReadLineAsync(ct);
        if (header is null) return new ImportResultDto { ErrorMessages = ["Leere Datei."] };

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            lineNr++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = line.Split(';');
            if (cols.Length < 3)
            {
                errors.Add($"Zeile {lineNr}: Zu wenige Spalten.");
                continue;
            }

            var lastName = cols[0].Trim();
            var firstName = cols[1].Trim();
            if (!DateOnly.TryParseExact(cols[2].Trim(), ["dd.MM.yyyy", "yyyy-MM-dd"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
            {
                errors.Add($"Zeile {lineNr}: Ungültiges Geburtsdatum '{cols[2].Trim()}'.");
                continue;
            }

            // Check for duplicate by name + DOB
            var exists = await context.Set<Patient>()
                .AnyAsync(p => p.LastName == lastName && p.FirstName == firstName && p.DateOfBirth == dob, ct);
            if (exists) { skipped++; continue; }

            var patient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dob,
                Gender = cols.Length > 3 ? NullIfEmpty(cols[3]) : null,
                Kvnr = cols.Length > 4 ? NullIfEmpty(cols[4]) : null,
                InsuranceNumber = cols.Length > 5 ? NullIfEmpty(cols[5]) : null,
                InsuranceProvider = cols.Length > 6 ? NullIfEmpty(cols[6]) : null,
                InsuranceType = cols.Length > 7 ? ParseInsuranceType(cols[7]) : null,
                Street = cols.Length > 8 ? NullIfEmpty(cols[8]) : null,
                PostalCode = cols.Length > 9 ? NullIfEmpty(cols[9]) : null,
                City = cols.Length > 10 ? NullIfEmpty(cols[10]) : null,
                Phone = cols.Length > 11 ? NullIfEmpty(cols[11]) : null,
                Email = cols.Length > 12 ? NullIfEmpty(cols[12]) : null
            };

            context.Set<Patient>().Add(patient);
            imported++;
        }

        if (imported > 0)
            await context.SaveChangesAsync(ct);

        return new ImportResultDto
        {
            Imported = imported,
            Skipped = skipped,
            Errors = errors.Count,
            ErrorMessages = errors
        };
    }

    private static string? NullIfEmpty(string s)
    {
        var trimmed = s.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static InsuranceType? ParseInsuranceType(string s)
    {
        var trimmed = s.Trim().ToUpper();
        return trimmed switch
        {
            "GKV" => InsuranceType.GKV,
            "PKV" or "BG" or "SELBSTZAHLER" or "SZ" => InsuranceType.PKV,
            _ => null
        };
    }
}
