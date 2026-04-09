using MediPrax.Application.Catalogs;

namespace MediPrax.UnitTests;

public class GopExclusionCatalogTests
{
    [Fact]
    public void AllRules_IsNotEmpty()
    {
        Assert.NotEmpty(GopExclusionCatalog.AllRules);
    }

    [Fact]
    public void AreExclusive_ReturnsTrue_ForKnownPair()
    {
        Assert.True(GopExclusionCatalog.AreExclusive("16210", "16211"));
    }

    [Fact]
    public void AreExclusive_IsSymmetric()
    {
        Assert.True(GopExclusionCatalog.AreExclusive("16220", "21220"));
        Assert.True(GopExclusionCatalog.AreExclusive("21220", "16220"));
    }

    [Fact]
    public void AreExclusive_ReturnsFalse_ForNonExclusivePair()
    {
        Assert.False(GopExclusionCatalog.AreExclusive("16210", "16220"));
    }

    [Fact]
    public void AreExclusive_ReturnsFalse_ForUnknownCodes()
    {
        Assert.False(GopExclusionCatalog.AreExclusive("99999", "88888"));
    }

    [Fact]
    public void AreExclusive_IsCaseInsensitive()
    {
        Assert.True(GopExclusionCatalog.AreExclusive("16210", "16211"));
    }

    [Fact]
    public void GetRulesFor_ReturnsRules_ForCodeInvolvedInExclusions()
    {
        var rules = GopExclusionCatalog.GetRulesFor("16210");
        Assert.NotEmpty(rules);
        Assert.All(rules, r =>
            Assert.True(r.GopCodeA == "16210" || r.GopCodeB == "16210"));
    }

    [Fact]
    public void GetRulesFor_ReturnsEmpty_ForCodeWithoutExclusions()
    {
        var rules = GopExclusionCatalog.GetRulesFor("99999");
        Assert.Empty(rules);
    }

    [Fact]
    public void FindRule_ReturnsRule_ForKnownPair()
    {
        var rule = GopExclusionCatalog.FindRule("16220", "21220");
        Assert.NotNull(rule);
        Assert.Contains("Gespräch", rule.Reason);
    }

    [Fact]
    public void FindRule_ReturnsNull_ForNonExclusivePair()
    {
        var rule = GopExclusionCatalog.FindRule("16210", "16220");
        Assert.Null(rule);
    }

    [Theory]
    [InlineData("16210", "21210")]
    [InlineData("16211", "21211")]
    [InlineData("21210", "21211")]
    [InlineData("16230", "16231")]
    [InlineData("21230", "21235")]
    [InlineData("35100", "21213")]
    public void AreExclusive_ReturnsTrue_ForAllCrossSpecialtyRules(string a, string b)
    {
        Assert.True(GopExclusionCatalog.AreExclusive(a, b));
    }

    [Fact]
    public void AllRules_HaveNonEmptyReasons()
    {
        Assert.All(GopExclusionCatalog.AllRules, rule =>
        {
            Assert.False(string.IsNullOrWhiteSpace(rule.GopCodeA));
            Assert.False(string.IsNullOrWhiteSpace(rule.GopCodeB));
            Assert.False(string.IsNullOrWhiteSpace(rule.Reason));
        });
    }
}
