package com.automation.tests

import com.automation.base.BaseTest
import com.automation.pages.LoginPage
import com.automation.pages.InventoryPage
import com.automation.pages.CartPage
import com.automation.pages.CheckoutPage
import com.automation.data.TestData
import com.automation.data.ProductData
import org.junit.jupiter.api.DisplayName
import org.junit.jupiter.api.Test

import static org.junit.jupiter.api.Assertions.*

@DisplayName("Checkout Tests - BDD Style")
class CheckoutTests extends BaseTest {

    /** Login as standard user, add the given items, and reach checkout step 1 */
    private CheckoutPage loginAndAddItems(List<String> productDataTestIds) {
        LoginPage loginPage = new LoginPage(page)
        loginPage.goTo()
        loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)

        InventoryPage inventoryPage = new InventoryPage(page)
        inventoryPage.addMultipleToCart(productDataTestIds)
        inventoryPage.goToCart()

        CartPage cartPage = new CartPage(page)
        cartPage.proceedToCheckout()

        return new CheckoutPage(page)
    }

    @Test
    @DisplayName("Given I am at checkout step 1 When I submit without a first name Then I should see a first name required error")
    void "shows first name required error"() {
        // Given
        CheckoutPage checkoutPage = loginAndAddItems([ProductData.Products.BACKPACK.getDataTest()])
        // When
        checkoutPage.fillCustomerInfo("", TestData.CustomerData.LAST_NAME, TestData.CustomerData.POSTAL_CODE)
        checkoutPage.submitEmptyForm()
        // Then
        checkoutPage.assertStep1Error(TestData.ErrorMessages.MISSING_FIRST_NAME)
    }

    @Test
    @DisplayName("Given I checkout one item When I reach the overview Then the subtotal should match that item's price")
    void "shows correct subtotal for one item"() {
        // Given
        CheckoutPage checkoutPage = loginAndAddItems([ProductData.Products.BACKPACK.getDataTest()])
        // When
        checkoutPage.fillAndContinue(TestData.CustomerData.FIRST_NAME, TestData.CustomerData.LAST_NAME,
            TestData.CustomerData.POSTAL_CODE)
        // Then
        CheckoutPage.PriceSummary summary = checkoutPage.getPriceSummary()
        assertEquals(ProductData.Products.BACKPACK.getPrice(), summary.subtotal, 0.01,
            "Subtotal should match the single item price")
    }

    @Test
    @DisplayName("Given I checkout two items When I reach the overview Then subtotal plus tax should equal the total")
    void "verifies subtotal plus tax equals total"() {
        // Given
        CheckoutPage checkoutPage = loginAndAddItems([
            ProductData.Products.FLEECE_JACKET.getDataTest(),
            ProductData.Products.BACKPACK.getDataTest()
        ])
        // When
        checkoutPage.fillAndContinue(TestData.CustomerData.FIRST_NAME, TestData.CustomerData.LAST_NAME,
            TestData.CustomerData.POSTAL_CODE)
        // Then
        checkoutPage.assertTotalCalculationIsCorrect()
    }

    @Test
    @DisplayName("Given I sort and add two items When I complete checkout Then I should see the order confirmation")
    void "completes checkout with two items and verifies confirmation"() {
        // Given
        LoginPage loginPage = new LoginPage(page)
        loginPage.goTo()
        loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)

        InventoryPage inventoryPage = new InventoryPage(page)
        inventoryPage.sortBy(ProductData.SortOptions.PRICE_HI_LO)
        inventoryPage.assertPricesSortedDescending()

        inventoryPage.addMultipleToCart([
            ProductData.Products.FLEECE_JACKET.getDataTest(),
            ProductData.Products.BACKPACK.getDataTest()
        ])
        inventoryPage.assertCartBadgeCount(2)

        inventoryPage.goToCart()
        CartPage cartPage = new CartPage(page)
        cartPage.assertItemInCart(ProductData.Products.FLEECE_JACKET.getName())
        cartPage.assertItemInCart(ProductData.Products.BACKPACK.getName())
        cartPage.assertItemCount(2)

        // When
        cartPage.proceedToCheckout()
        CheckoutPage checkoutPage = new CheckoutPage(page)
        checkoutPage.fillAndContinue(TestData.CustomerData.FIRST_NAME, TestData.CustomerData.LAST_NAME,
            TestData.CustomerData.POSTAL_CODE)
        checkoutPage.assertTotalCalculationIsCorrect()
        double expected = ProductData.Products.FLEECE_JACKET.getPrice() + ProductData.Products.BACKPACK.getPrice()
        assertEquals(expected, checkoutPage.getPriceSummary().subtotal, 0.01,
            "Subtotal should equal the sum of both item prices")

        checkoutPage.finishOrder()
        // Then
        checkoutPage.assertConfirmationPage()
    }
}
