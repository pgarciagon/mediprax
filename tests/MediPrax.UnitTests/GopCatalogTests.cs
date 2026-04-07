using MediPrax.Application.Data;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class GopCatalogTests
{
    [Fact]
    public void Search_ReturnsResults_ForValidCode()
    {
        var results = GopCatalog.Search("16220");
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.Code == "16220");
    }

    [Fact]
    public void Search_ReturnsResults_ForDescription()
    {
        var results = GopCatalog.Search("Psychiatrisch");
        Assert.NotEmpty(results);
    }

    [Fact]
    public void Search_ReturnsEmpty_ForShortQuery()
    {
        var results = GopCatalog.Search("1");
        Assert.Empty(results);
    }

    [Fact]
    public void Search_FiltersBy_BillingType()
    {
        var ebm = GopCatalog.Search("16220", BillingType.EBM);
        Assert.NotEmpty(ebm);
        Assert.All(ebm, r => Assert.Equal(BillingType.EBM, r.Type));

        var goa = GopCatalog.Search("16220", BillingType.GOA);
        Assert.Empty(goa);
    }

    [Fact]
    public void GetByCode_ReturnsEntry_ForValidCode()
    {
        var entry = GopCatalog.GetByCode("16220");
        Assert.NotNull(entry);
        Assert.Equal("16220", entry.Code);
        Assert.Equal(BillingType.EBM, entry.Type);
    }

    [Fact]
    public void GetByCode_ReturnsNull_ForInvalidCode()
    {
        var entry = GopCatalog.GetByCode("99999");
        Assert.Null(entry);
    }

    [Fact]
    public void CalculateEbmAmount_ReturnsCorrectAmount()
    {
        // 16220 has 248 Punktzahl, Punktwert ~4.03ct
        var amount = GopCatalog.CalculateEbmAmount("16220");
        Assert.True(amount > 0);
        Assert.Equal(Math.Round(248 * 0.0403m, 2), amount);
    }

    [Fact]
    public void CalculateGoaeAmount_ReturnsCorrectAmount()
    {
        // GOÄ 857 has 400 Punktzahl, Steigerungsfaktor 2.3
        var amount = GopCatalog.CalculateGoaeAmount("857", 2.3m);
        Assert.True(amount > 0);
        Assert.Equal(Math.Round(400 * 0.0582873m * 2.3m, 2), amount);
    }

    [Fact]
    public void CalculateGoaeAmount_ReturnsZero_ForEbmCode()
    {
        var amount = GopCatalog.CalculateGoaeAmount("16220", 2.3m);
        Assert.Equal(0, amount);
    }

    [Fact]
    public void CalculateEbmAmount_ReturnsZero_ForGoaCode()
    {
        var amount = GopCatalog.CalculateEbmAmount("857");
        Assert.Equal(0, amount);
    }
}
