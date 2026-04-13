using MediPrax.Reporting.Formulare;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MediPrax.UnitTests;

public class UeberweisungPsychotherapieDocumentTests
{
    static UeberweisungPsychotherapieDocumentTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public void GeneratePdf_WithAllFields_ProducesValidPdf()
    {
        var data = new UeberweisungPsychotherapieData
        {
            PatientName = "Max Mustermann",
            PatientGeburtsdatum = "01.01.1980",
            Kvnr = "A123456789",
            Krankenkasse = "AOK Bremen",
            Versichertennummer = "1234567890",
            Diagnose = "Mittelgradige depressive Episode",
            DiagnoseIcd = "F32.1",
            Befunde = "Seit 3 Monaten anhaltende depressive Symptomatik",
            SomatischeAbklaerung = true,
            SomatischeAbklaerungDetails = "Labor unauffällig, Schilddrüse abgeklärt",
            TherapieForm = "Verhaltenstherapie",
            Dringlichkeit = true,
            ArztName = "Dr. med. Test",
            Datum = "13.04.2026"
        };

        var doc = new UeberweisungPsychotherapieDocument(data);
        var pdf = doc.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100, "PDF should have meaningful content");
        // PDF magic bytes: %PDF
        Assert.Equal(0x25, pdf[0]); // %
        Assert.Equal(0x50, pdf[1]); // P
        Assert.Equal(0x44, pdf[2]); // D
        Assert.Equal(0x46, pdf[3]); // F
    }

    [Fact]
    public void GeneratePdf_WithMinimalFields_ProducesValidPdf()
    {
        var data = new UeberweisungPsychotherapieData
        {
            PatientName = "Erika Musterfrau",
            PatientGeburtsdatum = "15.06.1990",
            Diagnose = "Generalisierte Angststörung",
            TherapieForm = "Tiefenpsychologisch fundierte Psychotherapie",
            ArztName = "Dr. med. Test",
            Datum = "13.04.2026"
        };

        var doc = new UeberweisungPsychotherapieDocument(data);
        var pdf = doc.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);
    }

    [Fact]
    public void GeneratePdf_WithoutSomaticWorkup_ProducesValidPdf()
    {
        var data = new UeberweisungPsychotherapieData
        {
            PatientName = "Test Patient",
            PatientGeburtsdatum = "01.01.1985",
            Diagnose = "Rezidivierende depressive Störung",
            DiagnoseIcd = "F33.1",
            TherapieForm = "Analytische Psychotherapie",
            SomatischeAbklaerung = false,
            Dringlichkeit = false,
            ArztName = "Dr. med. Test",
            Datum = "13.04.2026"
        };

        var doc = new UeberweisungPsychotherapieDocument(data);
        var pdf = doc.GeneratePdf();

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);
    }

    [Fact]
    public void UeberweisungPsychotherapieData_DefaultValues_AreCorrect()
    {
        var data = new UeberweisungPsychotherapieData();

        Assert.Equal("Neuropsychiatricum Bremen", data.PraxisName);
        Assert.Equal("Osterstraße 1a, 28199 Bremen", data.PraxisAdresse);
        Assert.False(data.SomatischeAbklaerung);
        Assert.False(data.Dringlichkeit);
        Assert.Equal(string.Empty, data.PatientName);
        Assert.Equal(string.Empty, data.TherapieForm);
    }
}
