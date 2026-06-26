using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class CheckoutTests : BasePageTest
{
    /// <summary>
    /// Logs in as the standard user, adds the supplied products to the cart,
    /// navigates to the cart and proceeds to checkout step one.
    /// </summary>
    private async Task<(InventoryPage Inventory, CartPage Cart, CheckoutPage Checkout)> LoginAndAddItemsAsync(
        IEnumerable<string> productDataTestIds)
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);

        var inventoryPage = new InventoryPage(Page);
        await inventoryPage.AddMultipleToCartAsync(productDataTestIds);
        await inventoryPage.GoToCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.ProceedToCheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);
        return (inventoryPage, cartPage, checkoutPage);
    }

    [Test]
    [Description("shows first name required error")]
    public async Task ShowsFirstNameRequiredError()
    {
        var (_, _, checkout) = await LoginAndAddItemsAsync(new[] { Products.Backpack.DataTest });
        await checkout.FillCustomerInfoAsync(string.Empty, CustomerData.LastName, CustomerData.PostalCode);
        await checkout.SubmitEmptyFormAsync();
        await checkout.AssertStep1ErrorAsync(ErrorMessages.MissingFirstName);
    }

    [Test]
    [Description("shows correct subtotal for one item")]
    public async Task ShowsCorrectSubtotalForOneItem()
    {
        var (_, _, checkout) = await LoginAndAddItemsAsync(new[] { Products.Backpack.DataTest });
        await checkout.FillAndContinueAsync(CustomerData.FirstName, CustomerData.LastName, CustomerData.PostalCode);

        var (subtotal, _, _) = await checkout.GetPriceSummaryAsync();
        Assert.That(subtotal, Is.EqualTo(Products.Backpack.Price).Within(0.01));
    }

    [Test]
    [Description("verifies subtotal plus tax equals total")]
    public async Task VerifiesSubtotalPlusTaxEqualsTotal()
    {
        var (_, _, checkout) = await LoginAndAddItemsAsync(new[]
        {
            Products.FleeceJacket.DataTest,
            Products.Backpack.DataTest,
        });
        await checkout.FillAndContinueAsync(CustomerData.FirstName, CustomerData.LastName, CustomerData.PostalCode);
        await checkout.AssertTotalCalculationIsCorrectAsync();
    }

    [Test]
    [Description("completes checkout with two items and verifies confirmation")]
    public async Task CompletesCheckoutWithTwoItemsAndVerifiesConfirmation()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);

        var inventoryPage = new InventoryPage(Page);
        await inventoryPage.SortByAsync(SortOptions.PriceHiLo);
        await inventoryPage.AssertPricesSortedDescendingAsync();

        await inventoryPage.AddMultipleToCartAsync(new[]
        {
            Products.FleeceJacket.DataTest,
            Products.Backpack.DataTest,
        });
        await inventoryPage.AssertCartBadgeCountAsync(2);

        await inventoryPage.GoToCartAsync();
        var cartPage = new CartPage(Page);
        await cartPage.AssertItemInCartAsync(Products.FleeceJacket.Name);
        await cartPage.AssertItemInCartAsync(Products.Backpack.Name);
        await cartPage.AssertItemCountAsync(2);

        await cartPage.ProceedToCheckoutAsync();
        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillAndContinueAsync(CustomerData.FirstName, CustomerData.LastName, CustomerData.PostalCode);
        await checkoutPage.AssertTotalCalculationIsCorrectAsync();

        var (subtotal, _, _) = await checkoutPage.GetPriceSummaryAsync();
        Assert.That(subtotal, Is.EqualTo(Products.FleeceJacket.Price + Products.Backpack.Price).Within(0.01));

        await checkoutPage.FinishOrderAsync();
        await checkoutPage.AssertConfirmationPageAsync();
    }
}
