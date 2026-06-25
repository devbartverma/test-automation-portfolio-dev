using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

public class InventoryPage
{
    private readonly IPage _page;

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public ILocator ProductNames => _page.Locator(".inventory_item_name");
    public ILocator ProductPrices => _page.Locator(".inventory_item_price");
    public ILocator CartBadge => _page.Locator(".shopping_cart_badge");
    public ILocator CartLink => _page.Locator(".shopping_cart_link");
    public ILocator SortDropdown => _page.Locator("[data-test='product-sort-container']");

    public async Task GoToAsync() => await _page.GotoAsync(Urls.Inventory);

    public async Task AddToCartByDataTestAsync(string dataTestId)
    {
        await _page.Locator($"[data-test='{dataTestId}']").ClickAsync();
    }

    public async Task GoToCartAsync()
    {
        await CartLink.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.Cart));
    }

    public async Task AssertProductCountAsync(int expected)
    {
        Assert.That(await ProductNames.CountAsync(), Is.EqualTo(expected));
    }

    public async Task AssertCartBadgeCountAsync(int expected)
    {
        Assert.That(await CartBadge.TextContentAsync(), Is.EqualTo(expected.ToString()));
    }

    public async Task SelectSortAsync(string option)
    {
        await SortDropdown.SelectOptionAsync(option);
    }
}
