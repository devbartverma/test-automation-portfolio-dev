package com.automation.pages;

import com.microsoft.playwright.Page;
import com.microsoft.playwright.Locator;
import com.microsoft.playwright.options.AriaRole;
import com.automation.data.TestData;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class InventoryPage {
    private final Page page;

    public InventoryPage(Page page) {
        this.page = page;
    }

    private Locator getProductItems() {
        return page.locator(".inventory_item");
    }

    private Locator getProductNames() {
        return page.locator(".inventory_item_name");
    }

    private Locator getProductPrices() {
        return page.locator(".inventory_item_price");
    }

    private Locator getCartBadge() {
        return page.locator(".shopping_cart_badge");
    }

    private Locator getCartLink() {
        return page.locator(".shopping_cart_link");
    }

    private Locator getSortDropdown() {
        return page.locator("[data-test='product-sort-container']");
    }

    private Locator getBurgerMenuButton() {
        return page.getByRole(AriaRole.BUTTON, new Page.GetByRoleOptions().setName("Open Menu"));
    }

    private Locator getResetAppStateLink() {
        return page.getByRole(AriaRole.LINK, new Page.GetByRoleOptions().setName("Reset App State"));
    }

    private Locator getLogoutLink() {
        return page.getByRole(AriaRole.LINK, new Page.GetByRoleOptions().setName("Logout"));
    }

    public void goTo() {
        page.navigate(TestData.Urls.INVENTORY);
    }

    public void sortBy(String option) {
        getSortDropdown().selectOption(option);
    }

    /** Backwards-compatible alias kept for existing callers */
    public void selectSort(String option) {
        sortBy(option);
    }

    public void addToCartByDataTest(String dataTestId) {
        page.locator("[data-test='" + dataTestId + "']").click();
    }

    public void addMultipleToCart(List<String> dataTestIds) {
        for (String id : dataTestIds) {
            addToCartByDataTest(id);
        }
    }

    public void removeFromCartByDataTest(String dataTestId) {
        page.locator("[data-test='" + dataTestId + "']").click();
    }

    public void openProductDetail(String productName) {
        page.getByText(productName, new Page.GetByTextOptions().setExact(true)).click();
    }

    public void goToCart() {
        getCartLink().click();
        assertEquals(TestData.Urls.CART, page.url(), "User should be redirected to cart page");
    }

    public void logout() {
        getBurgerMenuButton().click();
        getLogoutLink().click();
        assertEquals(TestData.Urls.BASE, page.url(), "User should be redirected to login page after logout");
    }

    public void resetAppState() {
        getBurgerMenuButton().click();
        getResetAppStateLink().click();
    }

    // ─── Assertions ──────────────────────────────────────────────────────────────

    public void assertProductCount(int expected) {
        assertEquals(expected, getProductItems().count(),
            "Expected " + expected + " products but found " + getProductItems().count());
    }

    public void assertCartBadgeCount(int expected) {
        assertTrue(getCartBadge().isVisible(), "Cart badge should be visible");
        assertEquals(String.valueOf(expected), getCartBadge().textContent(),
            "Cart badge should show " + expected + " items");
    }

    public void assertCartBadgeNotVisible() {
        assertFalse(getCartBadge().isVisible(), "Cart badge should not be visible");
    }

    private List<Double> parsePrices() {
        List<Double> prices = new ArrayList<>();
        for (String raw : getProductPrices().allTextContents()) {
            prices.add(Double.parseDouble(raw.replace("$", "").trim()));
        }
        return prices;
    }

    public List<Double> assertPricesSortedDescending() {
        List<Double> prices = parsePrices();
        for (int i = 1; i < prices.size(); i++) {
            assertTrue(prices.get(i - 1) >= prices.get(i),
                "Prices not sorted descending: " + prices);
        }
        return prices;
    }

    public List<Double> assertPricesSortedAscending() {
        List<Double> prices = parsePrices();
        for (int i = 1; i < prices.size(); i++) {
            assertTrue(prices.get(i - 1) <= prices.get(i),
                "Prices not sorted ascending: " + prices);
        }
        return prices;
    }

    public List<String> assertNamesAlphabeticallyAscending() {
        List<String> names = new ArrayList<>(getProductNames().allTextContents());
        List<String> sorted = new ArrayList<>(names);
        Collections.sort(sorted);
        assertEquals(sorted, names, "Names should be sorted A to Z");
        return names;
    }

    public List<String> assertNamesAlphabeticallyDescending() {
        List<String> names = new ArrayList<>(getProductNames().allTextContents());
        List<String> sorted = new ArrayList<>(names);
        sorted.sort(Collections.reverseOrder());
        assertEquals(sorted, names, "Names should be sorted Z to A");
        return names;
    }
}
