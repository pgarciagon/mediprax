namespace MediPrax.Application.Interfaces.Telematik;

/// <summary>
/// ePA — elektronische Patientenakte.
/// Read/write patient records to the central ePA system via FHIR.
/// In production: communicates with ePA-Aktensystem via FHIR REST API.
/// </summary>
public interface IEpaService
{
    Task<EpaConsentDto> CheckConsentAsync(string kvnr, CancellationToken ct = default);
    Task<IReadOnlyList<EpaDocumentDto>> GetDocumentsAsync(string kvnr, CancellationToken ct = default);
    Task<EpaUploadResultDto> UploadDocumentAsync(string kvnr, EpaUploadRequestDto request, CancellationToken ct = default);
}

public class EpaConsentDto
{
    public bool HasConsent { get; set; }
    public string Kvnr { get; set; } = string.Empty;
    public DateTime? ConsentGrantedAt { get; set; }
    public string? ConsentScope { get; set; }
}

public class EpaDocumentDto
{
    public string DocumentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string? AuthorOrganization { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MimeType { get; set; } = "application/pdf";
}

public class EpaUploadRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string MimeType { get; set; } = "application/pdf";
}

public class EpaUploadResultDto
{
    public bool Success { get; set; }
    public string? DocumentId { get; set; }
    public string? ErrorMessage { get; set; }
}
