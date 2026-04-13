using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class UeberweisungPsychotherapieData
{
    // Patient
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;

    // Referral
    public string Diagnose { get; set; } = string.Empty;
    public string? DiagnoseIcd { get; set; }
    public string? Befunde { get; set; }
    public bool SomatischeAbklaerung { get; set; }
    public string? SomatischeAbklaerungDetails { get; set; }
    public string TherapieForm { get; set; } = string.Empty;
    public bool Dringlichkeit { get; set; }

    // Doctor / Practice
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
}

public class UeberweisungPsychotherapieDocument : IDocument
{
    private readonly UeberweisungPsychotherapieData _data;
    private static readonly string ThemeColor = Colors.Purple.Medium;
    private static readonly string ThemeLightBg = Colors.Purple.Lighten5;

    public UeberweisungPsychotherapieDocument(UeberweisungPsychotherapieData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Text("Überweisung zur Psychotherapie / Muster 7").Bold().FontSize(13);
                col.Item().PaddingVertical(4).LineHorizontal(1f).LineColor(ThemeColor);

                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Kasse: {_data.Krankenkasse}").FontSize(8);
                        c.Item().Text($"Vers.-Nr.: {_data.Versichertennummer}  KVNR: {_data.Kvnr}").FontSize(8);
                        c.Item().Text($"Patient: {_data.PatientName}, geb. {_data.PatientGeburtsdatum}").Bold();
                    });
                    row.ConstantItem(100).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Datum: {_data.Datum}").Bold();
                        if (_data.Dringlichkeit)
                            c.Item().PaddingTop(2).Text("DRINGEND").Bold().FontColor(Colors.Red.Medium).FontSize(10);
                    });
                });
            });

            page.Content().PaddingTop(12).Column(col =>
            {
                // Therapy form
                col.Item().Background(ThemeLightBg).Padding(8).Column(c =>
                {
                    c.Item().Text("Therapieverfahren:").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text(_data.TherapieForm).Bold().FontSize(11);
                });

                // Diagnosis
                col.Item().PaddingTop(10).Column(c =>
                {
                    c.Item().Text("Diagnose / Verdachtsdiagnose:").FontSize(8).FontColor(Colors.Grey.Medium);
                    var diagText = _data.Diagnose;
                    if (!string.IsNullOrEmpty(_data.DiagnoseIcd))
                        diagText = $"{_data.DiagnoseIcd} — {diagText}";
                    c.Item().Text(diagText).Bold();
                });

                // Somatic workup
                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Somatische Abklärung:").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Row(r =>
                    {
                        r.AutoItem().Text(_data.SomatischeAbklaerung ? "☑" : "☐").FontSize(11);
                        r.AutoItem().PaddingLeft(4).Text("Somatische Abklärung erfolgt").FontSize(9);
                    });
                    if (_data.SomatischeAbklaerung && !string.IsNullOrEmpty(_data.SomatischeAbklaerungDetails))
                        c.Item().PaddingTop(2).Text(_data.SomatischeAbklaerungDetails).FontSize(8);
                });

                // Findings
                if (!string.IsNullOrEmpty(_data.Befunde))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Relevante Befunde / Anamnese:").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.Befunde);
                    });
                }
            });

            page.Footer().Column(c =>
            {
                c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                c.Item().PaddingTop(4).Text($"{_data.ArztName} · {_data.PraxisName}").Bold().FontSize(8);
                c.Item().Text(_data.PraxisAdresse).FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }
}
