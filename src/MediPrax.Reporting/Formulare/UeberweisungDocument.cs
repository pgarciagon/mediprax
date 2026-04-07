using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class UeberweisungData
{
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;
    public string AnFachrichtung { get; set; } = string.Empty;
    public string? AnArzt { get; set; }
    public string Diagnose { get; set; } = string.Empty;
    public string? Fragestellung { get; set; }
    public string? Befunde { get; set; }
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
}

public class UeberweisungDocument : IDocument
{
    private readonly UeberweisungData _data;
    public UeberweisungDocument(UeberweisungData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Text("Überweisung / Muster 6").Bold().FontSize(13);
                col.Item().PaddingVertical(4).LineHorizontal(1f).LineColor(Colors.Blue.Medium);

                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Kasse: {_data.Krankenkasse}").FontSize(8);
                        c.Item().Text($"Vers.-Nr.: {_data.Versichertennummer}  KVNR: {_data.Kvnr}").FontSize(8);
                        c.Item().Text($"Patient: {_data.PatientName}, geb. {_data.PatientGeburtsdatum}").Bold();
                    });
                    row.ConstantItem(100).AlignRight().Text($"Datum: {_data.Datum}").Bold();
                });
            });

            page.Content().PaddingTop(12).Column(col =>
            {
                col.Item().Background(Colors.Blue.Lighten5).Padding(8).Column(c =>
                {
                    c.Item().Text("Überwiesen an:").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"{_data.AnFachrichtung}").Bold().FontSize(11);
                    if (!string.IsNullOrEmpty(_data.AnArzt))
                        c.Item().Text(_data.AnArzt).FontSize(9);
                });

                col.Item().PaddingTop(10).Column(c =>
                {
                    c.Item().Text("Diagnose / Verdachtsdiagnose:").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text(_data.Diagnose).Bold();
                });

                if (!string.IsNullOrEmpty(_data.Fragestellung))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Auftrag / Fragestellung:").FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.Fragestellung);
                    });
                }

                if (!string.IsNullOrEmpty(_data.Befunde))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Mitgegebene Befunde:").FontSize(8).FontColor(Colors.Grey.Medium);
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
