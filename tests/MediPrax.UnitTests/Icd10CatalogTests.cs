using MediPrax.Application.Data;

namespace MediPrax.UnitTests;

public class Icd10CatalogTests
{
    [Fact]
    public void Search_ReturnsResults_ForCode()
    {
        var results = Icd10Catalog.Search("F32");
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Contains("F32", r.Code));
    }

    [Fact]
    public void Search_ReturnsResults_ForDescription()
    {
        var results = Icd10Catalog.Search("depressive");
        Assert.NotEmpty(results);
    }

    [Fact]
    public void Search_ReturnsEmpty_ForShortQuery()
    {
        var results = Icd10Catalog.Search("F");
        Assert.Empty(results);
    }

    [Fact]
    public void Search_RespectsMaxResults()
    {
        var results = Icd10Catalog.Search("F3", maxResults: 3);
        Assert.True(results.Count <= 3);
    }

    [Fact]
    public void GetDescription_ReturnsDescription_ForValidCode()
    {
        var desc = Icd10Catalog.GetDescription("F32.1");
        Assert.NotNull(desc);
        Assert.Contains("depressive", desc, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetDescription_ReturnsNull_ForInvalidCode()
    {
        var desc = Icd10Catalog.GetDescription("Z99.99");
        Assert.Null(desc);
    }
}
