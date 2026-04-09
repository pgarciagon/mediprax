using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public interface IGdtService
{
    Task<GdtResultDto> ImportResultAsync(string content, string deviceType, CancellationToken ct = default);
    string GenerateRequest(GdtRequestDto request);
    GdtResultDto ParseResult(string content);
}

public class GdtService(DbContext context, IAuditService auditService) : IGdtService
{
    private DbSet<Document> Documents => context.Set<Document>();
    private DbSet<Patient> Patients => context.Set<Patient>();

    public async Task<GdtResultDto> ImportResultAsync(string content, string deviceType, CancellationToken ct = default)
    {
        var result = GdtParser.Parse(content);
        result.DeviceType = deviceType;

        // Try to match patient by ID or name
        Patient? patient = null;
        if (Guid.TryParse(result.PatientId, out var patientGuid))
            patient = await Patients.FindAsync([patientGuid], ct);

        if (patient is not null)
        {
            // Create a document with the results
            var document = new Document
            {
                PatientId = patient.Id,
                Title = $"{deviceType}-Ergebnis vom {result.ResultDate:dd.MM.yyyy}",
                Content = string.Join("\n", result.ResultLines),
                DocType = DocumentType.Befund,
                CreatedAt = DateTime.UtcNow
            };

            Documents.Add(document);
            await context.SaveChangesAsync(ct);

            await auditService.LogAsync(AuditAction.Create, "Document", document.Id,
                $"GDT-Import: {deviceType} für Patient {patient.FullName}");
        }

        return result;
    }

    public string GenerateRequest(GdtRequestDto request)
    {
        return GdtWriter.WriteRequest(request);
    }

    public GdtResultDto ParseResult(string content)
    {
        return GdtParser.Parse(content);
    }
}
