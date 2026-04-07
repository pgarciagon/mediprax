using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class RezeptData
{
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string PatientAdresse { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
    public bool IsPrivat { get; set; }
    public List<RezeptPosition> Positionen { get; set; } = [];
}

public class RezeptPosition
{
    public string Medikament { get; set; } = string.Empty;
    public string? Pzn { get; set; }
    public string? Dosierung { get; set; }
    public int Menge { get; set; } = 1;
    public bool AutIdem { get; set; } = true;
}

public class RezeptDocument : IDocument
{
    private readonly RezeptData _data;
    public RezeptDocument(RezeptData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_data.IsPrivat ? "Privatrezept" : "Kassenrezept").Bold().FontSize(12);
                        c.Item().Text($"Krankenkasse: {_data.Krankenkasse}").FontSize(8);
                        c.Item().Text($"Vers.-Nr.: {_data.Versichertennummer}  KVNR: {_data.Kvnr}").FontSize(8);
                    });
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text($"Datum: {_data.Datum}").Bold();
                    });
                });

                col.Item().PaddingVertical(6).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);

                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Patient: {_data.PatientName}").Bold();
                        c.Item().Text($"geb. {_data.PatientGeburtsdatum}").FontSize(8);
                        c.Item().Text(_data.PatientAdresse).FontSize(8);
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
            });

            page.Content().PaddingTop(10).Column(col =>
            {
                col.Item().Text("Rp.").Bold().FontSize(11);

                foreach (var pos in _data.Positionen)
                {
                    col.Item().PaddingTop(6).Row(row =>
                    {
                        row.ConstantItem(20).Text(pos.AutIdem ? "☐" : "☒").FontSize(10);
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"{pos.Medikament}  ×{pos.Menge}").Bold();
                            if (!string.IsNullOrEmpty(pos.Dosierung))
                                c.Item().Text($"Dos.: {pos.Dosierung}").FontSize(8);
                            if (!string.IsNullOrEmpty(pos.Pzn))
                                c.Item().Text($"PZN: {pos.Pzn}").FontSize(7).FontColor(Colors.Grey.Medium);
                        });
                    });
                }
            });

            page.Footer().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(4).Text($"{_data.ArztName}").Bold().FontSize(8);
                    c.Item().Text($"{_data.PraxisName} · {_data.PraxisAdresse}").FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }
}
