namespace MediPrax.Application.Interfaces;

public interface IKvdtExportService
{
    Task<KvdtExportResultDto> ExportQuarterAsync(string quarter, CancellationToken ct = default);
}

public class KvdtExportResultDto
{
    public bool Success { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public int PatientCount { get; set; }
    public int ItemCount { get; set; }
    public string? ErrorMessage { get; set; }
}
