namespace MediPrax.Application.DTOs;

public class GdtDeviceConfig
{
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // EEG, EMG, NLG, Doppler, EP
    public string ImportDirectory { get; set; } = string.Empty;
    public string ExportDirectory { get; set; } = string.Empty;
    public string GdtVersion { get; set; } = "2.1";
    public string Encoding { get; set; } = "ISO-8859-1";
}

public class GdtDevicesOptions
{
    public const string SectionName = "GdtDevices";
    public List<GdtDeviceConfig> Devices { get; set; } = [];
}

public class GdtResultDto
{
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DateOnly PatientDob { get; set; }
    public DateTime ResultDate { get; set; }
    public List<string> ResultLines { get; set; } = [];
    public string? RawContent { get; set; }
}

public class GdtRequestDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateOnly PatientDob { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string? RequestNotes { get; set; }
}
