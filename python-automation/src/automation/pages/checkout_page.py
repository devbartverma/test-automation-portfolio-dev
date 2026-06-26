from typing import Dict
from playwright.sync_api import Page
from src.automation.data.test_data import Urls


class CheckoutPage:
    def __init__(self, page: Page):
        self.page = page

    @property
    def first_name_input(self):
        return self.page.locator("[data-test='firstName']")

    @property
    def last_name_input(self):
        return self.page.locator("[data-test='lastName']")

    @property
    def postal_code_input(self):
        return self.page.locator("[data-test='postalCode']")

    @property
    def continue_button(self):
        return self.page.locator("[data-test='continue']")

    @property
    def finish_button(self):
        return self.page.locator("[data-test='finish']")

    @property
    def error_message(self):
        return self.page.locator("[data-test='error']")

    @property
    def subtotal_label(self):
        return self.page.locator(".summary_subtotal_label")

    @property
    def tax_label(self):
        return self.page.locator(".summary_tax_label")

    @property
    def total_label(self):
        return self.page.locator(".summary_total_label")

    @property
    def complete_header(self):
        return self.page.locator("[data-test='complete-header']")

    # ─── Step 1 actions ────────────────────────────────────────────────────────

    def fill_customer_info(self, first_name: str, last_name: str, postal_code: str):
        self.first_name_input.fill(first_name)
        self.last_name_input.fill(last_name)
        self.postal_code_input.fill(postal_code)

    def continue_checkout(self):
        self.continue_button.click()

    def continue_to_overview(self):
        self.continue_button.click()
        assert self.page.url == Urls.CHECKOUT_STEP_2, \
            f"User should be redirected to {Urls.CHECKOUT_STEP_2}, but got {self.page.url}"

    def fill_and_continue(self, first_name: str, last_name: str, postal_code: str):
        self.fill_customer_info(first_name, last_name, postal_code)
        self.continue_to_overview()

    def submit_empty_form(self):
        self.continue_button.click()

    # ─── Step 2 actions ────────────────────────────────────────────────────────

    def finish_order(self):
        self.finish_button.click()
        assert self.page.url == Urls.CHECKOUT_COMPLETE, \
            f"User should be redirected to {Urls.CHECKOUT_COMPLETE}, but got {self.page.url}"

    def get_price_summary(self) -> Dict[str, float]:
        """Returns {subtotal, tax, total} parsed as floats for math verification."""
        subtotal_text = self.subtotal_label.text_content() or ""
        tax_text = self.tax_label.text_content() or ""
        total_text = self.total_label.text_content() or ""

        return {
            "subtotal": float(subtotal_text.replace("Item total: $", "").strip()),
            "tax": float(tax_text.replace("Tax: $", "").strip()),
            "total": float(total_text.replace("Total: $", "").strip()),
        }

    # ─── Assertions ──────────────────────────────────────────────────────────

    def assert_step1_error(self, expected_text: str):
        assert self.error_message.is_visible(), "Error message should be visible"
        error_text = self.error_message.text_content()
        assert expected_text in error_text, \
            f"Error message should contain: {expected_text}, but got {error_text}"

    def assert_total_calculation_is_correct(self):
        """Core financial assertion: subtotal + tax must equal total."""
        summary = self.get_price_summary()
        expected = round(summary["subtotal"] + summary["tax"], 2)
        actual = round(summary["total"], 2)
        assert expected == actual, \
            f"Expected subtotal ({summary['subtotal']}) + tax ({summary['tax']}) = " \
            f"{expected}, got {actual}"

    def assert_confirmation_page(self):
        assert self.page.url == Urls.CHECKOUT_COMPLETE, \
            f"User should be on {Urls.CHECKOUT_COMPLETE}, but got {self.page.url}"
        assert self.complete_header.is_visible(), "Complete header should be visible"
        header_text = self.complete_header.text_content()
        assert header_text == "Thank you for your order!", \
            f"Complete header should be 'Thank you for your order!' but got '{header_text}'"
