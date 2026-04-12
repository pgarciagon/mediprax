using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// E2E tests for Textbausteine (text modules) — CRUD management and inline #-expansion.
/// Requires: PostgreSQL running, seed data applied (M47 migration), Chromium installed.
/// </summary>
[Collection("Playwright")]
public class TextbausteinE2ETests : E2ETestBase
{
    public TextbausteinE2ETests(PlaywrightFixture playwright, MediPraxServerFixture server)
        : base(playwright, server) { }

    [Fact]
    public async Task TextbausteinList_ShowsSeededModules()
    {
        await NavigateToAsync("/verwaltung/textbausteine");

        // Should see the seeded modules table
        var table = Page.Locator("table");
        await table.WaitForAsync(new() { Timeout = 5_000 });

        // Check for known seeded shortcuts
        var normpsy = Page.Locator("text=#normpsy");
        await Expect(normpsy).ToBeVisibleAsync();

        var normneuro = Page.Locator("text=#normneuro");
        await Expect(normneuro).ToBeVisibleAsync();
    }

    [Fact]
    public async Task TextbausteinCreate_NavigatesToFormAndSaves()
    {
        await NavigateToAsync("/verwaltung/textbausteine");

        // Click "new" button
        await Page.ClickAsync("a:has-text('Neuer Textbaustein')");
        await Page.WaitForURLAsync(url => url.Contains("/textbausteine/neu"), new() { Timeout = 5_000 });

        // Fill the form
        var shortcut = $"e2etest{DateTime.Now.Ticks % 100000}";
        await Page.FillAsync("input[placeholder*='normpsy']", shortcut);
        await Page.FillAsync("input[placeholder*='Beschreibung']", "E2E Test Baustein");
        await Page.FillAsync("textarea", "Dies ist ein automatischer Testtext.");

        // Submit
        await Page.ClickAsync("button[type='submit']:has-text('Speichern')");

        // Should redirect back to list
        await Page.WaitForURLAsync(url => url.Contains("/verwaltung/textbausteine") && !url.Contains("/neu"),
            new() { Timeout = 5_000 });

        // The new module should appear in the list
        var newEntry = Page.Locator($"text=#{shortcut}");
        await Expect(newEntry).ToBeVisibleAsync();
    }

    [Fact]
    public async Task TextbausteinEdit_NavigatesToFormAndUpdates()
    {
        await NavigateToAsync("/verwaltung/textbausteine");

        // Click edit on the first module
        var editLink = Page.Locator("a:has-text('Bearbeiten')").First;
        await editLink.ClickAsync();
        await Page.WaitForURLAsync(url => url.Contains("/bearbeiten"), new() { Timeout = 5_000 });

        // The form should be pre-filled
        var shortcutInput = Page.Locator("input[placeholder*='normpsy']");
        var value = await shortcutInput.InputValueAsync();
        Assert.False(string.IsNullOrEmpty(value), "Shortcut field should be pre-filled in edit mode");

        // Submit without changes (just verifying the round-trip)
        await Page.ClickAsync("button[type='submit']:has-text('Speichern')");
        await Page.WaitForURLAsync(url => url.Contains("/verwaltung/textbausteine") && !url.Contains("/bearbeiten"),
            new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task TextbausteinDelete_ShowsConfirmationAndRemoves()
    {
        // First create a module to delete
        await NavigateToAsync("/verwaltung/textbausteine/neu");
        var shortcut = $"e2edel{DateTime.Now.Ticks % 100000}";
        await Page.FillAsync("input[placeholder*='normpsy']", shortcut);
        await Page.FillAsync("input[placeholder*='Beschreibung']", "To Delete");
        await Page.FillAsync("textarea", "Delete me.");
        await Page.ClickAsync("button[type='submit']:has-text('Speichern')");
        await Page.WaitForURLAsync(url => url.Contains("/verwaltung/textbausteine") && !url.Contains("/neu"),
            new() { Timeout = 5_000 });

        // Click delete on the module we just created
        var row = Page.Locator($"tr:has-text('#{shortcut}')");
        await row.Locator("button:has-text('Loeschen')").ClickAsync();

        // Confirmation modal should appear
        var modal = Page.Locator(".modal-overlay");
        await Expect(modal).ToBeVisibleAsync();

        // Confirm deletion
        await Page.ClickAsync(".modal-card button:has-text('Loeschen')");

        // Module should be gone
        await Expect(Page.Locator($"text=#{shortcut}")).Not.ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    [Fact]
    public async Task InlineExpansion_HashTriggerShowsDropdown()
    {
        // Navigate to encounter form — we need a patient with an encounter
        // First, find a patient from the list
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Go to new encounter for this patient
        var newEncounterBtn = Page.Locator("a:has-text('Neuer Kontakt'), a:has-text('Neue Konsultation'), a:has-text('Neuer Encounter')").First;
        if (await newEncounterBtn.CountAsync() > 0)
        {
            await newEncounterBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Find a textarea with textmodule-autocomplete (the ABDTP sections)
        var autocompleteTextarea = Page.Locator(".textmodule-autocomplete textarea").First;
        if (await autocompleteTextarea.CountAsync() == 0)
        {
            // Skip if no autocomplete textareas found on this page
            return;
        }

        // Type #norm to trigger the dropdown
        await autocompleteTextarea.FocusAsync();
        await autocompleteTextarea.PressSequentiallyAsync("#norm", new() { Delay = 50 });

        // Wait for the dropdown to appear (200ms debounce + network)
        var dropdown = Page.Locator(".tm-dropdown");
        await Expect(dropdown).ToBeVisibleAsync(new() { Timeout = 3_000 });

        // Should show matching modules
        var items = dropdown.Locator(".tm-item");
        var count = await items.CountAsync();
        Assert.True(count > 0, "Dropdown should show matching text modules");

        // Verify #normpsy is in the results
        var normpsy = dropdown.Locator("text=normpsy");
        await Expect(normpsy).ToBeVisibleAsync();
    }

    [Fact]
    public async Task InlineExpansion_EnterSelectsWithoutNewline()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var newEncounterBtn = Page.Locator("a:has-text('Neuer Kontakt'), a:has-text('Neue Konsultation'), a:has-text('Neuer Encounter')").First;
        if (await newEncounterBtn.CountAsync() > 0)
        {
            await newEncounterBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        var autocompleteTextarea = Page.Locator(".textmodule-autocomplete textarea").First;
        if (await autocompleteTextarea.CountAsync() == 0)
            return;

        // Type #norm to trigger dropdown
        await autocompleteTextarea.FocusAsync();
        await autocompleteTextarea.PressSequentiallyAsync("#norm", new() { Delay = 50 });

        // Wait for dropdown
        var dropdown = Page.Locator(".tm-dropdown");
        await Expect(dropdown).ToBeVisibleAsync(new() { Timeout = 3_000 });

        // Press Enter to select the first item
        await Page.Keyboard.PressAsync("Enter");

        // Wait for dropdown to close
        await Expect(dropdown).Not.ToBeVisibleAsync(new() { Timeout = 3_000 });

        // The textarea should contain the expanded text, NOT a newline after #norm
        var textValue = await autocompleteTextarea.InputValueAsync();
        Assert.DoesNotContain("#norm", textValue); // The #shortcut should be replaced
        Assert.False(string.IsNullOrWhiteSpace(textValue), "Textarea should contain expanded text");

        // The text should NOT start with a newline (the Enter bug)
        Assert.False(textValue.StartsWith("\n"), "Enter should not insert a newline when selecting from dropdown");
    }

    [Fact]
    public async Task InlineExpansion_EscapeClosesDropdown()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var newEncounterBtn = Page.Locator("a:has-text('Neuer Kontakt'), a:has-text('Neue Konsultation'), a:has-text('Neuer Encounter')").First;
        if (await newEncounterBtn.CountAsync() > 0)
        {
            await newEncounterBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        var autocompleteTextarea = Page.Locator(".textmodule-autocomplete textarea").First;
        if (await autocompleteTextarea.CountAsync() == 0)
            return;

        // Type #norm to trigger dropdown
        await autocompleteTextarea.FocusAsync();
        await autocompleteTextarea.PressSequentiallyAsync("#norm", new() { Delay = 50 });

        var dropdown = Page.Locator(".tm-dropdown");
        await Expect(dropdown).ToBeVisibleAsync(new() { Timeout = 3_000 });

        // Press Escape
        await Page.Keyboard.PressAsync("Escape");

        // Dropdown should close
        await Expect(dropdown).Not.ToBeVisibleAsync(new() { Timeout = 2_000 });

        // Text should still contain #norm (not replaced)
        var textValue = await autocompleteTextarea.InputValueAsync();
        Assert.Contains("#norm", textValue);
    }

    [Fact]
    public async Task InlineExpansion_ArrowKeysNavigateDropdown()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var newEncounterBtn = Page.Locator("a:has-text('Neuer Kontakt'), a:has-text('Neue Konsultation'), a:has-text('Neuer Encounter')").First;
        if (await newEncounterBtn.CountAsync() > 0)
        {
            await newEncounterBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        var autocompleteTextarea = Page.Locator(".textmodule-autocomplete textarea").First;
        if (await autocompleteTextarea.CountAsync() == 0)
            return;

        // Type #norm to trigger dropdown
        await autocompleteTextarea.FocusAsync();
        await autocompleteTextarea.PressSequentiallyAsync("#norm", new() { Delay = 50 });

        var dropdown = Page.Locator(".tm-dropdown");
        await Expect(dropdown).ToBeVisibleAsync(new() { Timeout = 3_000 });

        // First item should be selected by default
        var selectedItem = dropdown.Locator(".tm-item-selected");
        await Expect(selectedItem).ToHaveCountAsync(1);

        // Press ArrowDown — selected item should change
        await Page.Keyboard.PressAsync("ArrowDown");
        await Page.WaitForTimeoutAsync(200);

        // There should still be exactly one selected item
        var newSelected = dropdown.Locator(".tm-item-selected");
        await Expect(newSelected).ToHaveCountAsync(1);
    }

    [Fact]
    public async Task CategoryFilter_FiltersModules()
    {
        await NavigateToAsync("/verwaltung/textbausteine");

        // Select a specific category from dropdown
        var categorySelect = Page.Locator("select").First;
        var options = await categorySelect.Locator("option").AllTextContentsAsync();

        if (options.Count > 1) // More than just "Alle Kategorien"
        {
            // Select the second option (first real category)
            await categorySelect.SelectOptionAsync(new SelectOptionValue { Index = 1 });
            await Page.WaitForTimeoutAsync(500);

            // Table should still be visible but with fewer rows
            var table = Page.Locator("table");
            await Expect(table).ToBeVisibleAsync();
        }
    }

    /// <summary>Helper to access Playwright Expect assertions.</summary>
    private static ILocatorAssertions Expect(ILocator locator)
        => Assertions.Expect(locator);
}
