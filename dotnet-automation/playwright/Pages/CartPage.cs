using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

/// <summary>
/// CartPage — asserts item presence, quantities, and initiates checkout.
/// </summary>
public class CartPage
{
    private readonly IPage _page;

    public CartPage(IPage page)
    {
        _page = page;
    }

    public ILocator ItemNames => _page.Locator(".inventory_item_name");
    public ILocator ItemPrices => _page.Locator(".inventory_item_price");
    public ILocator ItemQuantities => _page.Locator(".cart_quantity");
    public ILocator ContinueShoppingButton => _page.Locator("[data-test='continue-shopping']");
    public ILocator CheckoutButton => _page.Locator("[data-test='checkout']");

    public async Task GoToAsync()
    {
        await _page.GotoAsync(Urls.Cart);
        Assert.That(_page.Url, Is.EqualTo(Urls.Cart));
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

    /// <summary>Remove an item by building its remove- data-test id from the product name.</summary>
    public async Task RemoveItemAsync(string productName)
    {
        var id = Regex.Replace(productName.ToLowerInvariant(), "[()]", string.Empty);
        id = Regex.Replace(id, "\\s+", "-");
        await _page.Locator($"[data-test='remove-{id}']").ClickAsync();
    }

    // --- Assertions ---------------------------------------------------------

    public async Task AssertItemInCartAsync(string productName)
    {
        Assert.That(await _page.GetByText(productName, new() { Exact = true }).IsVisibleAsync(), Is.True);
    }

    public async Task AssertItemNotInCartAsync(string productName)
    {
        Assert.That(await _page.GetByText(productName, new() { Exact = true }).IsVisibleAsync(), Is.False);
    }

    public async Task AssertItemCountAsync(int expected)
    {
        Assert.That(await ItemNames.CountAsync(), Is.EqualTo(expected));
    }

    public async Task AssertCartIsEmptyAsync()
    {
        Assert.That(await ItemNames.CountAsync(), Is.EqualTo(0));
    }

    /// <summary>Retrieve all cart prices as numbers for downstream math assertions.</summary>
    public async Task<List<double>> GetCartPricesAsync()
    {
        var raw = await ItemPrices.AllTextContentsAsync();
        return raw
            .Select(p => double.Parse(p.Replace("$", string.Empty).Trim(),
                System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
    }
}
