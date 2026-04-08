using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.PtvForms;

public class PtvFormData
{
    public string PatientName { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;
    public string Kvnr { get; set; } = string.Empty;
    public string Krankenkasse { get; set; } = string.Empty;
    public string Versichertennummer { get; set; } = string.Empty;
    public string TherapeutName { get; set; } = string.Empty;
    public string Therapieverfahren { get; set; } = string.Empty;
    public string BeantragteSitzungen { get; set; } = string.Empty;
    public string Diagnosen { get; set; } = string.Empty;
    public string Datum { get; set; } = string.Empty;
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisAdresse { get; set; } = "Osterstrasse 1a, 28199 Bremen";
}

/// <summary>
/// PTV 1 — Antrag des Versicherten auf Psychotherapie
/// </summary>
public class Ptv1Document : IDocument
{
    private readonly PtvFormData _data;
    public Ptv1Document(PtvFormData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Text("Antrag auf Psychotherapie (PTV 1)").Bold().FontSize(14);
                col.Item().Text("Antrag des Versicherten an die Krankenkasse").FontSize(9).FontColor(Colors.Grey.Medium);
                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Black);
            });

            page.Content().PaddingTop(15).Column(col =>
            {
                // Patient section
                col.Item().Text("1. Angaben zum Versicherten").Bold().FontSize(11);
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        FieldPair(c, "Name", _data.PatientName);
                        FieldPair(c, "Geburtsdatum", _data.PatientGeburtsdatum);
                    });
                    row.RelativeItem().Column(c =>
                    {
                        FieldPair(c, "KVNR", _data.Kvnr);
                        FieldPair(c, "Versichertennr.", _data.Versichertennummer);
                    });
                });
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        FieldPair(c, "Krankenkasse", _data.Krankenkasse);
                    });
                });

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Therapy type
                col.Item().PaddingTop(10).Text("2. Beantragte Leistung").Bold().FontSize(11);
                col.Item().PaddingTop(6).Column(c =>
                {
                    FieldPair(c, "Therapieverfahren", _data.Therapieverfahren);
                    FieldPair(c, "Beantragte Sitzungen", _data.BeantragteSitzungen);
                });

                col.Item().PaddingTop(8).Text("Art des Antrags:").FontSize(9);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.ConstantItem(20).Text("[ ]").FontSize(9);
                    row.RelativeItem().Text("Erstantrag").FontSize(9);
                    row.ConstantItem(20).Text("[ ]").FontSize(9);
                    row.RelativeItem().Text("Umwandlungsantrag").FontSize(9);
                    row.ConstantItem(20).Text("[ ]").FontSize(9);
                    row.RelativeItem().Text("Verlaengerungsantrag").FontSize(9);
                });

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Therapist
                col.Item().PaddingTop(10).Text("3. Angaben zum Therapeuten").Bold().FontSize(11);
                col.Item().PaddingTop(6).Column(c =>
                {
                    FieldPair(c, "Therapeut", _data.TherapeutName);
                    FieldPair(c, "Praxis", $"{_data.PraxisName}, {_data.PraxisAdresse}");
                });

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Diagnoses
                col.Item().PaddingTop(10).Text("4. Diagnosen (ICD-10)").Bold().FontSize(11);
                col.Item().PaddingTop(6).Text(_data.Diagnosen).FontSize(9);

                // Signature section
                col.Item().PaddingTop(30).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text("Ort, Datum").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(40);
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text("Unterschrift des Versicherten").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            page.Footer().Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                col.Item().PaddingTop(4).Text($"Erstellt am {_data.Datum}").FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private static void FieldPair(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingTop(3).Row(row =>
        {
            row.ConstantItem(130).Text($"{label}:").FontSize(9).FontColor(Colors.Grey.Darken1);
            row.RelativeItem().Text(value).FontSize(9);
        });
    }
}

/// <summary>
/// PTV 2 — Angaben des Therapeuten an die Krankenkasse
/// </summary>
public class Ptv2Document : IDocument
{
    private readonly PtvFormData _data;
    private readonly int _sprechstundenCount;
    private readonly int _probatorikCount;

    public Ptv2Document(PtvFormData data, int sprechstundenCount = 0, int probatorikCount = 0)
    {
        _data = data;
        _sprechstundenCount = sprechstundenCount;
        _probatorikCount = probatorikCount;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                col.Item().Text("Angaben des Therapeuten an die Krankenkasse (PTV 2)").Bold().FontSize(14);
                col.Item().Text("Mitteilung an die Krankenkasse nach Abschluss der probatorischen Sitzungen").FontSize(9).FontColor(Colors.Grey.Medium);
                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Black);
            });

            page.Content().PaddingTop(15).Column(col =>
            {
                // Patient data
                col.Item().Text("1. Angaben zum Patienten").Bold().FontSize(11);
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        FieldPair(c, "Name", _data.PatientName);
                        FieldPair(c, "Geburtsdatum", _data.PatientGeburtsdatum);
                    });
                    row.RelativeItem().Column(c =>
                    {
                        FieldPair(c, "KVNR", _data.Kvnr);
                        FieldPair(c, "Krankenkasse", _data.Krankenkasse);
                    });
                });

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Therapist data
                col.Item().PaddingTop(10).Text("2. Angaben zum Therapeuten").Bold().FontSize(11);
                col.Item().PaddingTop(6).Column(c =>
                {
                    FieldPair(c, "Therapeut", _data.TherapeutName);
                    FieldPair(c, "Praxis", $"{_data.PraxisName}, {_data.PraxisAdresse}");
                });

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Therapy details
                col.Item().PaddingTop(10).Text("3. Angaben zur Therapie").Bold().FontSize(11);
                col.Item().PaddingTop(6).Column(c =>
                {
                    FieldPair(c, "Therapieverfahren", _data.Therapieverfahren);
                    FieldPair(c, "Sprechstunden durchgefuehrt", _sprechstundenCount.ToString());
                    FieldPair(c, "Probatorische Sitzungen", _probatorikCount.ToString());
                    FieldPair(c, "Beantragte Sitzungszahl", _data.BeantragteSitzungen);
                });

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Diagnoses
                col.Item().PaddingTop(10).Text("4. Diagnosen (ICD-10)").Bold().FontSize(11);
                col.Item().PaddingTop(6).Text(_data.Diagnosen).FontSize(9);

                col.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Clinical summary placeholder
                col.Item().PaddingTop(10).Text("5. Kurzfassung der klinischen Befunde").Bold().FontSize(11);
                col.Item().PaddingTop(6).MinHeight(80).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(6)
                    .Text("(Freitext)").FontSize(9).FontColor(Colors.Grey.Medium);

                // Signature
                col.Item().PaddingTop(30).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text("Ort, Datum").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(40);
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text("Unterschrift und Stempel des Therapeuten").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            page.Footer().Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                col.Item().PaddingTop(4).Text($"Erstellt am {_data.Datum}").FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private static void FieldPair(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingTop(3).Row(row =>
        {
            row.ConstantItem(180).Text($"{label}:").FontSize(9).FontColor(Colors.Grey.Darken1);
            row.RelativeItem().Text(value).FontSize(9);
        });
    }
}
