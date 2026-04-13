using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// E2E tests for Arzneimittelkatalog (M48) — medication catalog search and browse.
/// Requires: PostgreSQL running with medication_catalog seeded, Chromium installed.
/// </summary>
[Collection("Playwright")]
public class ArzneimittelkatalogE2ETests : E2ETestBase
{
    public ArzneimittelkatalogE2ETests(PlaywrightFixture playwright, MediPraxServerFixture server)
        : base(playwright, server) { }

    [Fact]
    public async Task CatalogPage_LoadsWithStatistics()
    {
        await NavigateToAsync("/verwaltung/arzneimittelkatalog");

        // Statistics panel should show total entries
        var statsPanel = Page.Locator(".catalog-statistics, .statistics-panel, .stat-card").First;
        await Expect(statsPanel).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task CatalogPage_SearchByTradeName()
    {
        await NavigateToAsync("/verwaltung/arzneimittelkatalog");

        // Search for a known medication
        var searchInput = Page.Locator("input[placeholder*='Suche'], input[type='search'], input[placeholder*='suchen']").First;
        await searchInput.WaitForAsync(new() { Timeout = 5_000 });
        await searchInput.FillAsync("Sertralin");

        // Click search button or wait for auto-search
        var searchButton = Page.Locator("button:has-text('Suchen')");
        if (await searchButton.CountAsync() > 0)
            await searchButton.ClickAsync();
        else
            await Page.WaitForTimeoutAsync(500); // debounce

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Results should contain Sertralin
        var results = Page.Locator("table tbody tr, .result-item");
        var count = await results.CountAsync();
        Assert.True(count > 0, "Search for 'Sertralin' should return results");

        var sertralinText = Page.Locator("text=Sertralin");
        await Expect(sertralinText.First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task CatalogPage_SearchByWirkstoff()
    {
        await NavigateToAsync("/verwaltung/arzneimittelkatalog");

        var searchInput = Page.Locator("input[placeholder*='Suche'], input[type='search'], input[placeholder*='suchen']").First;
        await searchInput.WaitForAsync(new() { Timeout = 5_000 });
        await searchInput.FillAsync("Levodopa");

        var searchButton = Page.Locator("button:has-text('Suchen')");
        if (await searchButton.CountAsync() > 0)
            await searchButton.ClickAsync();
        else
            await Page.WaitForTimeoutAsync(500);

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var results = Page.Locator("table tbody tr, .result-item");
        var count = await results.CountAsync();
        Assert.True(count > 0, "Search for 'Levodopa' should return results");
    }

    [Fact]
    public async Task CatalogPage_CategoryFilterWorks()
    {
        await NavigateToAsync("/verwaltung/arzneimittelkatalog");

        // Find category filter dropdown
        var categorySelect = Page.Locator("select").First;
        await categorySelect.WaitForAsync(new() { Timeout = 5_000 });

        var options = await categorySelect.Locator("option").AllTextContentsAsync();
        Assert.True(options.Count > 1, "Category dropdown should have multiple options");

        // Select a specific category (e.g., Antidepressivum)
        var antidepOption = options.FirstOrDefault(o => o.Contains("Antidepressiv"));
        if (antidepOption != null)
        {
            await categorySelect.SelectOptionAsync(new SelectOptionValue { Label = antidepOption });
            await Page.WaitForTimeoutAsync(500);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Results should be filtered
            var results = Page.Locator("table tbody tr, .result-item");
            var count = await results.CountAsync();
            Assert.True(count > 0, "Filtered results should show antidepressants");
        }
    }

    [Fact]
    public async Task CatalogPage_BtmBadgeVisible()
    {
        await NavigateToAsync("/verwaltung/arzneimittelkatalog");

        // Search for a known BtM substance
        var searchInput = Page.Locator("input[placeholder*='Suche'], input[type='search'], input[placeholder*='suchen']").First;
        await searchInput.WaitForAsync(new() { Timeout = 5_000 });
        await searchInput.FillAsync("Methylphenidat");

        var searchButton = Page.Locator("button:has-text('Suchen')");
        if (await searchButton.CountAsync() > 0)
            await searchButton.ClickAsync();
        else
            await Page.WaitForTimeoutAsync(500);

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should show BtM badge
        var btmBadge = Page.Locator("text=BtM, .btm-badge, .badge:has-text('BtM')").First;
        await Expect(btmBadge).ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    [Fact]
    public async Task SidebarLink_NavigatesToCatalog()
    {
        await NavigateToAsync("/");

        // Click sidebar link
        var sidebarLink = Page.Locator("a:has-text('Arzneimittelkatalog')");
        await Expect(sidebarLink).ToBeVisibleAsync(new() { Timeout = 5_000 });
        await sidebarLink.ClickAsync();

        await Page.WaitForURLAsync(url => url.Contains("/verwaltung/arzneimittelkatalog"), new() { Timeout = 5_000 });
    }

    /// <summary>Helper to access Playwright Expect assertions.</summary>
    private static ILocatorAssertions Expect(ILocator locator)
        => Assertions.Expect(locator);
}
