using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Formulare;

public class GoaeRechnungPosition
{
    public string GopCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly Datum { get; set; }
    public decimal Steigerungsfaktor { get; set; } = 2.3m;
    public decimal Punktzahl { get; set; }
    public decimal Betrag { get; set; }
}

public class GoaeRechnungData
{
    public string PraxisName { get; set; } = "Neuropsychiatricum Bremen";
    public string PraxisUntertitel { get; set; } = "Gemeinschaftspraxis für Psychiatrie und Neurologie";
    public string PraxisAdresse { get; set; } = "Osterstraße 1a, 28199 Bremen";
    public string PraxisTelefon { get; set; } = string.Empty;
    public string ArztName { get; set; } = string.Empty;
    public string? SteuernummerOderUstId { get; set; }

    public string PatientName { get; set; } = string.Empty;
    public string PatientAdresse { get; set; } = string.Empty;
    public string PatientGeburtsdatum { get; set; } = string.Empty;

    public string RechnungsNummer { get; set; } = string.Empty;
    public string RechnungsDatum { get; set; } = string.Empty;
    public string? Diagnose { get; set; }

    public List<GoaeRechnungPosition> Positionen { get; set; } = [];

    public decimal Gesamtbetrag { get; set; }
    public string Zahlungsziel { get; set; } = string.Empty;

    public string BankInhaber { get; set; } = string.Empty;
    public string BankIban { get; set; } = string.Empty;
    public string BankBic { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}

public class GoaeRechnungDocument : IDocument
{
    private readonly GoaeRechnungData _data;
    public GoaeRechnungDocument(GoaeRechnungData data) => _data = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginHorizontal(50);
            page.MarginVertical(40);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

            page.Header().Column(col =>
            {
                // Practice header
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_data.PraxisName).Bold().FontSize(14);
                        c.Item().Text(_data.PraxisUntertitel).FontSize(8).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_data.PraxisAdresse).FontSize(8);
                        if (!string.IsNullOrEmpty(_data.PraxisTelefon))
                            c.Item().Text($"Tel.: {_data.PraxisTelefon}").FontSize(8);
                    });
                    row.ConstantItem(140).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Rechnungsnr.: {_data.RechnungsNummer}").Bold().FontSize(9);
                        c.Item().Text($"Datum: {_data.RechnungsDatum}").FontSize(9);
                        if (!string.IsNullOrEmpty(_data.SteuernummerOderUstId))
                            c.Item().Text($"Steuer-Nr.: {_data.SteuernummerOderUstId}").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor(Colors.Blue.Darken3);
            });

            page.Content().PaddingTop(16).Column(col =>
            {
                ComposePatientBlock(col);
                ComposeInvoiceTitle(col);
                ComposeDiagnosis(col);
                ComposePositionsTable(col);
                ComposeTotalBlock(col);
                ComposePaymentBlock(col);
                ComposeNotes(col);
            });

            page.Footer().Column(c =>
            {
                c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                c.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text($"{_data.ArztName} · {_data.PraxisName}").FontSize(7).FontColor(Colors.Grey.Medium);
                    row.RelativeItem().AlignCenter().Text(_data.PraxisAdresse).FontSize(7).FontColor(Colors.Grey.Medium);
                    row.RelativeItem().AlignRight().Text($"Seite 1").FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }

    private void ComposePatientBlock(ColumnDescriptor col)
    {
        col.Item().PaddingBottom(4).Text("An:").FontSize(7).FontColor(Colors.Grey.Medium);
        col.Item().Column(c =>
        {
            c.Item().Text(_data.PatientName).Bold().FontSize(10);
            c.Item().Text(_data.PatientAdresse);
            if (!string.IsNullOrEmpty(_data.PatientGeburtsdatum))
                c.Item().Text($"geb. {_data.PatientGeburtsdatum}").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }

    private void ComposeInvoiceTitle(ColumnDescriptor col)
    {
        col.Item().PaddingTop(20).Text($"Rechnung Nr. {_data.RechnungsNummer}").Bold().FontSize(13);
        col.Item().PaddingTop(2).Text("Liquidation nach der Gebührenordnung für Ärzte (GOÄ)")
            .FontSize(9).FontColor(Colors.Grey.Darken1);
    }

    private void ComposeDiagnosis(ColumnDescriptor col)
    {
        if (string.IsNullOrEmpty(_data.Diagnose)) return;

        col.Item().PaddingTop(12).Column(c =>
        {
            c.Item().Text("Diagnose:").Bold().FontSize(9);
            c.Item().PaddingTop(2).Text(_data.Diagnose).FontSize(9);
        });
    }

    private void ComposePositionsTable(ColumnDescriptor col)
    {
        col.Item().PaddingTop(16).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(65);  // Date
                columns.ConstantColumn(50);  // GOÄ-Ziffer
                columns.RelativeColumn(3);   // Description
                columns.ConstantColumn(45);  // Factor
                columns.ConstantColumn(55);  // Punktzahl
                columns.ConstantColumn(65);  // Betrag
            });

            // Header row
            table.Header(header =>
            {
                header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4)
                    .Text("Datum").Bold().FontSize(8);
                header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4)
                    .Text("Ziffer").Bold().FontSize(8);
                header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4)
                    .Text("Leistungsbezeichnung").Bold().FontSize(8);
                header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4).AlignRight()
                    .Text("Faktor").Bold().FontSize(8);
                header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4).AlignRight()
                    .Text("Punktzahl").Bold().FontSize(8);
                header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4).AlignRight()
                    .Text("Betrag €").Bold().FontSize(8);
            });

            // Position rows
            foreach (var pos in _data.Positionen)
            {
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4)
                    .Text(pos.Datum.ToString("dd.MM.yyyy")).FontSize(8);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4)
                    .Text(pos.GopCode).FontSize(8);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4)
                    .Text(pos.Description).FontSize(8);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight()
                    .Text(pos.Steigerungsfaktor.ToString("F2")).FontSize(8);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight()
                    .Text(pos.Punktzahl.ToString("F0")).FontSize(8);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight()
                    .Text(pos.Betrag.ToString("N2")).FontSize(8);
            }
        });
    }

    private void ComposeTotalBlock(ColumnDescriptor col)
    {
        col.Item().PaddingTop(8).AlignRight().Row(row =>
        {
            row.ConstantItem(200).Column(c =>
            {
                c.Item().BorderTop(2).BorderColor(Colors.Black).PaddingTop(6).Row(r =>
                {
                    r.RelativeItem().Text("Gesamtbetrag:").Bold().FontSize(11);
                    r.ConstantItem(80).AlignRight().Text($"{_data.Gesamtbetrag:N2} €").Bold().FontSize(11);
                });
                c.Item().PaddingTop(2).Text("(gem. § 4 Nr. 14a UStG umsatzsteuerbefreit)")
                    .FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private void ComposePaymentBlock(ColumnDescriptor col)
    {
        col.Item().PaddingTop(20).Column(c =>
        {
            c.Item().Text($"Bitte überweisen Sie den Betrag von {_data.Gesamtbetrag:N2} € bis zum {_data.Zahlungsziel} auf folgendes Konto:")
                .FontSize(9);

            c.Item().PaddingTop(8).Background(Colors.Grey.Lighten4).Padding(10).Column(bank =>
            {
                bank.Item().Text("Bankverbindung").Bold().FontSize(9);
                bank.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text($"Kontoinhaber: {_data.BankInhaber}").FontSize(9);
                        left.Item().Text($"IBAN: {_data.BankIban}").FontSize(9).Bold();
                    });
                    row.RelativeItem().Column(right =>
                    {
                        right.Item().Text($"BIC: {_data.BankBic}").FontSize(9);
                        if (!string.IsNullOrEmpty(_data.BankName))
                            right.Item().Text($"Bank: {_data.BankName}").FontSize(9);
                    });
                });
                bank.Item().PaddingTop(4).Text($"Verwendungszweck: {_data.RechnungsNummer}")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private void ComposeNotes(ColumnDescriptor col)
    {
        col.Item().PaddingTop(20).Column(c =>
        {
            c.Item().Text("Bitte geben Sie bei der Überweisung die Rechnungsnummer als Verwendungszweck an.")
                .FontSize(8).FontColor(Colors.Grey.Darken1);
            c.Item().PaddingTop(4).Text("Bei Rückfragen wenden Sie sich bitte an unsere Praxis.")
                .FontSize(8).FontColor(Colors.Grey.Darken1);
            c.Item().PaddingTop(4).Text("Diese Rechnung wurde maschinell erstellt und ist ohne Unterschrift gültig.")
                .FontSize(7).FontColor(Colors.Grey.Medium);
        });
    }
}
