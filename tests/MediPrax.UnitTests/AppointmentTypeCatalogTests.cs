using MediPrax.Application.Catalogs;

namespace MediPrax.UnitTests;

public class AppointmentTypeCatalogTests
{
    [Fact]
    public void AllTemplates_IsNotEmpty()
    {
        Assert.NotEmpty(AppointmentTypeCatalog.AllTemplates);
    }

    [Fact]
    public void AllTemplates_HasExpectedCount()
    {
        Assert.Equal(8, AppointmentTypeCatalog.AllTemplates.Count);
    }

    [Theory]
    [InlineData("Erstgespräch", 50)]
    [InlineData("Folgetermin", 25)]
    [InlineData("Medikamentenkontrolle", 10)]
    [InlineData("Psychotherapie-Sitzung", 50)]
    [InlineData("EEG", 30)]
    [InlineData("EMG/NLG", 45)]
    [InlineData("Akuttermin", 25)]
    [InlineData("Notfall", 15)]
    public void GetByName_ReturnsCorrectDuration(string name, int expectedDuration)
    {
        var template = AppointmentTypeCatalog.GetByName(name);
        Assert.NotNull(template);
        Assert.Equal(expectedDuration, template.DefaultDurationMinutes);
    }

    [Fact]
    public void GetByName_ReturnsNull_ForUnknownName()
    {
        var template = AppointmentTypeCatalog.GetByName("Unbekannt");
        Assert.Null(template);
    }

    [Fact]
    public void GetByName_IsCaseInsensitive()
    {
        var template = AppointmentTypeCatalog.GetByName("erstgespräch");
        Assert.NotNull(template);
    }

    [Fact]
    public void AllTemplates_HaveValidColors()
    {
        Assert.All(AppointmentTypeCatalog.AllTemplates, t =>
        {
            Assert.False(string.IsNullOrWhiteSpace(t.Color));
            Assert.StartsWith("#", t.Color);
        });
    }

    [Fact]
    public void AllTemplates_HavePositiveDuration()
    {
        Assert.All(AppointmentTypeCatalog.AllTemplates, t =>
            Assert.True(t.DefaultDurationMinutes > 0));
    }
}
