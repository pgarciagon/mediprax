using System.Text;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

/// <summary>
/// Generates KVDT-compatible export files for EBM billing submission to KVHB.
/// Format based on KBV KVDT specification (simplified for MVP).
/// </summary>
public class KvdtExportService(DbContext context) : IKvdtExportService
{
    public async Task<KvdtExportResultDto> ExportQuarterAsync(string quarter, CancellationToken ct = default)
    {
        var items = await context.Set<BillingItem>()
            .Include(b => b.Patient)
            .Include(b => b.Encounter).ThenInclude(e => e.Doctor)
            .Include(b => b.Encounter).ThenInclude(e => e!.EncounterDiagnoses).ThenInclude(ed => ed.PatientDiagnosis)
            .Where(b => b.Quarter == quarter && b.BillingType == BillingType.EBM && !b.KvdtExported)
            .OrderBy(b => b.Patient.LastName)
            .ThenBy(b => b.CreatedAt)
            .ToListAsync(ct);

        if (items.Count == 0)
            return new KvdtExportResultDto { Success = false, ErrorMessage = "Keine exportierbaren EBM-Positionen gefunden." };

        var sb = new StringBuilder();
        var parts = quarter.Split("-Q");
        var year = parts[0];
        var q = parts[1];

        // KVDT file header (con0)
        sb.AppendLine($"con0");
        sb.AppendLine($"0101{year}{q}"); // Abrechnungsquartal
        sb.AppendLine($"0102030"); // KV-Bezirk Bremen = 03, Abrechnungsbereich 0
        sb.AppendLine($"0103999999900"); // BSNR (Betriebsstättennummer) — Platzhalter
        sb.AppendLine($"0201Neuropsychiatricum Bremen");

        var grouped = items.GroupBy(b => b.PatientId);
        var patientCount = 0;

        foreach (var patientGroup in grouped)
        {
            patientCount++;
            var patient = patientGroup.First().Patient;
            var lanr = patientGroup.First().Encounter?.Doctor?.Id.ToString("N")[..9] ?? "000000000"; // LANR placeholder

            // Patient record (sch0)
            sb.AppendLine($"sch0");
            sb.AppendLine($"3000{patient.Kvnr ?? ""}"); // KVNR
            sb.AppendLine($"3100{patient.InsuranceNumber ?? ""}"); // Versichertennummer
            sb.AppendLine($"3101{patient.LastName}");
            sb.AppendLine($"3102{patient.FirstName}");
            sb.AppendLine($"3103{patient.DateOfBirth:ddMMyyyy}");
            sb.AppendLine($"0212{lanr}"); // LANR des behandelnden Arztes

            // Billing items (beh0)
            foreach (var item in patientGroup)
            {
                sb.AppendLine($"beh0");
                sb.AppendLine($"5001{item.GopCode}");
                sb.AppendLine($"5005{item.Quantity}");
                sb.AppendLine($"5006{item.CreatedAt:ddMMyyyy}");

                if (item.Encounter?.Icd10Codes?.Count > 0)
                {
                    foreach (var icd in item.Encounter.Icd10Codes)
                    {
                        sb.AppendLine($"6001{icd}");

                        // Look up certainty and laterality from EncounterDiagnoses (M44)
                        var encDiag = item.Encounter.EncounterDiagnoses?
                            .FirstOrDefault(ed => ed.PatientDiagnosis?.Icd10Code == icd);

                        var certainty = encDiag?.PatientDiagnosis?.Certainty.ToString() ?? "G";
                        sb.AppendLine($"6003{certainty}");

                        if (encDiag?.PatientDiagnosis?.Laterality != null)
                            sb.AppendLine($"6004{encDiag.PatientDiagnosis.Laterality}");
                    }
                }
            }
        }

        // Footer
        sb.AppendLine($"con9");
        sb.AppendLine($"9999{items.Count:D6}");

        // Mark as exported
        foreach (var item in items)
            item.KvdtExported = true;
        await context.SaveChangesAsync(ct);

        var fileName = $"KVDT_{quarter}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.con";
        return new KvdtExportResultDto
        {
            Success = true,
            FileName = fileName,
            Content = Encoding.UTF8.GetBytes(sb.ToString()),
            PatientCount = patientCount,
            ItemCount = items.Count
        };
    }
}
