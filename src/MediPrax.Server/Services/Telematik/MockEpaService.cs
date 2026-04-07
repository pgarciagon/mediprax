using MediPrax.Application.Interfaces.Telematik;

namespace MediPrax.Server.Services.Telematik;

public class MockEpaService : IEpaService
{
    public async Task<EpaConsentDto> CheckConsentAsync(string kvnr, CancellationToken ct = default)
    {
        await Task.Delay(400, ct);
        return new EpaConsentDto
        {
            HasConsent = true,
            Kvnr = kvnr,
            ConsentGrantedAt = DateTime.UtcNow.AddMonths(-6),
            ConsentScope = "Lesen und Schreiben"
        };
    }

    public async Task<IReadOnlyList<EpaDocumentDto>> GetDocumentsAsync(string kvnr, CancellationToken ct = default)
    {
        await Task.Delay(500, ct);
        return
        [
            new() { DocumentId = "epa-001", Title = "Entlassbrief Klinikum Bremen-Mitte", DocumentType = "Arztbrief", AuthorName = "Dr. Weber", AuthorOrganization = "Klinikum Bremen-Mitte", CreatedAt = DateTime.UtcNow.AddMonths(-3) },
            new() { DocumentId = "epa-002", Title = "MRT Schädel", DocumentType = "Befundbericht", AuthorName = "Dr. Weber", AuthorOrganization = "Klinikum Bremen-Mitte", CreatedAt = DateTime.UtcNow.AddMonths(-2) },
            new() { DocumentId = "epa-003", Title = "Laborbefund 2026-01", DocumentType = "Laborbefund", AuthorName = "Labor Bremen", AuthorOrganization = "Labor Bremen GmbH", CreatedAt = DateTime.UtcNow.AddMonths(-1) },
        ];
    }

    public async Task<EpaUploadResultDto> UploadDocumentAsync(string kvnr, EpaUploadRequestDto request, CancellationToken ct = default)
    {
        await Task.Delay(600, ct);
        return new EpaUploadResultDto
        {
            Success = true,
            DocumentId = $"epa-{Guid.NewGuid():N}"[..12]
        };
    }
}
