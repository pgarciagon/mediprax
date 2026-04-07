namespace MediPrax.Application.Interfaces;

public interface IReportService
{
    Task<DailyReportDto> GetDailyReportAsync(DateOnly date, CancellationToken ct = default);
    Task<QuarterlyReportDto> GetQuarterlyReportAsync(string quarter, CancellationToken ct = default);
}

public class DailyReportDto
{
    public DateOnly Date { get; set; }
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int EncountersCreated { get; set; }
    public int DocumentsCreated { get; set; }
    public IReadOnlyList<DoctorDayDto> DoctorBreakdown { get; set; } = [];
}

public class DoctorDayDto
{
    public string DoctorName { get; set; } = string.Empty;
    public int Appointments { get; set; }
    public int Encounters { get; set; }
}

public class QuarterlyReportDto
{
    public string Quarter { get; set; } = string.Empty;
    public int TotalPatientsSeen { get; set; }
    public int TotalEncounters { get; set; }
    public int TotalDocuments { get; set; }
    public int NewPatients { get; set; }
    public int BillingItems { get; set; }
    public decimal EstimatedRevenue { get; set; }
    public IReadOnlyList<TopDiagnosisDto> TopDiagnoses { get; set; } = [];
    public IReadOnlyList<DoctorQuarterDto> DoctorBreakdown { get; set; } = [];
}

public class TopDiagnosisDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Count { get; set; }
}

public class DoctorQuarterDto
{
    public string DoctorName { get; set; } = string.Empty;
    public int Encounters { get; set; }
    public int BillingItems { get; set; }
}
