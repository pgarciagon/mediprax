using MediPrax.Application.DTOs;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public interface IDmpService
{
    Task<DmpEnrollmentDto> EnrollAsync(CreateDmpEnrollmentDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<DmpEnrollmentDto>> GetEnrollmentsAsync(Guid? patientId = null, CancellationToken ct = default);
    Task<DmpDocumentationDto> CreateDocumentationAsync(CreateDmpDocumentationDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<DmpDocumentationDto>> GetDocumentationsAsync(Guid enrollmentId, CancellationToken ct = default);
    Task SubmitDocumentationAsync(Guid documentationId, CancellationToken ct = default);
}

public class DmpService(DbContext context) : IDmpService
{
    private DbSet<DmpEnrollment> Enrollments => context.Set<DmpEnrollment>();
    private DbSet<DmpDocumentation> Documentations => context.Set<DmpDocumentation>();

    public async Task<DmpEnrollmentDto> EnrollAsync(CreateDmpEnrollmentDto dto, CancellationToken ct = default)
    {
        var enrollment = new DmpEnrollment
        {
            PatientId = dto.PatientId,
            DmpType = dto.DmpType,
            EnrollmentDate = dto.EnrollmentDate,
            Status = DmpStatus.Active
        };
        Enrollments.Add(enrollment);
        await context.SaveChangesAsync(ct);

        var created = await Enrollments.Include(e => e.Patient)
            .FirstAsync(e => e.Id == enrollment.Id, ct);
        return MapToDto(created);
    }

    public async Task<IReadOnlyList<DmpEnrollmentDto>> GetEnrollmentsAsync(Guid? patientId = null, CancellationToken ct = default)
    {
        var query = Enrollments.Include(e => e.Patient).AsQueryable();
        if (patientId.HasValue)
            query = query.Where(e => e.PatientId == patientId.Value);
        return await query.OrderByDescending(e => e.EnrollmentDate)
            .Select(e => MapToDto(e))
            .ToListAsync(ct);
    }

    public async Task<DmpDocumentationDto> CreateDocumentationAsync(CreateDmpDocumentationDto dto, CancellationToken ct = default)
    {
        var doc = new DmpDocumentation
        {
            DmpEnrollmentId = dto.DmpEnrollmentId,
            DocumentationDate = dto.DocumentationDate,
            Quarter = dto.Quarter,
            FormData = dto.FormData
        };
        Documentations.Add(doc);
        await context.SaveChangesAsync(ct);
        return MapDocToDto(doc);
    }

    public async Task<IReadOnlyList<DmpDocumentationDto>> GetDocumentationsAsync(Guid enrollmentId, CancellationToken ct = default)
    {
        return await Documentations
            .Where(d => d.DmpEnrollmentId == enrollmentId)
            .OrderByDescending(d => d.DocumentationDate)
            .Select(d => MapDocToDto(d))
            .ToListAsync(ct);
    }

    public async Task SubmitDocumentationAsync(Guid documentationId, CancellationToken ct = default)
    {
        var doc = await Documentations.FindAsync([documentationId], ct)
            ?? throw new KeyNotFoundException("DMP-Dokumentation nicht gefunden.");
        doc.Submitted = true;
        doc.SubmittedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    private static DmpEnrollmentDto MapToDto(DmpEnrollment e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        PatientName = e.Patient?.FullName ?? string.Empty,
        DmpType = e.DmpType,
        EnrollmentDate = e.EnrollmentDate,
        DisenrollmentDate = e.DisenrollmentDate,
        Status = e.Status
    };

    private static DmpDocumentationDto MapDocToDto(DmpDocumentation d) => new()
    {
        Id = d.Id,
        DmpEnrollmentId = d.DmpEnrollmentId,
        DocumentationDate = d.DocumentationDate,
        Quarter = d.Quarter,
        FormData = d.FormData,
        Submitted = d.Submitted,
        SubmittedAt = d.SubmittedAt
    };
}
