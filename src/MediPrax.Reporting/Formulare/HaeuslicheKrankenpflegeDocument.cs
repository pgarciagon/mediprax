using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class HaeuslicheKrankenpflegeData
{
    public string PatientName { get; set; } = string.Empty;
    public string Geburtsdatum { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
    public string Diagnose { get; set; } = string.Empty;
    public string? DiagnoseIcd { get; set; }
    public string? Begruendung { get; set; }
    public string VonDatum { get; set; } = string.Empty;
    public string BisDatum { get; set; } = string.Empty;
    public string Leistungen { get; set; } = string.Empty; // e.g. "Medikamentengabe, Verbandswechsel"
    public string? Frequenz { get; set; } // e.g. "täglich"
    public bool IsPsychiatrisch { get; set; }
}

public class HaeuslicheKrankenpflegeDocument : IDocument
{
    private readonly HaeuslicheKrankenpflegeData _data;
    public HaeuslicheKrankenpflegeDocument(HaeuslicheKrankenpflegeData data) => _data = data;

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
                        c.Item().Text("Verordnung häuslicher Krankenpflege").Bold().FontSize(13);
                        c.Item().Text("Muster 12 (KBV)").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(100).AlignRight().Text($"Datum: {_data.Datum}").Bold();
                });
                col.Item().PaddingTop(6).LineHorizontal(1f).LineColor(Colors.Orange.Medium);
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
                            if (!string.IsNullOrEmpty(_data.Adresse))
                                p.Item().Text($"Adresse: {_data.Adresse}");
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
                    c.Item().Text("Diagnose").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Diagnose);
                    if (!string.IsNullOrEmpty(_data.DiagnoseIcd))
                        c.Item().Text($"ICD-10: {_data.DiagnoseIcd}").FontColor(Colors.Grey.Medium);
                });

                if (!string.IsNullOrEmpty(_data.Begruendung))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Begründung der häuslichen Krankenpflege:").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.Begruendung);
                    });
                }

                // Period
                col.Item().PaddingTop(12).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Verordnungszeitraum").Bold().FontSize(10);
                        c.Item().PaddingTop(4).Text($"Von {_data.VonDatum}  bis  {_data.BisDatum}").Bold().FontSize(11);
                    });
                });

                // Services
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Verordnete Leistungen").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Leistungen);
                    if (!string.IsNullOrEmpty(_data.Frequenz))
                    {
                        c.Item().PaddingTop(4).Row(row =>
                        {
                            row.ConstantItem(70).Text("Frequenz:").FontSize(8).FontColor(Colors.Grey.Medium);
                            row.RelativeItem().Text(_data.Frequenz).Bold();
                        });
                    }
                });

                // Psychiatric checkbox
                col.Item().PaddingTop(12).Row(row =>
                {
                    row.ConstantItem(18).Text(_data.IsPsychiatrisch ? "☒" : "☐").FontSize(11);
                    row.RelativeItem().Text("Psychiatrische häusliche Krankenpflege (§ 37 Abs. 2 SGB V)").Bold();
                });

                col.Item().PaddingTop(30).Column(c =>
                {
                    c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(4).Text("Datum, Stempel und Unterschrift des Arztes").FontSize(8).FontColor(Colors.Grey.Medium);
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
