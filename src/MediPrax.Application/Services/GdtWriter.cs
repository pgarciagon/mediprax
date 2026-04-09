using System.Text;
using MediPrax.Application.DTOs;

namespace MediPrax.Application.Services;

/// <summary>
/// Writes GDT 2.1 request files (Satzart 6310) for sending patient data to devices.
/// </summary>
public static class GdtWriter
{
    /// <summary>
    /// Creates a GDT request file content for a device examination.
    /// </summary>
    public static string WriteRequest(GdtRequestDto request)
    {
        var sb = new StringBuilder();

        // Satzart 6310 = Daten einer Untersuchung anfordern
        AppendField(sb, "8000", "6310");
        // Satzlänge — placeholder, will be calculated
        AppendField(sb, "8100", "00000");
        // Patient ID
        AppendField(sb, "3000", request.PatientId.ToString("N")[..10]);
        // Patient last name (extract from full name)
        var nameParts = request.PatientName.Split(',', ' ');
        if (nameParts.Length >= 2)
        {
            AppendField(sb, "3101", nameParts[0].Trim());
            AppendField(sb, "3102", nameParts[1].Trim());
        }
        else
        {
            AppendField(sb, "3101", request.PatientName);
        }
        // Date of birth (ddMMyyyy)
        AppendField(sb, "3103", request.PatientDob.ToString("ddMMyyyy"));
        // Request date
        AppendField(sb, "6200", DateTime.Today.ToString("ddMMyyyy"));
        // Device type
        AppendField(sb, "6302", request.DeviceType);
        // Notes
        if (!string.IsNullOrEmpty(request.RequestNotes))
            AppendField(sb, "6220", request.RequestNotes);

        return sb.ToString();
    }

    private static void AppendField(StringBuilder sb, string fieldId, string value)
    {
        // GDT format: 3-digit length (including \r\n) + 4-digit field ID + value + \r\n
        var content = fieldId + value;
        var length = content.Length + 5; // 3 for length digits + 2 for \r\n
        sb.AppendLine($"{length:000}{content}");
    }
}
