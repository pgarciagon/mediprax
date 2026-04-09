using MediPrax.Application.Data;

namespace MediPrax.UnitTests;

public class GopSuggestionServiceTests
{
    [Theory]
    [InlineData("16210")]
    [InlineData("16211")]
    [InlineData("16220")]
    [InlineData("21210")]
    [InlineData("21211")]
    [InlineData("21220")]
    [InlineData("21213")]
    [InlineData("16230")]
    public void GopCatalog_ContainsAllSuggestedCodes(string code)
    {
        var entry = GopCatalog.GetByCode(code);
        Assert.NotNull(entry);
        Assert.Equal(code, entry.Code);
    }

    [Fact]
    public void GetCurrentQuarter_FormatIsCorrect()
    {
        var now = DateTime.UtcNow;
        var q = (now.Month - 1) / 3 + 1;
        var expected = $"{now.Year}-Q{q}";
        Assert.Matches(@"^\d{4}-Q[1-4]$", expected);
    }
}
