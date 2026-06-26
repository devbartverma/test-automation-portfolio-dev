using System.Globalization;
using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

/// <summary>
/// CheckoutPage — covers Step 1 (info), Step 2 (overview), and confirmation.
/// </summary>
public class CheckoutPage
{
    private readonly IPage _page;

    public CheckoutPage(IPage page)
    {
        _page = page;
    }

    public ILocator FirstNameInput => _page.Locator("[data-test='firstName']");
    public ILocator LastNameInput => _page.Locator("[data-test='lastName']");
    public ILocator PostalCodeInput => _page.Locator("[data-test='postalCode']");
    public ILocator ContinueButton => _page.Locator("[data-test='continue']");
    public ILocator CancelButton => _page.Locator("[data-test='cancel']");
    public ILocator ErrorMessage => _page.Locator("[data-test='error']");

    public ILocator SubtotalLabel => _page.Locator(".summary_subtotal_label");
    public ILocator TaxLabel => _page.Locator(".summary_tax_label");
    public ILocator TotalLabel => _page.Locator(".summary_total_label");
    public ILocator FinishButton => _page.Locator("[data-test='finish']");

    public ILocator CompleteHeader => _page.Locator("[data-test='complete-header']");
    public ILocator CompleteText => _page.Locator("[data-test='complete-text']");
    public ILocator BackHomeButton => _page.Locator("[data-test='back-to-products']");

    // --- Step 1 actions -----------------------------------------------------

    public async Task FillCustomerInfoAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueToOverviewAsync()
    {
        await ContinueButton.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.CheckoutStep2));
    }

    public async Task FillAndContinueAsync(string firstName, string lastName, string postalCode)
    {
        await FillCustomerInfoAsync(firstName, lastName, postalCode);
        await ContinueToOverviewAsync();
    }

    /// <summary>Submit empty form to trigger validation; does not assert navigation.</summary>
    public async Task SubmitEmptyFormAsync()
    {
        await ContinueButton.ClickAsync();
    }

    // --- Step 2 actions -----------------------------------------------------

    public async Task FinishOrderAsync()
    {
        await FinishButton.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.CheckoutComplete));
    }

    /// <summary>Returns (Subtotal, Tax, Total) as numbers for math verification.</summary>
    public async Task<(double Subtotal, double Tax, double Total)> GetPriceSummaryAsync()
    {
        var subtotalText = await SubtotalLabel.TextContentAsync() ?? string.Empty;
        var taxText = await TaxLabel.TextContentAsync() ?? string.Empty;
        var totalText = await TotalLabel.TextContentAsync() ?? string.Empty;

        var subtotal = ParseAmount(subtotalText, "Item total: $");
        var tax = ParseAmount(taxText, "Tax: $");
        var total = ParseAmount(totalText, "Total: $");

        return (subtotal, tax, total);
    }

    // --- Assertions ---------------------------------------------------------

    public async Task AssertStep1ErrorAsync(string expectedText)
    {
        Assert.That(await ErrorMessage.IsVisibleAsync(), Is.True);
        Assert.That(await ErrorMessage.TextContentAsync(), Does.Contain(expectedText));
    }

    /// <summary>Core financial assertion: subtotal + tax must equal total (within 0.01).</summary>
    public async Task AssertTotalCalculationIsCorrectAsync()
    {
        var (subtotal, tax, total) = await GetPriceSummaryAsync();
        Assert.That(subtotal + tax, Is.EqualTo(total).Within(0.01),
            $"Expected subtotal ({subtotal}) + tax ({tax}) = {subtotal + tax}, got {total}");
    }

    public async Task AssertConfirmationPageAsync()
    {
        Assert.That(_page.Url, Is.EqualTo(Urls.CheckoutComplete));
        Assert.That(await CompleteHeader.IsVisibleAsync(), Is.True);
        Assert.That(await CompleteHeader.TextContentAsync(), Is.EqualTo("Thank you for your order!"));
    }

    private static double ParseAmount(string text, string prefix)
    {
        var cleaned = text.Replace(prefix, string.Empty).Trim();
        return double.Parse(cleaned, CultureInfo.InvariantCulture);
    }
}
