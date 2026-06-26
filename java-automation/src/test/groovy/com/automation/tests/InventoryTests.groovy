package com.automation.tests

import com.automation.base.BaseTest
import com.automation.pages.LoginPage
import com.automation.pages.InventoryPage
import com.automation.pages.ProductDetailPage
import com.automation.data.TestData
import com.automation.data.ProductData
import org.junit.jupiter.api.DisplayName
import org.junit.jupiter.api.Test

import static org.junit.jupiter.api.Assertions.*

@DisplayName("Inventory Tests - BDD Style")
class InventoryTests extends BaseTest {
    private LoginPage loginPage
    private InventoryPage inventoryPage

    @Override
    void setUp() {
        super.setUp()
        loginPage = new LoginPage(page)
        loginPage.goTo()
        loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)
        inventoryPage = new InventoryPage(page)
    }

    @Test
    @DisplayName("Given I am logged in When I view the inventory Then I should see six products")
    void "shows six inventory products"() {
        // Given & When - inventory is loaded
        // Then
        inventoryPage.assertProductCount(6)
    }

    @Test
    @DisplayName("Given I am on the inventory page When I sort by price high to low Then products should be ordered descending")
    void "sorts products by price high to low"() {
        // When
        inventoryPage.sortBy(ProductData.SortOptions.PRICE_HI_LO)
        // Then
        List<Double> prices = inventoryPage.assertPricesSortedDescending()
        assertTrue(prices.get(0) > prices.get(prices.size() - 1),
            "First price should be greater than last price")
    }

    @Test
    @DisplayName("Given I am on the inventory page When I sort by name A to Z Then products should be ordered alphabetically ascending")
    void "sorts products by name A to Z"() {
        // When
        inventoryPage.sortBy(ProductData.SortOptions.NAME_AZ)
        // Then
        inventoryPage.assertNamesAlphabeticallyAscending()
    }

    @Test
    @DisplayName("Given I am on the inventory page When I open the Sauce Labs Backpack detail Then I should see its name, price, description and a back button")
    void "opens product detail page for Sauce Labs Backpack"() {
        // When
        inventoryPage.openProductDetail(ProductData.Products.BACKPACK.getName())

        // Then
        ProductDetailPage detailPage = new ProductDetailPage(page)
        detailPage.assertProductDetail(
            ProductData.Products.BACKPACK.getName(),
            "\$" + ProductData.Products.BACKPACK.getPrice())
        assertTrue(page.locator(".inventory_details_desc").textContent().trim().length() > 0,
            "Product description should not be empty")
        assertTrue(page.locator("[data-test='back-to-products']").isVisible(),
            "Back to products button should be visible")
    }
}
