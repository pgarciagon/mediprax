using MediPrax.Application.DTOs;

namespace MediPrax.Application.Services;

/// <summary>
/// Parses GDT 2.1 format files (Gerätedatentransfer).
/// Fixed-width text format with field identifiers (Feldkennungen).
/// </summary>
public static class GdtParser
{
    // GDT field identifiers
    private const string FkPatientId = "3000";
    private const string FkPatientName = "3101";
    private const string FkPatientFirstName = "3102";
    private const string FkPatientDob = "3103";
    private const string FkResultText = "6200";
    private const string FkResultDate = "6201";
    private const string FkDeviceIdent = "6302";

    /// <summary>
    /// Parses a GDT response file (Satzart 6311) into a result DTO.
    /// </summary>
    public static GdtResultDto Parse(string content)
    {
        var result = new GdtResultDto { RawContent = content };
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            if (line.Length < 7) continue;

            // GDT format: 3-digit length + 4-digit field ID + content
            var fieldId = line.Substring(3, 4);
            var value = line.Length > 7 ? line[7..].Trim() : string.Empty;

            switch (fieldId)
            {
                case FkPatientId:
                    result.PatientId = value;
                    break;
                case FkPatientName:
                    result.PatientName = value;
                    break;
                case FkPatientFirstName:
                    result.PatientName = $"{value} {result.PatientName}".Trim();
                    break;
                case FkPatientDob:
                    if (DateOnly.TryParseExact(value, "ddMMyyyy", out var dob))
                        result.PatientDob = dob;
                    break;
                case FkResultDate:
                    if (DateTime.TryParseExact(value, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                        result.ResultDate = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    break;
                case FkResultText:
                    result.ResultLines.Add(value);
                    break;
                case FkDeviceIdent:
                    result.DeviceName = value;
                    break;
            }
        }

        if (result.ResultDate == default)
            result.ResultDate = DateTime.UtcNow;

        return result;
    }

    /// <summary>
    /// Validates whether the content looks like a valid GDT file.
    /// </summary>
    public static bool IsValidGdt(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return false;
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 3) return false;

        // First line should have Satzart field (8000)
        return lines[0].Length >= 7 && lines[0].Substring(3, 4) == "8000";
    }
}
