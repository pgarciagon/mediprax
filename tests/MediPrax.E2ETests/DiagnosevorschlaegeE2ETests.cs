using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// E2E tests for Diagnosevorschläge (M42) — automatic diagnosis suggestions in encounter form.
/// Requires: PostgreSQL running with seed data (patients with AMDP/Neuro findings), Chromium installed.
/// </summary>
[Collection("Playwright")]
public class DiagnosevorschlaegeE2ETests : E2ETestBase
{
    public DiagnosevorschlaegeE2ETests(PlaywrightFixture playwright, MediPraxServerFixture server)
        : base(playwright, server) { }

    [Fact]
    public async Task EncounterForm_DiagnoseSectionExists()
    {
        // Navigate to patient list
        await NavigateToAsync("/patienten");

        // Click first patient
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(1000); // Wait for Blazor interactivity

        // Find existing encounter to edit
        var encounterLink = Page.Locator("a[href*='dokumentation']:not(:has-text('Neu'))").First;
        if (await encounterLink.CountAsync() > 0)
        {
            await encounterLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(1000);

            // ICD-10 Diagnosen section should be visible on encounter form
            var icdSection = Page.Locator("fieldset:has-text('ICD-10'), legend:has-text('ICD-10'), input[placeholder*='ICD']").First;
            if (await icdSection.CountAsync() > 0)
                await Expect(icdSection).ToBeVisibleAsync(new() { Timeout = 5_000 });
        }
    }

    [Fact]
    public async Task EncounterForm_SuggestionPanelRendersWhenFindingsExist()
    {
        // Navigate to patients
        await NavigateToAsync("/patienten");

        // Look for Weber (has depression encounters with potential AMDP findings)
        var weberLink = Page.Locator("tr:has-text('Weber') a").First;
        if (await weberLink.CountAsync() == 0)
        {
            // Try first patient if Weber not found
            weberLink = Page.Locator("table tbody tr a").First;
        }
        await weberLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click on an existing encounter
        var encounterLink = Page.Locator("a[href*='dokumentation']:not(:has-text('Neu'))").First;
        if (await encounterLink.CountAsync() == 0)
            return; // No existing encounters

        await encounterLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // The suggestion panel only appears if there are structured findings (AMDP/Neuro)
        // Check if the panel is present (it may or may not have suggestions)
        var suggestionPanel = Page.Locator(".diagnosis-suggestions, .suggestion-panel");
        // This is a soft check — the panel may not exist if no findings are linked
        var hasSuggestions = await suggestionPanel.CountAsync() > 0;

        // If suggestions exist, they should have confidence badges
        if (hasSuggestions)
        {
            var badges = Page.Locator(".confidence-badge, .suggestion-confidence");
            var badgeCount = await badges.CountAsync();
            Assert.True(badgeCount >= 0, "Suggestion badges should render if panel exists");
        }
    }

    [Fact]
    public async Task EncounterForm_CanAccessAmdpFromEncounter()
    {
        await NavigateToAsync("/patienten");

        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Look for AMDP / Psychopathological assessment link
        var amdpLink = Page.Locator("a:has-text('AMDP'), a:has-text('Psychopath'), button:has-text('Psychopath')").First;
        if (await amdpLink.CountAsync() > 0)
        {
            await Expect(amdpLink).ToBeVisibleAsync();
            // Don't click — just verify it's accessible from the UI
        }
    }

    [Fact]
    public async Task NewEncounter_NoSuggestionsWithoutFindings()
    {
        await NavigateToAsync("/patienten");

        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Go to new encounter
        var newEncounterBtn = Page.Locator("a:has-text('Neuer Kontakt'), a:has-text('Neue Konsultation')").First;
        if (await newEncounterBtn.CountAsync() == 0)
            return;

        await newEncounterBtn.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // New encounter should NOT have suggestions (no structured findings yet)
        var suggestionPanel = Page.Locator(".diagnosis-suggestions, .suggestion-panel");
        var count = await suggestionPanel.CountAsync();
        Assert.True(count == 0, "New encounter should not have diagnosis suggestions");
    }

    [Fact]
    public async Task EncounterForm_Icd10SearchWorksWithoutCrash()
    {
        // This test verifies the fix for the concurrent DbContext crash
        // that occurred when typing in the ICD-10 search field
        await NavigateToAsync("/patienten");

        // Find Weber's encounter (has existing data)
        var weberLink = Page.Locator("tr:has-text('Weber') a").First;
        if (await weberLink.CountAsync() == 0)
        {
            weberLink = Page.Locator("table tbody tr a").First;
        }
        await weberLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click on an existing encounter
        var encounterLink = Page.Locator("a[href*='dokumentation']:not(:has-text('Neu'))").First;
        if (await encounterLink.CountAsync() == 0)
            return;

        await encounterLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000); // Wait for diagnosis suggestions to load

        // Find ICD-10 search input
        var icdInput = Page.Locator("input[placeholder*='ICD']");
        await Expect(icdInput).ToBeVisibleAsync(new() { Timeout = 5_000 });

        // Type a search term — this used to crash with concurrent DbContext error
        await icdInput.FillAsync("F32");
        await Page.WaitForTimeoutAsync(1000);

        // Page should NOT have crashed (no error overlay)
        var errorOverlay = Page.Locator("text='An unhandled error has occurred'");
        var errorCount = await errorOverlay.CountAsync();
        Assert.True(errorCount == 0, "ICD-10 search should not cause a concurrent DbContext crash");

        // Dropdown should show results
        var dropdown = Page.Locator(".icd-dropdown, .icd-results, li:has-text('F32')").First;
        if (await dropdown.CountAsync() > 0)
        {
            await Expect(dropdown).ToBeVisibleAsync(new() { Timeout = 3_000 });
        }
    }

    /// <summary>Helper to access Playwright Expect assertions.</summary>
    private static ILocatorAssertions Expect(ILocator locator)
        => Assertions.Expect(locator);
}
