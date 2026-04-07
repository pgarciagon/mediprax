using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class AuData
{
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;
    public string Diagnose { get; set; } = string.Empty;
    public string? IcdCode { get; set; }
    public DateOnly VonDatum { get; set; }
    public DateOnly BisDatum { get; set; }
    public bool Erstbescheinigung { get; set; } = true;
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
}

public class AuDocument : IDocument
{
    private readonly AuData _data;
    public AuDocument(AuData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Text("Arbeitsunfähigkeitsbescheinigung").Bold().FontSize(13);
                col.Item().Text("Muster 1").FontSize(8).FontColor(Colors.Grey.Medium);
                col.Item().PaddingVertical(4).LineHorizontal(1f).LineColor(Colors.Red.Medium);

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

            page.Content().PaddingTop(15).Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Background(_data.Erstbescheinigung ? Colors.Red.Lighten5 : Colors.Orange.Lighten5).Padding(8).Column(c =>
                    {
                        c.Item().Text(_data.Erstbescheinigung ? "☒ Erstbescheinigung" : "☒ Folgebescheinigung").Bold().FontSize(10);
                    });
                });

                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Arbeitsunfähig:").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"von {_data.VonDatum:dd.MM.yyyy}  bis voraussichtlich {_data.BisDatum:dd.MM.yyyy}").Bold().FontSize(12);
                });

                col.Item().PaddingTop(12).Column(c =>
                {
                    c.Item().Text("Diagnose (nur Ausfertigung für Arzt/Krankenkasse):").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"{_data.IcdCode ?? ""} — {_data.Diagnose}").Bold();
                });

                col.Item().PaddingTop(20).Column(c =>
                {
                    c.Item().Text("Stempel / Unterschrift des Arztes:").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(20).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                });
            });

            page.Footer().Column(c =>
            {
                c.Item().Text($"{_data.ArztName} · {_data.PraxisName}").Bold().FontSize(8);
                c.Item().Text(_data.PraxisAdresse).FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }
}
