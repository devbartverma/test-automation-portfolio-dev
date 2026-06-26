using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class CartTests : BasePageTest
{
    private InventoryPage _inventoryPage = null!;
    private CartPage _cartPage = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);

        _inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
    }

    [Test]
    [Description("shows added items in cart with correct names")]
    public async Task ShowsAddedItemsInCartWithCorrectNames()
    {
        await _inventoryPage.AddMultipleToCartAsync(new[]
        {
            Products.FleeceJacket.DataTest,
            Products.Backpack.DataTest,
        });
        await _inventoryPage.GoToCartAsync();

        await _cartPage.AssertItemInCartAsync(Products.FleeceJacket.Name);
        await _cartPage.AssertItemInCartAsync(Products.Backpack.Name);
        await _cartPage.AssertItemCountAsync(2);
    }

    [Test]
    [Description("removes item from cart and updates count")]
    public async Task RemovesItemFromCartAndUpdatesCount()
    {
        await _inventoryPage.AddMultipleToCartAsync(new[]
        {
            Products.Backpack.DataTest,
            Products.BikeLight.DataTest,
        });
        await _inventoryPage.GoToCartAsync();

        await _cartPage.RemoveItemAsync(Products.Backpack.Name);

        await _cartPage.AssertItemNotInCartAsync(Products.Backpack.Name);
        await _cartPage.AssertItemCountAsync(1);
        Assert.That(await Page.Locator(".shopping_cart_badge").TextContentAsync(), Is.EqualTo("1"));
    }

    [Test]
    [Description("preserves cart contents after returning from cart")]
    public async Task PreservesCartContentsAfterReturningFromCart()
    {
        await _inventoryPage.AddToCartByDataTestAsync(Products.Backpack.DataTest);
        await _inventoryPage.GoToCartAsync();
        await _cartPage.ContinueShoppingAsync();
        await _inventoryPage.AssertCartBadgeCountAsync(1);
    }
}
