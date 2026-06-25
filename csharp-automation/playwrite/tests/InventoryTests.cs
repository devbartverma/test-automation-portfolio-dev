using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class InventoryTests : PageTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);

        _inventoryPage = new InventoryPage(Page);
    }

    [Test]
    public async Task ShowsSixProducts()
    {
        await _inventoryPage.AssertProductCountAsync(6);
    }

    [Test]
    public async Task CanAddProductToCart()
    {
        await _inventoryPage.AddToCartByDataTestAsync("add-to-cart-sauce-labs-backpack");
        await _inventoryPage.AssertCartBadgeCountAsync(1);
    }
}
