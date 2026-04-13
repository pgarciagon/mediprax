using MediPrax.Reporting.Formulare;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MediPrax.UnitTests;

public class GoaeRechnungDocumentTests
{
    static GoaeRechnungDocumentTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
    private static GoaeRechnungData CreateSampleData(int positionCount = 3)
    {
        var positionen = new List<GoaeRechnungPosition>();
        decimal total = 0m;

        for (int i = 1; i <= positionCount; i++)
        {
            var betrag = 25.00m * i;
            total += betrag;
            positionen.Add(new GoaeRechnungPosition
            {
                GopCode = $"{i}",
                Description = $"Beratung Position {i}",
                Datum = new DateOnly(2026, 4, 10),
                Steigerungsfaktor = 2.3m,
                Punktzahl = 80 + i * 10,
                Betrag = betrag
            });
        }

        return new GoaeRechnungData
        {
            PraxisName = "Neuropsychiatricum Bremen",
            PraxisUntertitel = "Gemeinschaftspraxis für Psychiatrie und Neurologie",
            PraxisAdresse = "Osterstraße 1a, 28199 Bremen",
            PraxisTelefon = "0421 12345",
            ArztName = "Dr. med. Max Mustermann",
            SteuernummerOderUstId = "60/123/45678",
            PatientName = "Erika Musterfrau",
            PatientAdresse = "Musterweg 5, 28195 Bremen",
            PatientGeburtsdatum = "15.03.1975",
            RechnungsNummer = "RE-2026-0001",
            RechnungsDatum = "10.04.2026",
            Diagnose = "F32.1 Mittelgradige depressive Episode",
            Positionen = positionen,
            Gesamtbetrag = total,
            Zahlungsziel = "10.05.2026",
            BankInhaber = "Dr. med. Max Mustermann",
            BankIban = "DE89 3704 0044 0532 0130 00",
            BankBic = "COBADEFFXXX",
            BankName = "Commerzbank Bremen"
        };
    }

    [Fact]
    public void GeneratePdf_ProducesValidPdfBytes()
    {
        var data = CreateSampleData();
        var document = new GoaeRechnungDocument(data);

        var pdf = document.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100, "PDF should have substantial content");
        // PDF files start with %PDF
        Assert.Equal((byte)'%', pdf[0]);
        Assert.Equal((byte)'P', pdf[1]);
        Assert.Equal((byte)'D', pdf[2]);
        Assert.Equal((byte)'F', pdf[3]);
    }

    [Fact]
    public void GeneratePdf_WithMultiplePositions_ProducesValidPdf()
    {
        var data = CreateSampleData(positionCount: 10);
        var document = new GoaeRechnungDocument(data);

        var pdf = document.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);
        Assert.Equal((byte)'%', pdf[0]);
    }

    [Fact]
    public void GeneratePdf_WithSinglePosition_ProducesValidPdf()
    {
        var data = CreateSampleData(positionCount: 1);
        var document = new GoaeRechnungDocument(data);

        var pdf = document.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);
        Assert.Equal((byte)'%', pdf[0]);
    }

    [Fact]
    public void GeneratePdf_WithoutOptionalFields_ProducesValidPdf()
    {
        var data = CreateSampleData();
        data.Diagnose = null;
        data.SteuernummerOderUstId = null;
        data.PraxisTelefon = string.Empty;

        var document = new GoaeRechnungDocument(data);

        var pdf = document.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);
        Assert.Equal((byte)'%', pdf[0]);
    }

    [Fact]
    public void GoaeRechnungData_DefaultValues_AreCorrect()
    {
        var data = new GoaeRechnungData();

        Assert.Equal("Neuropsychiatricum Bremen", data.PraxisName);
        Assert.Equal("Gemeinschaftspraxis für Psychiatrie und Neurologie", data.PraxisUntertitel);
        Assert.Equal("Osterstraße 1a, 28199 Bremen", data.PraxisAdresse);
        Assert.Empty(data.Positionen);
    }

    [Fact]
    public void GoaeRechnungPosition_StoresAllFields()
    {
        var pos = new GoaeRechnungPosition
        {
            GopCode = "1",
            Description = "Beratung",
            Datum = new DateOnly(2026, 4, 10),
            Steigerungsfaktor = 2.3m,
            Punktzahl = 80,
            Betrag = 18.40m
        };

        Assert.Equal("1", pos.GopCode);
        Assert.Equal("Beratung", pos.Description);
        Assert.Equal(new DateOnly(2026, 4, 10), pos.Datum);
        Assert.Equal(2.3m, pos.Steigerungsfaktor);
        Assert.Equal(80m, pos.Punktzahl);
        Assert.Equal(18.40m, pos.Betrag);
    }

    [Fact]
    public void Gesamtbetrag_MatchesSumOfPositions()
    {
        var data = CreateSampleData(positionCount: 5);
        var expectedTotal = data.Positionen.Sum(p => p.Betrag);

        Assert.Equal(expectedTotal, data.Gesamtbetrag);
    }
}
