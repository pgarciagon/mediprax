using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MediPrax.Reporting.Arztbrief;

public static class ArztbriefGenerator
{
    static ArztbriefGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Generate(ArztbriefModel model)
    {
        var document = new ArztbriefDocument(model);
        return document.GeneratePdf();
    }
}
