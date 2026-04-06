using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Arztbrief;

public class ArztbriefDocument : IDocument
{
    private readonly ArztbriefModel _model;

    public ArztbriefDocument(ArztbriefModel model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Arztbrief — {_model.PatientName}",
        Author = _model.DoctorName
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginTop(2, Unit.Centimetre);
            page.MarginBottom(2, Unit.Centimetre);
            page.MarginHorizontal(2.5f, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            // Briefkopf
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(_model.PraxisName).Bold().FontSize(14);
                    left.Item().Text(_model.PraxisSubtitle).FontSize(8).FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(180).AlignRight().Column(right =>
                {
                    right.Item().Text(_model.PraxisAddress).FontSize(8).FontColor(Colors.Grey.Medium);
                    right.Item().Text(_model.PraxisPhone).FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            col.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

            // Empfänger
            col.Item().PaddingTop(10).Column(addr =>
            {
                addr.Item().Text(_model.RecipientName).FontSize(10);
                if (!string.IsNullOrEmpty(_model.RecipientAddress))
                    addr.Item().Text(_model.RecipientAddress).FontSize(10).FontColor(Colors.Grey.Darken1);
            });

            // Datum
            col.Item().PaddingTop(15).AlignRight().Text($"Bremen, {_model.Date}").FontSize(9);

            col.Item().PaddingTop(5).PaddingBottom(5);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            // Betreff
            col.Item().Text(_model.Subject).Bold().FontSize(11);

            // Patientendaten
            col.Item().PaddingTop(8).Background(Colors.Grey.Lighten4).Padding(8).Column(patient =>
            {
                patient.Item().Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Patient: ").SemiBold();
                        t.Span(_model.PatientName);
                    });
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Geb.: ").SemiBold();
                        t.Span(_model.PatientDateOfBirth);
                    });
                });

                if (!string.IsNullOrEmpty(_model.PatientInsurance))
                {
                    patient.Item().Text(t =>
                    {
                        t.Span("Versicherung: ").SemiBold();
                        t.Span(_model.PatientInsurance);
                    });
                }
            });

            // Diagnosen
            if (_model.Diagnoses.Count > 0)
            {
                col.Item().PaddingTop(12).Text("Diagnosen:").SemiBold();
                col.Item().PaddingLeft(10).Column(diag =>
                {
                    foreach (var d in _model.Diagnoses)
                        diag.Item().Text($"• {d}").FontSize(9.5f);
                });
            }

            // Freitext
            col.Item().PaddingTop(12).Text(_model.Body).FontSize(10).LineHeight(1.5f);

            // Grußformel + Unterschrift
            col.Item().PaddingTop(25).Text("Mit freundlichen kollegialen Grüßen").FontSize(10);
            col.Item().PaddingTop(30).Column(sig =>
            {
                sig.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                sig.Item().PaddingTop(3).Text(_model.DoctorName).FontSize(10);
                sig.Item().Text(_model.PraxisName).FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(t =>
        {
            t.Span("Seite ").FontSize(8).FontColor(Colors.Grey.Medium);
            t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
            t.Span(" von ").FontSize(8).FontColor(Colors.Grey.Medium);
            t.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }
}
