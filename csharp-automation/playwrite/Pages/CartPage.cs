using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

public class CartPage
{
    private readonly IPage _page;

    public CartPage(IPage page)
    {
        _page = page;
    }

    public ILocator ItemNames => _page.Locator(".inventory_item_name");
    public ILocator CheckoutButton => _page.Locator("[data-test='checkout']");
    public ILocator ContinueShoppingButton => _page.Locator("[data-test='continue-shopping']");

    public async Task GoToAsync()
    {
        await _page.GotoAsync(Urls.Cart);
        Assert.That(_page.Url, Is.EqualTo(Urls.Cart));
    }

    public async Task AssertItemInCartAsync(string productName)
    {
        Assert.That(await _page.Locator(".inventory_item_name", new PageLocatorOptions { HasTextString = productName }).CountAsync(), Is.GreaterThan(0));
    }

    public async Task ProceedToCheckoutAsync()
    {
        await CheckoutButton.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.CheckoutStep1));
    }

    public async Task ContinueShoppingAsync()
    {
        await ContinueShoppingButton.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.Inventory));
    }
}
