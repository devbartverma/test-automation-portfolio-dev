using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

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
    public ILocator FinishButton => _page.Locator("[data-test='finish']");
    public ILocator ErrorMessage => _page.Locator("[data-test='error']");
    public ILocator CompleteHeader => _page.Locator("[data-test='complete-header']");

    public async Task FillCustomerInfoAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueAsync()
    {
        await ContinueButton.ClickAsync();
    }

    public async Task FinishOrderAsync()
    {
        await FinishButton.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.CheckoutComplete));
    }

    public async Task AssertStep1ErrorAsync(string expectedText)
    {
        Assert.That(await ErrorMessage.TextContentAsync(), Does.Contain(expectedText));
    }
}
