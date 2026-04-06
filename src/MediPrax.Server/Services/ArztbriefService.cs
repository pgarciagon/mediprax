using MediPrax.Application.Data;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Reporting.Arztbrief;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Server.Services;

public class ArztbriefService(DbContext context) : IArztbriefService
{
    public async Task<Guid> CreateAndGeneratePdfAsync(CreateArztbriefDto dto, CancellationToken ct = default)
    {
        var patient = await context.Set<Patient>().FindAsync([dto.PatientId], ct)
            ?? throw new KeyNotFoundException("Patient not found");
        var doctor = await context.Set<User>().FindAsync([dto.DoctorId], ct)
            ?? throw new KeyNotFoundException("Doctor not found");

        // Build diagnoses with descriptions
        var diagnosesWithDesc = dto.Diagnoses
            .Select(code =>
            {
                var desc = Icd10Catalog.GetDescription(code);
                return desc is not null ? $"{code} — {desc}" : code;
            })
            .ToList();

        var model = new ArztbriefModel
        {
            PatientName = $"{patient.FirstName} {patient.LastName}",
            PatientDateOfBirth = patient.DateOfBirth.ToString("dd.MM.yyyy"),
            PatientInsurance = patient.InsuranceProvider,
            PatientAddress = FormatAddress(patient),
            RecipientName = dto.RecipientName,
            RecipientAddress = dto.RecipientAddress,
            Date = DateTime.Today.ToString("dd.MM.yyyy"),
            Subject = dto.Subject,
            Diagnoses = diagnosesWithDesc,
            Body = dto.Body,
            DoctorName = doctor.FullName
        };

        var pdfBytes = ArztbriefGenerator.Generate(model);

        var document = new Document
        {
            PatientId = dto.PatientId,
            EncounterId = dto.EncounterId,
            DocType = DocumentType.Arztbrief,
            Title = $"{dto.Subject} — {dto.RecipientName} ({DateTime.Today:dd.MM.yyyy})",
            Content = dto.Body,
            PdfData = pdfBytes
        };

        context.Set<Document>().Add(document);
        await context.SaveChangesAsync(ct);

        return document.Id;
    }

    public async Task<byte[]?> GetPdfAsync(Guid documentId, CancellationToken ct = default)
    {
        var doc = await context.Set<Document>().FindAsync([documentId], ct);
        return doc?.PdfData;
    }

    public async Task<IReadOnlyList<ArztbriefListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await context.Set<Document>()
            .Where(d => d.PatientId == patientId && d.DocType == DocumentType.Arztbrief)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new ArztbriefListItemDto
            {
                Id = d.Id,
                Title = d.Title,
                CreatedAt = d.CreatedAt,
                HasPdf = d.PdfData != null
            })
            .ToListAsync(ct);
    }

    private static string? FormatAddress(Patient p)
    {
        var parts = new[] { p.Street, $"{p.PostalCode} {p.City}".Trim() }
            .Where(s => !string.IsNullOrWhiteSpace(s));
        var result = string.Join(", ", parts);
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
