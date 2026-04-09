using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class HeilmittelverordnungData
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
    public string HeilmittelTyp { get; set; } = string.Empty; // Ergotherapie, Physiotherapie, Logopädie
    public string Diagnose { get; set; } = string.Empty;
    public string? DiagnoseIcd { get; set; }
    public string? Leitsymptomatik { get; set; }
    public string? Therapieziel { get; set; }
    public int AnzahlSitzungen { get; set; } = 10;
    public string? Frequenz { get; set; } // z.B. "2x wöchentlich"
    public bool IsErstverordnung { get; set; } = true;
    public bool IsHausbesuch { get; set; }
    public bool IsLangfristig { get; set; }
}

public class HeilmittelverordnungDocument : IDocument
{
    private readonly HeilmittelverordnungData _data;
    public HeilmittelverordnungDocument(HeilmittelverordnungData data) => _data = data;

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
                        c.Item().Text("Heilmittelverordnung").Bold().FontSize(13);
                        c.Item().Text("Muster 13 (KBV)").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(100).AlignRight().Text($"Datum: {_data.Datum}").Bold();
                });
                col.Item().PaddingTop(6).LineHorizontal(1f).LineColor(Colors.Green.Medium);
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

                // Heilmittel type
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Heilmittel").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Background(Colors.Green.Lighten5).Padding(8).Text(_data.HeilmittelTyp).Bold().FontSize(11);
                });

                // Diagnosis
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Diagnose").Bold().FontSize(10);
                    c.Item().PaddingTop(4).Text(_data.Diagnose);
                    if (!string.IsNullOrEmpty(_data.DiagnoseIcd))
                        c.Item().Text($"ICD-10: {_data.DiagnoseIcd}").FontColor(Colors.Grey.Medium);
                });

                if (!string.IsNullOrEmpty(_data.Leitsymptomatik))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Leitsymptomatik:").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.Leitsymptomatik);
                    });
                }

                if (!string.IsNullOrEmpty(_data.Therapieziel))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Therapieziel:").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.Therapieziel);
                    });
                }

                // Sessions and frequency
                col.Item().PaddingTop(12).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Anzahl Sitzungen").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text($"{_data.AnzahlSitzungen}").Bold().FontSize(14);
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Frequenz").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.Frequenz ?? "nach Bedarf").Bold();
                    });
                });

                // Checkboxes
                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Optionen").Bold().FontSize(10);
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
                    c.Item().PaddingTop(3).Row(row =>
                    {
                        row.ConstantItem(18).Text(_data.IsHausbesuch ? "☒" : "☐").FontSize(11);
                        row.RelativeItem().Text("Hausbesuch");
                    });
                    c.Item().PaddingTop(3).Row(row =>
                    {
                        row.ConstantItem(18).Text(_data.IsLangfristig ? "☒" : "☐").FontSize(11);
                        row.RelativeItem().Text("Langfristiger Heilmittelbedarf");
                    });
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
