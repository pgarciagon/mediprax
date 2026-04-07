namespace MediPrax.Application.Interfaces;

public interface IImportService
{
    Task<ImportResultDto> ImportPatientsCsvAsync(Stream csvStream, CancellationToken ct = default);
}

public class ImportResultDto
{
    public int Imported { get; set; }
    public int Skipped { get; set; }
    public int Errors { get; set; }
    public IReadOnlyList<string> ErrorMessages { get; set; } = [];
}
