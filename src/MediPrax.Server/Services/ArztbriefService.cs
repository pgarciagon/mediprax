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
    private DbSet<Document> Documents => context.Set<Document>();

    public async Task<Guid> SaveDraftAsync(CreateArztbriefDto dto, CancellationToken ct = default)
    {
        var document = new Document
        {
            PatientId = dto.PatientId,
            EncounterId = dto.EncounterId,
            DocType = DocumentType.Arztbrief,
            Title = $"{dto.Subject} — {dto.RecipientName} ({DateTime.Today:dd.MM.yyyy})",
            Content = dto.Body,
            ArztbriefStatus = ArztbriefStatus.Entwurf,
            DoctorId = dto.DoctorId,
            RecipientName = dto.RecipientName,
            RecipientAddress = dto.RecipientAddress,
            Subject = dto.Subject,
            Diagnoses = dto.Diagnoses
        };

        Documents.Add(document);
        await context.SaveChangesAsync(ct);
        return document.Id;
    }

    public async Task UpdateAsync(Guid id, CreateArztbriefDto dto, CancellationToken ct = default)
    {
        var doc = await Documents.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Document {id} not found");

        if (doc.ArztbriefStatus == ArztbriefStatus.Versendet)
            throw new InvalidOperationException("Versendete Arztbriefe können nicht bearbeitet werden.");

        doc.Content = dto.Body;
        doc.DoctorId = dto.DoctorId;
        doc.EncounterId = dto.EncounterId;
        doc.RecipientName = dto.RecipientName;
        doc.RecipientAddress = dto.RecipientAddress;
        doc.Subject = dto.Subject;
        doc.Diagnoses = dto.Diagnoses;
        doc.Title = $"{dto.Subject} — {dto.RecipientName} ({DateTime.Today:dd.MM.yyyy})";

        // Reset to draft if was Freigegeben (content changed)
        if (doc.ArztbriefStatus == ArztbriefStatus.Freigegeben)
        {
            doc.ArztbriefStatus = ArztbriefStatus.Entwurf;
            doc.PdfData = null; // Invalidate old PDF
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task<Guid> GeneratePdfAsync(Guid id, CancellationToken ct = default)
    {
        var doc = await Documents.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Document {id} not found");

        var patient = await context.Set<Patient>().FindAsync([doc.PatientId], ct)
            ?? throw new KeyNotFoundException("Patient not found");
        var doctor = doc.DoctorId.HasValue
            ? await context.Set<User>().FindAsync([doc.DoctorId.Value], ct)
            : null;

        var diagnosesWithDesc = (doc.Diagnoses ?? [])
            .Select(code =>
            {
                var desc = Icd10Catalog.GetDescription(code);
                return desc is not null ? $"{code} — {desc}" : code;
            })
            .ToList();

        var model = new ArztbriefModel
        {
            PatientName = patient.FullName,
            PatientDateOfBirth = patient.DateOfBirth.ToString("dd.MM.yyyy"),
            PatientInsurance = patient.InsuranceProvider,
            PatientAddress = FormatAddress(patient),
            RecipientName = doc.RecipientName ?? "",
            RecipientAddress = doc.RecipientAddress,
            Date = DateTime.Today.ToString("dd.MM.yyyy"),
            Subject = doc.Subject ?? "Arztbrief",
            Diagnoses = diagnosesWithDesc,
            Body = doc.Content ?? "",
            DoctorName = doctor?.FullName ?? ""
        };

        doc.PdfData = ArztbriefGenerator.Generate(model);
        await context.SaveChangesAsync(ct);
        return id;
    }

    public async Task UpdateStatusAsync(Guid id, ArztbriefStatus status, CancellationToken ct = default)
    {
        var doc = await Documents.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Document {id} not found");
        doc.ArztbriefStatus = status;
        await context.SaveChangesAsync(ct);
    }

    public async Task<ArztbriefDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var doc = await Documents.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (doc is null) return null;

        return new ArztbriefDetailDto
        {
            Id = doc.Id,
            PatientId = doc.PatientId,
            DoctorId = doc.DoctorId ?? Guid.Empty,
            EncounterId = doc.EncounterId,
            RecipientName = doc.RecipientName ?? "",
            RecipientAddress = doc.RecipientAddress,
            Subject = doc.Subject ?? "",
            Body = doc.Content ?? "",
            Diagnoses = doc.Diagnoses ?? [],
            Status = doc.ArztbriefStatus ?? ArztbriefStatus.Entwurf,
            HasPdf = doc.PdfData is not null,
            CreatedAt = doc.CreatedAt
        };
    }

    public async Task<byte[]?> GetPdfAsync(Guid documentId, CancellationToken ct = default)
    {
        var doc = await Documents.FindAsync([documentId], ct);
        return doc?.PdfData;
    }

    public async Task<IReadOnlyList<ArztbriefListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Documents
            .Where(d => d.PatientId == patientId && d.DocType == DocumentType.Arztbrief)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new ArztbriefListItemDto
            {
                Id = d.Id,
                Title = d.Title,
                RecipientName = d.RecipientName,
                Status = d.ArztbriefStatus ?? ArztbriefStatus.Entwurf,
                CreatedAt = d.CreatedAt,
                HasPdf = d.PdfData != null
            })
            .ToListAsync(ct);
    }

    // Legacy: create + PDF in one step
    public async Task<Guid> CreateAndGeneratePdfAsync(CreateArztbriefDto dto, CancellationToken ct = default)
    {
        var id = await SaveDraftAsync(dto, ct);
        await GeneratePdfAsync(id, ct);
        await UpdateStatusAsync(id, ArztbriefStatus.Freigegeben, ct);
        return id;
    }

    private static string? FormatAddress(Patient p)
    {
        var parts = new[] { p.Street, $"{p.PostalCode} {p.City}".Trim() }
            .Where(s => !string.IsNullOrWhiteSpace(s));
        var result = string.Join(", ", parts);
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
