using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class CheckoutTests : PageTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;
    private CartPage _cartPage = null!;
    private CheckoutPage _checkoutPage = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);

        _inventoryPage = new InventoryPage(Page);
    }

    [Test]
    public async Task CheckoutWithOneItemCompletesSuccessfully()
    {
        await _inventoryPage.AddToCartByDataTestAsync(Products.BackpackAddToCart);
        await _inventoryPage.GoToCartAsync();

        _cartPage = new CartPage(Page);
        await _cartPage.ProceedToCheckoutAsync();

        _checkoutPage = new CheckoutPage(Page);
        await _checkoutPage.FillCustomerInfoAsync(CustomerData.FirstName, CustomerData.LastName, CustomerData.PostalCode);
        await _checkoutPage.ContinueAsync();
        Assert.That(Page.Url, Is.EqualTo(Urls.CheckoutStep2));
        await _checkoutPage.FinishOrderAsync();

        Assert.That(await _checkoutPage.CompleteHeader.TextContentAsync(), Is.EqualTo("Thank you for your order!"));
    }

    [Test]
    public async Task CheckoutValidationShowsFirstNameError()
    {
        await _inventoryPage.AddToCartByDataTestAsync(Products.BackpackAddToCart);
        await _inventoryPage.GoToCartAsync();

        _cartPage = new CartPage(Page);
        await _cartPage.ProceedToCheckoutAsync();

        _checkoutPage = new CheckoutPage(Page);
        await _checkoutPage.ContinueAsync();
        await _checkoutPage.AssertStep1ErrorAsync("Error: First Name is required");
    }
}
