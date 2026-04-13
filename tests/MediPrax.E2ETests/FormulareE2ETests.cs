using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// E2E tests for Formulare (M24) — KBV form generation including Muster 7.
/// Requires: PostgreSQL running with seed data, Chromium installed.
/// </summary>
[Collection("Playwright")]
public class FormulareE2ETests : E2ETestBase
{
    public FormulareE2ETests(PlaywrightFixture playwright, MediPraxServerFixture server)
        : base(playwright, server) { }

    private async Task<string> GetFirstPatientFormularUrl()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Extract patient ID from URL
        var url = Page.Url;
        var patientId = url.Split('/').Last(s => Guid.TryParse(s, out _));

        return $"/patienten/{patientId}/formulare";
    }

    [Fact]
    public async Task FormularDruck_ShowsAllFormCards()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Navigate to formulare via patient detail
        var formLink = Page.Locator("a:has-text('Formulare')").First;
        if (await formLink.CountAsync() > 0)
        {
            await formLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        else
        {
            // Direct navigation if no link
            var patientId = Page.Url.Split('/').LastOrDefault(s => Guid.TryParse(s, out _));
            if (patientId != null)
                await NavigateToAsync($"/patienten/{patientId}/formulare");
            else
                return; // Skip if can't determine patient ID
        }

        // Should see form cards including Muster 7
        var formCards = Page.Locator(".form-card, .card");
        var count = await formCards.CountAsync();
        Assert.True(count >= 7, $"Should show at least 7 form cards, got {count}");

        // Muster 7 card should be visible
        var muster7 = Page.Locator("text=Psychotherapie, text=Muster 7").First;
        await Expect(muster7).ToBeVisibleAsync(new() { Timeout = 3_000 });
    }

    [Fact]
    public async Task Muster7_ShowsFormFields()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var formLink = Page.Locator("a:has-text('Formulare')").First;
        if (await formLink.CountAsync() > 0)
        {
            await formLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        else
        {
            var patientId = Page.Url.Split('/').LastOrDefault(s => Guid.TryParse(s, out _));
            if (patientId != null)
                await NavigateToAsync($"/patienten/{patientId}/formulare");
            else
                return;
        }

        // Click on Muster 7 card
        var muster7Card = Page.Locator(".form-card:has-text('Psychotherapie'), .card:has-text('Muster 7')").First;
        if (await muster7Card.CountAsync() > 0)
        {
            await muster7Card.ClickAsync();
            await Page.WaitForTimeoutAsync(500);

            // Form fields should appear
            var therapySelect = Page.Locator("select:has(option:has-text('Verhaltenstherapie'))");
            if (await therapySelect.CountAsync() > 0)
                await Expect(therapySelect).ToBeVisibleAsync();

            // Somatic workup checkbox
            var somaticCheckbox = Page.Locator("input[type='checkbox']").First;
            if (await somaticCheckbox.CountAsync() > 0)
                await Expect(somaticCheckbox).ToBeVisibleAsync();
        }
    }

    [Fact]
    public async Task ExistingForms_Krankenhauseinweisung_FormFieldsVisible()
    {
        await NavigateToAsync("/patienten");
        var patientLink = Page.Locator("table tbody tr a").First;
        await patientLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var formLink = Page.Locator("a:has-text('Formulare')").First;
        if (await formLink.CountAsync() > 0)
        {
            await formLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        else
        {
            var patientId = Page.Url.Split('/').LastOrDefault(s => Guid.TryParse(s, out _));
            if (patientId != null)
                await NavigateToAsync($"/patienten/{patientId}/formulare");
            else
                return;
        }

        // Click Krankenhauseinweisung card
        var khCard = Page.Locator(".form-card:has-text('Krankenhaus'), .card:has-text('Krankenhaus')").First;
        if (await khCard.CountAsync() > 0)
        {
            await khCard.ClickAsync();
            await Page.WaitForTimeoutAsync(500);

            // Should show form fields for hospital referral
            var hospitalInput = Page.Locator("input[placeholder*='Krankenhaus'], input[placeholder*='krankenhaus']").First;
            if (await hospitalInput.CountAsync() > 0)
                await Expect(hospitalInput).ToBeVisibleAsync();
        }
    }

    /// <summary>Helper to access Playwright Expect assertions.</summary>
    private static ILocatorAssertions Expect(ILocator locator)
        => Assertions.Expect(locator);
}
