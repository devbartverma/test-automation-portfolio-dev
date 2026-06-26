using System.Globalization;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class InventoryTests : BasePageTest
{
    private InventoryPage _inventoryPage = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);

        _inventoryPage = new InventoryPage(Page);
    }

    [Test]
    [Description("shows six inventory products")]
    public async Task ShowsSixInventoryProducts()
    {
        await _inventoryPage.AssertProductCountAsync(6);
    }

    [Test]
    [Description("sorts products by price high to low")]
    public async Task SortsProductsByPriceHighToLow()
    {
        await _inventoryPage.SortByAsync(SortOptions.PriceHiLo);
        var prices = await _inventoryPage.AssertPricesSortedDescendingAsync();
        Assert.That(prices[0], Is.GreaterThan(prices[^1]));
    }

    [Test]
    [Description("sorts products by name A to Z")]
    public async Task SortsProductsByNameAToZ()
    {
        await _inventoryPage.SortByAsync(SortOptions.NameAZ);
        await _inventoryPage.AssertNamesAlphabeticallyAscendingAsync();
    }

    [Test]
    [Description("opens product detail page for Sauce Labs Backpack")]
    public async Task OpensProductDetailPageForBackpack()
    {
        await _inventoryPage.OpenProductDetailAsync(Products.Backpack.Name);

        var detailPage = new ProductDetailPage(Page);
        Assert.That(await detailPage.ProductName.TextContentAsync(), Is.EqualTo(Products.Backpack.Name));
        Assert.That(await detailPage.ProductPrice.TextContentAsync(),
            Is.EqualTo($"${Products.Backpack.Price.ToString(CultureInfo.InvariantCulture)}"));
        Assert.That(await detailPage.ProductDescription.TextContentAsync(), Is.Not.Empty);
        Assert.That(await detailPage.BackButton.IsVisibleAsync(), Is.True);
    }
}
