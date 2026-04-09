using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class PsychKGZeugnisData
{
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string PatientAdresse { get; set; } = string.Empty;
    public string UntersuchungsDatum { get; set; } = string.Empty;
    public string UntersuchungsZeit { get; set; } = string.Empty;
    public string PsychopathologischeBefunde { get; set; } = string.Empty;
    public string Selbstgefaehrdung { get; set; } = string.Empty;
    public string Fremdgefaehrdung { get; set; } = string.Empty;
    public string Diagnose { get; set; } = string.Empty;
    public string Empfehlung { get; set; } = string.Empty;
    public string VorgeschlageneDauer { get; set; } = string.Empty;
    public string ArztName { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string Datum { get; set; } = string.Empty;
}

public class PsychKGZeugnisDocument : IDocument
{
    private readonly PsychKGZeugnisData _data;
    public PsychKGZeugnisDocument(PsychKGZeugnisData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(35);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Text("Ärztliches Zeugnis gemäß PsychKG").Bold().FontSize(14);
                col.Item().Text("zur Unterbringung psychisch Kranker").FontSize(11);
                col.Item().PaddingVertical(4).LineHorizontal(1.5f).LineColor(Colors.Black);

                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"{_data.PraxisName}").Bold();
                        c.Item().Text(_data.PraxisAdresse).FontSize(9);
                    });
                    row.ConstantItem(150).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Datum: {_data.Datum}").Bold();
                        c.Item().Text(_data.ArztName).FontSize(9);
                    });
                });
            });

            page.Content().PaddingTop(15).Column(col =>
            {
                // Patient data
                col.Item().Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                {
                    c.Item().Text("Patientendaten").Bold().FontSize(11);
                    c.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text($"Name: {_data.PatientName}");
                        r.RelativeItem().Text($"Geb.: {_data.PatientGeburtsdatum}");
                    });
                    c.Item().Text($"Adresse: {_data.PatientAdresse}");
                });

                col.Item().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Text($"Untersuchungsdatum: {_data.UntersuchungsDatum}");
                    row.RelativeItem().Text($"Uhrzeit: {_data.UntersuchungsZeit}");
                });

                col.Item().PaddingTop(10).Column(c =>
                {
                    c.Item().Text("Psychopathologische Befunde:").Bold();
                    c.Item().PaddingTop(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(6).Text(_data.PsychopathologischeBefunde);
                });

                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Selbstgefährdung:").Bold();
                    c.Item().PaddingTop(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(6).Text(_data.Selbstgefaehrdung);
                });

                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Gefährdung anderer Personen:").Bold();
                    c.Item().PaddingTop(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(6).Text(_data.Fremdgefaehrdung);
                });

                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Diagnose (ICD-10):").Bold();
                    c.Item().PaddingTop(4).Text(_data.Diagnose).Bold();
                });

                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Empfehlung:").Bold();
                    c.Item().PaddingTop(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(6).Text(_data.Empfehlung);
                });

                col.Item().PaddingTop(8).Text($"Vorgeschlagene Dauer der Unterbringung: {_data.VorgeschlageneDauer}");

                col.Item().PaddingTop(30).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f);
                        c.Item().PaddingTop(4).Text("Datum, Ort").FontSize(9);
                    });
                    row.ConstantItem(20);
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f);
                        c.Item().PaddingTop(4).Text("Unterschrift, Stempel").FontSize(9);
                    });
                });
            });
        });
    }
}
