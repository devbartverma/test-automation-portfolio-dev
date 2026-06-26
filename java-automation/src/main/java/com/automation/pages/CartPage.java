package com.automation.pages;

import com.microsoft.playwright.Page;
import com.microsoft.playwright.Locator;
import com.automation.data.TestData;

import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class CartPage {
    private final Page page;

    public CartPage(Page page) {
        this.page = page;
    }

    private Locator getItemNames() {
        return page.locator(".inventory_item_name");
    }

    private Locator getItemPrices() {
        return page.locator(".inventory_item_price");
    }

    private Locator getCheckoutButton() {
        return page.locator("[data-test='checkout']");
    }

    private Locator getContinueShoppingButton() {
        return page.locator("[data-test='continue-shopping']");
    }

    public void goTo() {
        page.navigate(TestData.Urls.CART);
        assertEquals(TestData.Urls.CART, page.url(), "User should be on cart page");
    }

    public void proceedToCheckout() {
        getCheckoutButton().click();
        assertEquals(TestData.Urls.CHECKOUT_STEP_1, page.url(),
            "User should be redirected to checkout step 1");
    }

    public void continueShopping() {
        getContinueShoppingButton().click();
        assertEquals(TestData.Urls.INVENTORY, page.url(),
            "User should be redirected to inventory page");
    }

    /** Build the remove- data-test id from the product name and click it */
    public void removeItem(String productName) {
        String id = productName
            .toLowerCase()
            .replaceAll("[()]", "")
            .replaceAll("\\s+", "-");
        page.locator("[data-test='remove-" + id + "']").click();
    }

    // ─── Assertions ──────────────────────────────────────────────────────────────

    public void assertItemInCart(String productName) {
        Locator item = page.getByText(productName, new Page.GetByTextOptions().setExact(true));
        assertTrue(item.isVisible(), "Product '" + productName + "' should be in cart");
    }

    public void assertItemNotInCart(String productName) {
        Locator item = page.getByText(productName, new Page.GetByTextOptions().setExact(true));
        assertFalse(item.isVisible(), "Product '" + productName + "' should not be in cart");
    }

    public void assertItemCount(int expected) {
        assertEquals(expected, getItemNames().count(),
            "Cart should contain " + expected + " items");
    }

    public void assertCartIsEmpty() {
        assertEquals(0, getItemNames().count(), "Cart should be empty");
    }

    /** Retrieve all cart prices as numbers for downstream math assertions */
    public List<Double> getCartPrices() {
        List<Double> prices = new ArrayList<>();
        for (String raw : getItemPrices().allTextContents()) {
            prices.add(Double.parseDouble(raw.replace("$", "").trim()));
        }
        return prices;
    }
}
