package com.automation.tests

import com.automation.base.BaseTest
import com.automation.pages.LoginPage
import com.automation.pages.InventoryPage
import com.automation.pages.CartPage
import com.automation.data.TestData
import com.automation.data.ProductData
import org.junit.jupiter.api.DisplayName
import org.junit.jupiter.api.Test

import static org.junit.jupiter.api.Assertions.*

@DisplayName("Shopping Cart Tests - BDD Style")
class CartTests extends BaseTest {
    private LoginPage loginPage
    private InventoryPage inventoryPage
    private CartPage cartPage

    @Override
    void setUp() {
        super.setUp()
        loginPage = new LoginPage(page)
        loginPage.goTo()
        loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)
        inventoryPage = new InventoryPage(page)
        cartPage = new CartPage(page)
    }

    @Test
    @DisplayName("Given I added items When I open the cart Then the items should be listed with correct names")
    void "shows added items in cart with correct names"() {
        // Given
        inventoryPage.addMultipleToCart([
            ProductData.Products.FLEECE_JACKET.getDataTest(),
            ProductData.Products.BACKPACK.getDataTest()
        ])
        // When
        inventoryPage.goToCart()
        // Then
        cartPage.assertItemInCart(ProductData.Products.FLEECE_JACKET.getName())
        cartPage.assertItemInCart(ProductData.Products.BACKPACK.getName())
        cartPage.assertItemCount(2)
    }

    @Test
    @DisplayName("Given I have two items in the cart When I remove one Then the count should drop to 1")
    void "removes item from cart and updates count"() {
        // Given
        inventoryPage.addMultipleToCart([
            ProductData.Products.BACKPACK.getDataTest(),
            ProductData.Products.BIKE_LIGHT.getDataTest()
        ])
        inventoryPage.goToCart()
        // When
        cartPage.removeItem(ProductData.Products.BACKPACK.getName())
        // Then
        cartPage.assertItemNotInCart(ProductData.Products.BACKPACK.getName())
        cartPage.assertItemCount(1)
        assertEquals("1", page.locator(".shopping_cart_badge").textContent(),
            "Cart badge should show 1")
    }

    @Test
    @DisplayName("Given I added an item When I continue shopping from the cart Then the cart contents should be preserved")
    void "preserves cart contents after returning from cart"() {
        // Given
        inventoryPage.addToCartByDataTest(ProductData.Products.BACKPACK.getDataTest())
        inventoryPage.goToCart()
        // When
        cartPage.continueShopping()
        // Then
        inventoryPage.assertCartBadgeCount(1)
    }
}
