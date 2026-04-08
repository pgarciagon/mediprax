using MediPrax.Application.Catalogs;

namespace MediPrax.UnitTests;

public class AmdpCatalogTests
{
    [Fact]
    public void AllSymptoms_ReturnsAllSymptoms_AtLeast80()
    {
        var symptoms = AmdpCatalog.AllSymptoms;
        Assert.True(symptoms.Count >= 80, $"Expected at least 80 symptoms, got {symptoms.Count}");
    }

    [Fact]
    public void Categories_ContainsAllExpectedCategories()
    {
        var categories = AmdpCatalog.Categories;
        var expectedCodes = new[] { "BEW", "ORI", "AUF", "FDS", "INH", "SIN", "ICH", "AFF", "ANT", "CIR", "SUI", "SOM" };

        Assert.Equal(expectedCodes.Length, categories.Count);
        foreach (var code in expectedCodes)
        {
            Assert.Contains(categories, c => c.Code == code);
        }
    }

    [Fact]
    public void GetByCategory_ReturnsCorrectSymptoms_ForBEW()
    {
        var symptoms = AmdpCatalog.GetByCategory("BEW");
        Assert.Equal(4, symptoms.Count);
        Assert.All(symptoms, s => Assert.Equal("BEW", s.CategoryCode));
        Assert.Contains(symptoms, s => s.Name == "Bewusstseinsverminderung");
    }

    [Fact]
    public void GetByCategory_ReturnsCorrectSymptoms_ForAFF()
    {
        var symptoms = AmdpCatalog.GetByCategory("AFF");
        Assert.Equal(19, symptoms.Count);
        Assert.All(symptoms, s => Assert.Equal("AFF", s.CategoryCode));
    }

    [Fact]
    public void GetByCategory_ReturnsEmpty_ForUnknownCategory()
    {
        var symptoms = AmdpCatalog.GetByCategory("UNKNOWN");
        Assert.Empty(symptoms);
    }

    [Fact]
    public void Search_FindsSymptomsByName()
    {
        var results = AmdpCatalog.Search("Bewusstseins");
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Contains("Bewusstseins", r.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Search_FindsSymptomsByCode()
    {
        var results = AmdpCatalog.Search("BEW-1");
        Assert.Single(results);
        Assert.Equal("BEW-1", results[0].SymptomCode);
    }

    [Fact]
    public void Search_ReturnsEmpty_ForShortQuery()
    {
        var results = AmdpCatalog.Search("B");
        Assert.Empty(results);
    }

    [Fact]
    public void Search_IsCaseInsensitive()
    {
        var results = AmdpCatalog.Search("bewusstseins");
        Assert.NotEmpty(results);
    }

    [Fact]
    public void AllSymptoms_HaveNarrativeTemplate()
    {
        Assert.All(AmdpCatalog.AllSymptoms, s =>
            Assert.False(string.IsNullOrWhiteSpace(s.NarrativeTemplate),
                $"Symptom {s.SymptomCode} ({s.Name}) has empty NarrativeTemplate"));
    }

    [Fact]
    public void AllSymptoms_HaveUniqueSymptomCodes()
    {
        var codes = AmdpCatalog.AllSymptoms.Select(s => s.SymptomCode).ToList();
        Assert.Equal(codes.Count, codes.Distinct().Count());
    }
}
