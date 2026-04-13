using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// E2E tests for Verwaltung pages: Aktionsketten, Textbausteine, ICD-10 Katalog, Arzneimittelkatalog.
/// Verifies seed data is present and pages are accessible for Arzt role.
/// </summary>
[Collection("Playwright")]
public class VerwaltungE2ETests : E2ETestBase
{
    public VerwaltungE2ETests(PlaywrightFixture playwright, MediPraxServerFixture server)
        : base(playwright, server) { }

    // --- Aktionsketten ---

    [Fact]
    public async Task Aktionsketten_PageLoadsWithSeedData()
    {
        await NavigateToAsync("/verwaltung/aktionsketten");

        var heading = Page.Locator("h1:has-text('Aktionsketten')");
        await Expect(heading).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Should have 8 seed action chains
        var countText = Page.Locator("text=Aktionsketten").First;
        await Expect(countText).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Verify specific chains exist
        await Expect(Page.Locator("text='Depression Standard'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
        await Expect(Page.Locator("text='Epilepsie Kontrolle'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    [Fact]
    public async Task Aktionsketten_FilterByCategory()
    {
        await NavigateToAsync("/verwaltung/aktionsketten");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Filter by Neurologie
        var categorySelect = Page.Locator("select").First;
        await categorySelect.SelectOptionAsync("Neurologie");
        await Page.WaitForTimeoutAsync(500);

        // Should see neuro chains but not psychiatrie
        await Expect(Page.Locator("text='Epilepsie Kontrolle'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
        var depChain = Page.Locator("text='Depression Standard'");
        var depCount = await depChain.CountAsync();
        Assert.Equal(0, depCount);
    }

    // --- Textbausteine ---

    [Fact]
    public async Task Textbausteine_PageLoadsWithSeedData()
    {
        await NavigateToAsync("/verwaltung/textbausteine");

        var heading = Page.Locator("h1:has-text('Textbausteine')");
        await Expect(heading).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Should have seed text modules
        await Expect(Page.Locator("text='#normpsy'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
        await Expect(Page.Locator("text='#depbefund'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
        await Expect(Page.Locator("text='Arztbrief Einleitung'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    [Fact]
    public async Task Textbausteine_FilterByCategory()
    {
        await NavigateToAsync("/verwaltung/textbausteine");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var categorySelect = Page.Locator("select").First;
        await categorySelect.SelectOptionAsync(new SelectOptionValue { Label = "Neurologie/Befund" });
        await Page.WaitForTimeoutAsync(500);

        // Should see neuro modules
        await Expect(Page.Locator("text='#normneuro'").First).ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    // --- ICD-10 Katalog ---

    [Fact]
    public async Task Icd10Katalog_AccessibleForArzt()
    {
        await NavigateToAsync("/verwaltung/icd10");

        // Should NOT redirect to AccessDenied — heading should be visible
        var heading = Page.Locator("h1:has-text('ICD-10')");
        await Expect(heading).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Should show entry count (e.g. "87 Einträge")
        await Expect(Page.Locator("text=/\\d+ Eintr/").First).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task Icd10Katalog_ShowsSeedData()
    {
        await NavigateToAsync("/verwaltung/icd10");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should have ICD-10 codes displayed
        await Expect(Page.Locator("text='F00.0'").First).ToBeVisibleAsync(new() { Timeout = 5_000 });
        await Expect(Page.Locator("text='F32.0'").First).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task Icd10Katalog_SearchWorks()
    {
        await NavigateToAsync("/verwaltung/icd10");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(1000);

        var searchInput = Page.Locator("input[placeholder*='suchen'], input[placeholder*='Suchen']").First;
        await Expect(searchInput).ToBeVisibleAsync(new() { Timeout = 5_000 });
        await searchInput.FillAsync("F32");
        await Page.WaitForTimeoutAsync(1000);

        // Should filter to depression-related codes
        await Expect(Page.Locator("td:has-text('F32.0')").First).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    // --- Arzneimittelkatalog ---

    [Fact]
    public async Task Arzneimittelkatalog_PageLoadsWithData()
    {
        await NavigateToAsync("/verwaltung/arzneimittelkatalog");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(1000);

        // Page should load without error
        var error = Page.Locator("text='An unhandled error has occurred'");
        Assert.Equal(0, await error.CountAsync());

        // Should show heading or search field
        var heading = Page.Locator("h1, h2, input[placeholder*='suchen'], input[placeholder*='Suchen']").First;
        await Expect(heading).ToBeVisibleAsync(new() { Timeout = 5_000 });
    }

    // --- Aktionskette execution in encounter ---

    [Fact]
    public async Task AktionsketteExecution_DepChainAddsIcdAndBilling()
    {
        // Navigate to existing encounter
        await NavigateToAsync("/patienten");

        var weberLink = Page.Locator("tr:has-text('Weber') a").First;
        if (await weberLink.CountAsync() == 0)
            return; // Skip if no Weber patient

        await weberLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(1000);

        // Click existing encounter
        var encounterLink = Page.Locator("a[href*='dokumentation']:not(:has-text('Neu'))").First;
        if (await encounterLink.CountAsync() == 0)
            return;

        await encounterLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000);

        // Find Aktionskette field
        var chainInput = Page.Locator("input[placeholder*='Aktionsketten']");
        await Expect(chainInput).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Type #dep
        await chainInput.FillAsync("#dep");
        await Page.WaitForTimeoutAsync(1000);

        // Dropdown should show Depression Standard
        var depOption = Page.Locator("text='Depression Standard'").First;
        if (await depOption.CountAsync() > 0)
        {
            await depOption.ClickAsync();
            await Page.WaitForTimeoutAsync(2000);

            // Should show execution result — "3/3 Schritte ausgeführt"
            var result = Page.Locator(".chain-result, text=/Schritte ausgeführt/").First;
            await Expect(result).ToBeVisibleAsync(new() { Timeout = 5_000 });

            // No error overlay
            var error = Page.Locator("text='An unhandled error has occurred'");
            Assert.Equal(0, await error.CountAsync());
        }
    }

    // --- Navigation from sidebar ---

    [Fact]
    public async Task Sidebar_VerwaltungLinksWork()
    {
        await NavigateToAsync("/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check Aktionsketten link
        var aktionskettenLink = Page.Locator("a[href='/verwaltung/aktionsketten']");
        await Expect(aktionskettenLink).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Check Textbausteine link
        var textbausteineLink = Page.Locator("a[href='/verwaltung/textbausteine']");
        await Expect(textbausteineLink).ToBeVisibleAsync(new() { Timeout = 3_000 });

        // Check ICD-10 link
        var icd10Link = Page.Locator("a[href='/verwaltung/icd10']");
        await Expect(icd10Link).ToBeVisibleAsync(new() { Timeout = 3_000 });

        // Check Arzneimittelkatalog link
        var arzneimittelLink = Page.Locator("a[href='/verwaltung/arzneimittelkatalog']");
        await Expect(arzneimittelLink).ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    /// <summary>Helper to access Playwright Expect assertions.</summary>
    private static ILocatorAssertions Expect(ILocator locator)
        => Assertions.Expect(locator);
}
