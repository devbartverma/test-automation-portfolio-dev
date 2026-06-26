package com.automation.pages;

import com.microsoft.playwright.Page;
import com.microsoft.playwright.Locator;
import com.automation.data.TestData;

import static org.junit.jupiter.api.Assertions.*;

public class CheckoutPage {
    private final Page page;

    public CheckoutPage(Page page) {
        this.page = page;
    }

    /** Small holder for the parsed price summary (subtotal, tax, total) */
    public static class PriceSummary {
        public final double subtotal;
        public final double tax;
        public final double total;

        public PriceSummary(double subtotal, double tax, double total) {
            this.subtotal = subtotal;
            this.tax = tax;
            this.total = total;
        }
    }

    private Locator getFirstNameInput() {
        return page.locator("[data-test='firstName']");
    }

    private Locator getLastNameInput() {
        return page.locator("[data-test='lastName']");
    }

    private Locator getPostalCodeInput() {
        return page.locator("[data-test='postalCode']");
    }

    private Locator getContinueButton() {
        return page.locator("[data-test='continue']");
    }

    private Locator getFinishButton() {
        return page.locator("[data-test='finish']");
    }

    private Locator getErrorMessage() {
        return page.locator("[data-test='error']");
    }

    private Locator getSubtotalLabel() {
        return page.locator(".summary_subtotal_label");
    }

    private Locator getTaxLabel() {
        return page.locator(".summary_tax_label");
    }

    private Locator getTotalLabel() {
        return page.locator(".summary_total_label");
    }

    public Locator getCompleteHeader() {
        return page.locator("[data-test='complete-header']");
    }

    // ─── Step 1 actions ─────────────────────────────────────────────────────────

    public void fillCustomerInfo(String firstName, String lastName, String postalCode) {
        getFirstNameInput().fill(firstName);
        getLastNameInput().fill(lastName);
        getPostalCodeInput().fill(postalCode);
    }

    public void continueCheckout() {
        getContinueButton().click();
    }

    public void continueToOverview() {
        getContinueButton().click();
        assertEquals(TestData.Urls.CHECKOUT_STEP_2, page.url(),
            "User should be redirected to checkout step 2");
    }

    public void fillAndContinue(String firstName, String lastName, String postalCode) {
        fillCustomerInfo(firstName, lastName, postalCode);
        continueToOverview();
    }

    /** Submit empty form to trigger validation */
    public void submitEmptyForm() {
        getContinueButton().click();
    }

    // ─── Step 2 actions ─────────────────────────────────────────────────────────

    public void finishOrder() {
        getFinishButton().click();
        assertEquals(TestData.Urls.CHECKOUT_COMPLETE, page.url(),
            "User should be redirected to checkout complete page");
    }

    /** Returns the parsed { subtotal, tax, total } for math verification */
    public PriceSummary getPriceSummary() {
        String subtotalText = getSubtotalLabel().textContent();
        String taxText = getTaxLabel().textContent();
        String totalText = getTotalLabel().textContent();

        double subtotal = Double.parseDouble(subtotalText.replace("Item total: $", "").trim());
        double tax = Double.parseDouble(taxText.replace("Tax: $", "").trim());
        double total = Double.parseDouble(totalText.replace("Total: $", "").trim());

        return new PriceSummary(subtotal, tax, total);
    }

    // ─── Assertions ──────────────────────────────────────────────────────────────

    public void assertStep1Error(String expectedText) {
        assertTrue(getErrorMessage().isVisible(), "Error message should be visible");
        assertTrue(getErrorMessage().textContent().contains(expectedText),
            "Error message should contain: " + expectedText);
    }

    /** Core financial assertion: subtotal + tax must equal total within 0.01 */
    public void assertTotalCalculationIsCorrect() {
        PriceSummary summary = getPriceSummary();
        assertEquals(summary.total, summary.subtotal + summary.tax, 0.01,
            "Expected subtotal (" + summary.subtotal + ") + tax (" + summary.tax
                + ") to equal total (" + summary.total + ")");
    }

    public void assertConfirmationPage() {
        assertEquals(TestData.Urls.CHECKOUT_COMPLETE, page.url(),
            "User should be on the checkout complete page");
        assertTrue(getCompleteHeader().isVisible(), "Completion header should be visible");
        assertEquals("Thank you for your order!", getCompleteHeader().textContent(),
            "Completion message should be displayed");
    }
}
