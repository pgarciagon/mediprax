using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// E2E tests for Abrechnung (M23) — billing overview, plausibility validation, and GOÄ invoices.
/// Requires: PostgreSQL running with seed data, Chromium installed.
/// </summary>
[Collection("Playwright")]
public class AbrechnungE2ETests : E2ETestBase
{
    public AbrechnungE2ETests(PlaywrightFixture playwright, MediPraxServerFixture server)
        : base(playwright, server) { }

    [Fact]
    public async Task AbrechnungsUebersicht_LoadsWithData()
    {
        await NavigateToAsync("/abrechnung");

        // Should see quarter navigation and billing overview
        var header = Page.Locator("h1, h2, .page-header").First;
        await header.WaitForAsync(new() { Timeout = 5_000 });

        // Should show patient billing entries from seed data
        var table = Page.Locator("table");
        await Expect(table.First).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task QuartalsValidierung_RunsChecks()
    {
        await NavigateToAsync("/abrechnung/validierung");

        // Click validation button
        var validateBtn = Page.Locator("button:has-text('Validierung'), button:has-text('starten'), button:has-text('Prüfen')").First;
        await validateBtn.WaitForAsync(new() { Timeout = 5_000 });
        await validateBtn.ClickAsync();

        // Wait for results to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000); // Allow time for async validation

        // Should show validation results (summary cards or table)
        var results = Page.Locator(".validation-result, .summary-card, .stat-card, table").First;
        await Expect(results).ToBeVisibleAsync(new() { Timeout = 10_000 });
    }

    [Fact]
    public async Task Privatrechnungen_PageLoads()
    {
        await NavigateToAsync("/abrechnung/privatrechnungen");

        // Page should load with invoice list or empty state
        var content = Page.Locator(".page-content, .main-content, h1, h2").First;
        await content.WaitForAsync(new() { Timeout = 5_000 });

        // Should not show error
        var error = Page.Locator(".error-message, .alert-danger");
        var errorCount = await error.CountAsync();
        Assert.True(errorCount == 0, "Page should not show errors");
    }

    [Fact]
    public async Task SidebarNavigation_AbrechnungLinksWork()
    {
        await NavigateToAsync("/");

        // Click Abrechnung in sidebar
        var abrechnungLink = Page.Locator("a:has-text('Abrechnung')").First;
        await Expect(abrechnungLink).ToBeVisibleAsync(new() { Timeout = 5_000 });
        await abrechnungLink.ClickAsync();

        await Page.WaitForURLAsync(url => url.Contains("/abrechnung"), new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task BillingTabs_NavigateBetweenPages()
    {
        // Start at Abrechnung overview
        await NavigateToAsync("/abrechnung");

        // Should see billing tabs
        var tabs = Page.Locator(".billing-tabs");
        await Expect(tabs).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Should see 3 tab links
        var tabLinks = Page.Locator(".billing-tab");
        await Expect(tabLinks).ToHaveCountAsync(3);

        // "Übersicht" should be active
        var activeTab = Page.Locator(".billing-tab.active");
        await Expect(activeTab).ToHaveTextAsync("Übersicht");

        // Click "Quartalsvalidierung" tab
        var validierungTab = Page.Locator(".billing-tab:has-text('Quartalsvalidierung')");
        await validierungTab.ClickAsync();
        await Page.WaitForURLAsync(url => url.Contains("/abrechnung/validierung"), new() { Timeout = 5_000 });

        // "Quartalsvalidierung" should now be active
        activeTab = Page.Locator(".billing-tab.active");
        await Expect(activeTab).ToHaveTextAsync("Quartalsvalidierung");

        // Click "Privatrechnungen" tab
        var privatTab = Page.Locator(".billing-tab:has-text('Privatrechnungen')");
        await privatTab.ClickAsync();
        await Page.WaitForURLAsync(url => url.Contains("/abrechnung/privatrechnungen"), new() { Timeout = 5_000 });

        // "Privatrechnungen (GOÄ)" should now be active
        activeTab = Page.Locator(".billing-tab.active");
        await Expect(activeTab).ToContainTextAsync("Privatrechnungen");

        // Navigate back to overview via tab
        var uebersichtTab = Page.Locator(".billing-tab:has-text('Übersicht')");
        await uebersichtTab.ClickAsync();
        await Page.WaitForURLAsync(url => url.EndsWith("/abrechnung") || url.EndsWith("/abrechnung/"), new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task BillingTabs_VisibleOnAllSubpages()
    {
        // Tabs should be visible on Quartalsvalidierung
        await NavigateToAsync("/abrechnung/validierung");
        var tabs = Page.Locator(".billing-tabs");
        await Expect(tabs).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Tabs should be visible on Privatrechnungen
        await NavigateToAsync("/abrechnung/privatrechnungen");
        tabs = Page.Locator(".billing-tabs");
        await Expect(tabs).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    /// <summary>Helper to access Playwright Expect assertions.</summary>
    private static ILocatorAssertions Expect(ILocator locator)
        => Assertions.Expect(locator);
}
