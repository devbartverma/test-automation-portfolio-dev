using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

/// <summary>
/// InventoryPage — product listing, sorting, and add-to-cart interactions.
/// </summary>
public class InventoryPage
{
    private readonly IPage _page;

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public ILocator SortDropdown => _page.Locator("[data-test='product-sort-container']");
    public ILocator CartBadge => _page.Locator(".shopping_cart_badge");
    public ILocator CartLink => _page.Locator(".shopping_cart_link");
    public ILocator ProductItems => _page.Locator(".inventory_item");
    public ILocator ProductNames => _page.Locator(".inventory_item_name");
    public ILocator ProductPrices => _page.Locator(".inventory_item_price");
    public ILocator BurgerMenuButton => _page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
    public ILocator ResetAppStateLink => _page.GetByRole(AriaRole.Link, new() { Name = "Reset App State" });
    public ILocator LogoutLink => _page.GetByRole(AriaRole.Link, new() { Name = "Logout" });

    public async Task GoToAsync() => await _page.GotoAsync(Urls.Inventory);

    /// <summary>Select a sort option from the dropdown.</summary>
    public async Task SortByAsync(string option)
    {
        await SortDropdown.SelectOptionAsync(option);
    }

    /// <summary>Click the Add-to-Cart button for a product by its data-test id.</summary>
    public async Task AddToCartByDataTestAsync(string dataTestId)
    {
        await _page.Locator($"[data-test='{dataTestId}']").ClickAsync();
    }

    /// <summary>Add multiple products by passing a collection of data-test ids.</summary>
    public async Task AddMultipleToCartAsync(IEnumerable<string> dataTestIds)
    {
        foreach (var id in dataTestIds)
        {
            await AddToCartByDataTestAsync(id);
        }
    }

    /// <summary>Remove an item from the inventory page via its remove button data-test id.</summary>
    public async Task RemoveFromCartByDataTestAsync(string dataTestId)
    {
        await _page.Locator($"[data-test='{dataTestId}']").ClickAsync();
    }

    /// <summary>Click an item name to open the product detail page.</summary>
    public async Task OpenProductDetailAsync(string productName)
    {
        await _page.GetByText(productName, new() { Exact = true }).ClickAsync();
    }

    /// <summary>Navigate to cart.</summary>
    public async Task GoToCartAsync()
    {
        await CartLink.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.Cart));
    }

    /// <summary>Open burger menu and click Logout.</summary>
    public async Task LogoutAsync()
    {
        await BurgerMenuButton.ClickAsync();
        await LogoutLink.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.Base));
    }

    /// <summary>Open burger menu and reset app state (clears cart without navigating).</summary>
    public async Task ResetAppStateAsync()
    {
        await BurgerMenuButton.ClickAsync();
        await ResetAppStateLink.ClickAsync();
    }

    // --- Assertions ---------------------------------------------------------

    public async Task AssertProductCountAsync(int expected)
    {
        Assert.That(await ProductItems.CountAsync(), Is.EqualTo(expected));
    }

    public async Task AssertCartBadgeCountAsync(int expected)
    {
        Assert.That(await CartBadge.IsVisibleAsync(), Is.True);
        Assert.That(await CartBadge.TextContentAsync(), Is.EqualTo(expected.ToString()));
    }

    public async Task AssertCartBadgeNotVisibleAsync()
    {
        Assert.That(await CartBadge.IsVisibleAsync(), Is.False);
    }

    /// <summary>
    /// Retrieve all prices as numbers and assert they are sorted descending.
    /// Returns the parsed list so tests can do further assertions.
    /// </summary>
    public async Task<List<double>> AssertPricesSortedDescendingAsync()
    {
        var prices = await GetPricesAsync();
        var isSorted = prices.Zip(prices.Skip(1), (a, b) => a >= b).All(ordered => ordered);
        Assert.That(isSorted, Is.True, $"Prices not sorted descending: {string.Join(", ", prices)}");
        return prices;
    }

    public async Task<List<double>> AssertPricesSortedAscendingAsync()
    {
        var prices = await GetPricesAsync();
        var isSorted = prices.Zip(prices.Skip(1), (a, b) => a <= b).All(ordered => ordered);
        Assert.That(isSorted, Is.True, $"Prices not sorted ascending: {string.Join(", ", prices)}");
        return prices;
    }

    public async Task<List<string>> AssertNamesAlphabeticallyAscendingAsync()
    {
        var names = (await ProductNames.AllTextContentsAsync()).ToList();
        var sorted = names.OrderBy(n => n, StringComparer.Ordinal).ToList();
        Assert.That(names, Is.EqualTo(sorted));
        return names;
    }

    public async Task<List<string>> AssertNamesAlphabeticallyDescendingAsync()
    {
        var names = (await ProductNames.AllTextContentsAsync()).ToList();
        var sorted = names.OrderByDescending(n => n, StringComparer.Ordinal).ToList();
        Assert.That(names, Is.EqualTo(sorted));
        return names;
    }

    private async Task<List<double>> GetPricesAsync()
    {
        var raw = await ProductPrices.AllTextContentsAsync();
        return raw
            .Select(p => double.Parse(p.Replace("$", string.Empty).Trim(),
                System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
    }
}
