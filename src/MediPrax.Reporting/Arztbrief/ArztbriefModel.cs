namespace MediPrax.Reporting.Arztbrief;

public class ArztbriefModel
{
    // Praxis
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisSubtitle { get; set; } = "Gemeinschaftspraxis für Psychiatrie und Neurologie";
    public string PraxisAddress { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string PraxisPhone { get; set; } = "Tel: 0421-XXXXXXX";

    // Patient
    public string PatientName { get; set; } = string.Empty;
    public string PatientDateOfBirth { get; set; } = string.Empty;
    public string? PatientInsurance { get; set; }
    public string? PatientAddress { get; set; }

    // Empfänger
    public string RecipientName { get; set; } = string.Empty;
    public string? RecipientAddress { get; set; }

    // Inhalt
    public string Date { get; set; } = string.Empty;
    public string Subject { get; set; } = "Arztbrief";
    public List<string> Diagnoses { get; set; } = [];
    public string Body { get; set; } = string.Empty;

    // Arzt
    public string DoctorName { get; set; } = string.Empty;
}
