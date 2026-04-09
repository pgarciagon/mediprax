using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class KrankenhauseinweisungData
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
    public string Krankenhaus { get; set; } = string.Empty;
    public string Fachabteilung { get; set; } = string.Empty;
    public string Einweisungsdiagnose { get; set; } = string.Empty;
    public string? DiagnoseIcd { get; set; }
    public string? Befunde { get; set; }
    public bool IsNotfall { get; set; }
}

public class KrankenhauseinweisungDocument : IDocument
{
    private readonly KrankenhauseinweisungData _data;
    public KrankenhauseinweisungDocument(KrankenhauseinweisungData data) => _data = data;

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
                        c.Item().Text("Verordnung von Krankenhausbehandlung").Bold().FontSize(13);
                        c.Item().Text("Muster 2 (KBV)").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(120).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Datum: {_data.Datum}").Bold();
                        if (_data.IsNotfall)
                            c.Item().Background(Colors.Red.Medium).Padding(4).Text("NOTFALL").Bold().FontSize(10).FontColor(Colors.White);
                    });
                });
                col.Item().PaddingTop(6).LineHorizontal(1f).LineColor(Colors.Blue.Medium);
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

                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Krankenhaus / Abteilung").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Row(row =>
                    {
                        row.RelativeItem().Column(p =>
                        {
                            p.Item().Text("Krankenhaus:").FontSize(8).FontColor(Colors.Grey.Medium);
                            p.Item().Text(_data.Krankenhaus).Bold();
                        });
                        row.RelativeItem().Column(p =>
                        {
                            p.Item().Text("Fachabteilung:").FontSize(8).FontColor(Colors.Grey.Medium);
                            p.Item().Text(_data.Fachabteilung).Bold();
                        });
                    });
                });

                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Einweisungsdiagnose").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Einweisungsdiagnose);
                    if (!string.IsNullOrEmpty(_data.DiagnoseIcd))
                        c.Item().Text($"ICD-10: {_data.DiagnoseIcd}").FontColor(Colors.Grey.Medium);
                });

                if (!string.IsNullOrEmpty(_data.Befunde))
                {
                    col.Item().PaddingTop(12).Column(c =>
                    {
                        c.Item().Text("Befunde / Begründung").Bold().FontSize(10);
                        c.Item().PaddingTop(4).Text(_data.Befunde);
                    });
                }

                col.Item().PaddingTop(12).Row(row =>
                {
                    row.ConstantItem(20).Text(_data.IsNotfall ? "☒" : "☐").FontSize(12);
                    row.RelativeItem().Text("Notfalleinweisung").Bold();
                });

                col.Item().PaddingTop(30).Column(c =>
                {
                    c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(4).Text("Datum, Stempel und Unterschrift des einweisenden Arztes").FontSize(8).FontColor(Colors.Grey.Medium);
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
