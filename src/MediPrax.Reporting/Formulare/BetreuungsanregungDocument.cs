using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class BetreuungsanregungData
{
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string PatientAdresse { get; set; } = string.Empty;
    public string Diagnose { get; set; } = string.Empty;
    public string BegruendungBetreuungsbedarf { get; set; } = string.Empty;
    public string VorgeschlagenerBetreuer { get; set; } = string.Empty;
    public string BetreuungsAufgaben { get; set; } = string.Empty;
    public string ZusaetzlicheHinweise { get; set; } = string.Empty;
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
}

public class BetreuungsanregungDocument : IDocument
{
    private readonly BetreuungsanregungData _data;
    public BetreuungsanregungDocument(BetreuungsanregungData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(35);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_data.PraxisName).Bold();
                        c.Item().Text(_data.PraxisAdresse).FontSize(9);
                        c.Item().PaddingTop(4).Text(_data.ArztName).FontSize(9);
                    });
                    row.ConstantItem(150).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Datum: {_data.Datum}").Bold();
                    });
                });

                col.Item().PaddingTop(8).Text("An das").FontSize(10);
                col.Item().Text("Amtsgericht — Betreuungsgericht").Bold().FontSize(10);
                col.Item().PaddingVertical(8).LineHorizontal(1f).LineColor(Colors.Black);
                col.Item().Text("Anregung zur Einrichtung einer rechtlichen Betreuung").Bold().FontSize(13);
            });

            page.Content().PaddingTop(15).Column(col =>
            {
                col.Item().PaddingBottom(6).Text("Sehr geehrte Damen und Herren,");
                col.Item().PaddingBottom(10).Text("ich rege die Einrichtung einer rechtlichen Betreuung für folgende Person an:");

                col.Item().Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                {
                    c.Item().Text("Betroffene Person:").Bold();
                    c.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text($"Name: {_data.PatientName}");
                        r.RelativeItem().Text($"Geb.: {_data.PatientGeburtsdatum}");
                    });
                    c.Item().Text($"Adresse: {_data.PatientAdresse}");
                });

                col.Item().PaddingTop(10).Column(c =>
                {
                    c.Item().Text("Diagnose:").Bold();
                    c.Item().PaddingTop(4).Text(_data.Diagnose);
                });

                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Begründung des Betreuungsbedarfs:").Bold();
                    c.Item().PaddingTop(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(6).Text(_data.BegruendungBetreuungsbedarf);
                });

                if (!string.IsNullOrWhiteSpace(_data.BetreuungsAufgaben))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Vorgeschlagene Aufgabenkreise:").Bold();
                        c.Item().PaddingTop(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                            .Padding(6).Text(_data.BetreuungsAufgaben);
                    });
                }

                if (!string.IsNullOrWhiteSpace(_data.VorgeschlagenerBetreuer))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Vorschlag für den Betreuer:").Bold();
                        c.Item().PaddingTop(4).Text(_data.VorgeschlagenerBetreuer);
                    });
                }

                if (!string.IsNullOrWhiteSpace(_data.ZusaetzlicheHinweise))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("Zusätzliche Hinweise:").Bold();
                        c.Item().PaddingTop(4).Text(_data.ZusaetzlicheHinweise);
                    });
                }

                col.Item().PaddingTop(20).Text("Mit freundlichen Grüßen");
                col.Item().PaddingTop(30).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f);
                        c.Item().PaddingTop(4).Text($"{_data.ArztName}").FontSize(9);
                        c.Item().Text(_data.PraxisName).FontSize(9);
                    });
                });
            });
        });
    }
}
