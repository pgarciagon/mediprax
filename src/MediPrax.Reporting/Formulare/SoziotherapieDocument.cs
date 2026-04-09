using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class SoziotherapieData
{
    public string PatientName { get; set; } = string.Empty;
    public string Geburtsdatum { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
    public string Diagnose { get; set; } = string.Empty; // typically F20-F31
    public string? DiagnoseIcd { get; set; }
    public string Faehigkeitsstoerungen { get; set; } = string.Empty;
    public string Therapieziele { get; set; } = string.Empty;
    public int VerordneteStunden { get; set; } = 30; // max 120 Stunden / 3 Jahre
    public string? Soziotherapeut { get; set; }
    public bool IsErstverordnung { get; set; } = true;
}

public class SoziotherapieDocument : IDocument
{
    private readonly SoziotherapieData _data;
    public SoziotherapieDocument(SoziotherapieData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(25);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Soziotherapie — Verordnung").Bold().FontSize(13);
                        c.Item().Text("Muster 26 (KBV) · § 37a SGB V").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(100).AlignRight().Text($"Datum: {_data.Datum}").Bold();
                });
                col.Item().PaddingTop(6).LineHorizontal(1f).LineColor(Colors.Purple.Medium);
            });

            page.Content().PaddingTop(12).Column(col =>
            {
                // Patient section
                col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                {
                    c.Item().Text("Patientendaten").Bold().FontSize(10);
                    c.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Column(p =>
                        {
                            p.Item().Text($"Name: {_data.PatientName}").Bold();
                            p.Item().Text($"Geburtsdatum: {_data.Geburtsdatum}");
                        });
                        row.RelativeItem().Column(p =>
                        {
                            p.Item().Text($"Krankenkasse: {_data.Krankenkasse}");
                            p.Item().Text($"Vers.-Nr.: {_data.Versichertennummer}");
                            p.Item().Text($"KVNR: {_data.Kvnr}");
                        });
                    });
                });

                // Diagnosis
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Psychiatrische Diagnose").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Diagnose).Bold();
                    if (!string.IsNullOrEmpty(_data.DiagnoseIcd))
                        c.Item().Text($"ICD-10: {_data.DiagnoseIcd}").FontColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(4).Text("Soziotherapie ist indiziert bei schweren psychischen Erkrankungen (typisch: F20–F31).").FontSize(8).FontColor(Colors.Grey.Medium);
                });

                // Functional impairments
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Fähigkeitsstörungen").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Faehigkeitsstoerungen);
                });

                // Goals
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Therapieziele").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Therapieziele);
                });

                // Hours and provider
                col.Item().PaddingTop(12).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Verordnete Stunden").Bold().FontSize(10);
                        c.Item().PaddingTop(4).Text($"{_data.VerordneteStunden} Stunden").Bold().FontSize(14);
                        c.Item().Text("(max. 120 Stunden / 3 Jahre)").FontSize(7).FontColor(Colors.Grey.Medium);
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Soziotherapeut / Leistungserbringer").Bold().FontSize(10);
                        c.Item().PaddingTop(4).Text(_data.Soziotherapeut ?? "—");
                    });
                });

                // First / follow-up
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Verordnungsart").Bold().FontSize(10);
                    c.Item().PaddingTop(6).Row(row =>
                    {
                        row.ConstantItem(18).Text(_data.IsErstverordnung ? "☒" : "☐").FontSize(11);
                        row.RelativeItem().Text("Erstverordnung");
                    });
                    c.Item().PaddingTop(3).Row(row =>
                    {
                        row.ConstantItem(18).Text(!_data.IsErstverordnung ? "☒" : "☐").FontSize(11);
                        row.RelativeItem().Text("Folgeverordnung");
                    });
                });

                col.Item().PaddingTop(30).Column(c =>
                {
                    c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(4).Text("Datum, Stempel und Unterschrift des Facharztes für Psychiatrie").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            page.Footer().Column(c =>
            {
                c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                c.Item().PaddingTop(4).Text($"{_data.ArztName} · {_data.PraxisName}").Bold().FontSize(8);
                c.Item().Text(_data.PraxisAdresse).FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }
}
