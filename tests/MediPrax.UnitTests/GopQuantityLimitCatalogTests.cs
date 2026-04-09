using MediPrax.Application.Catalogs;

namespace MediPrax.UnitTests;

public class GopQuantityLimitCatalogTests
{
    [Fact]
    public void AllLimits_IsNotEmpty()
    {
        Assert.NotEmpty(GopQuantityLimitCatalog.AllLimits);
    }

    [Theory]
    [InlineData("16210", 1)]
    [InlineData("16211", 1)]
    [InlineData("21210", 1)]
    [InlineData("21211", 1)]
    public void GetLimit_ReturnsCorrectQuarterLimit_ForGrundpauschalen(string code, int expectedMax)
    {
        var limit = GopQuantityLimitCatalog.GetLimit(code);
        Assert.NotNull(limit);
        Assert.Equal(expectedMax, limit.MaxPerQuarter);
    }

    [Fact]
    public void GetLimit_IncludesDayLimit_ForGespraeche()
    {
        var limit = GopQuantityLimitCatalog.GetLimit("16220");
        Assert.NotNull(limit);
        Assert.Equal(3, limit.MaxPerQuarter);
        Assert.Equal(1, limit.MaxPerDay);
    }

    [Fact]
    public void GetLimit_ReturnsNull_ForUnknownCode()
    {
        var limit = GopQuantityLimitCatalog.GetLimit("99999");
        Assert.Null(limit);
    }

    [Fact]
    public void GetLimit_ReturnsCorrectLimit_ForPsychotherapy()
    {
        var limit = GopQuantityLimitCatalog.GetLimit("35100");
        Assert.NotNull(limit);
        Assert.Equal(6, limit.MaxPerQuarter);
        Assert.Null(limit.MaxPerDay);
    }

    [Fact]
    public void GetLimit_ReturnsCorrectLimit_ForKrisenintervention()
    {
        var limit = GopQuantityLimitCatalog.GetLimit("21215");
        Assert.NotNull(limit);
        Assert.Equal(2, limit.MaxPerQuarter);
        Assert.Equal(1, limit.MaxPerDay);
    }

    [Fact]
    public void AllLimits_HaveValidData()
    {
        Assert.All(GopQuantityLimitCatalog.AllLimits, limit =>
        {
            Assert.False(string.IsNullOrWhiteSpace(limit.GopCode));
            Assert.True(limit.MaxPerQuarter > 0);
            Assert.False(string.IsNullOrWhiteSpace(limit.Description));
        });
    }
}
