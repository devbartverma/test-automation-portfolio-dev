using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

public class ProductDetailPage
{
    private readonly IPage _page;

    public ProductDetailPage(IPage page)
    {
        _page = page;
    }

    public ILocator ProductName => _page.Locator(".inventory_details_name");
    public ILocator ProductPrice => _page.Locator(".inventory_details_price");
    public ILocator ProductDescription => _page.Locator(".inventory_details_desc");
    public ILocator AddToCartButton => _page.Locator("[data-test='add-to-cart']");
    public ILocator BackButton => _page.Locator("[data-test='back-to-products']");

    public async Task AssertProductDetailAsync(string expectedName, string expectedPrice)
    {
        Assert.That(await ProductName.TextContentAsync(), Is.EqualTo(expectedName));
        Assert.That(await ProductPrice.TextContentAsync(), Is.EqualTo(expectedPrice));
        Assert.That(await ProductDescription.IsVisibleAsync(), Is.True);
    }

    public async Task AddToCartAsync()
    {
        await AddToCartButton.ClickAsync();
    }

    public async Task BackToProductsAsync()
    {
        await BackButton.ClickAsync();
        Assert.That(_page.Url, Is.EqualTo(Urls.Inventory));
    }
}
