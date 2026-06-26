import re
from typing import List
from playwright.sync_api import Page
from src.automation.data.test_data import Urls


class CartPage:
    def __init__(self, page: Page):
        self.page = page

    @property
    def item_names(self):
        return self.page.locator(".inventory_item_name")

    @property
    def item_prices(self):
        return self.page.locator(".inventory_item_price")

    @property
    def checkout_button(self):
        return self.page.locator("[data-test='checkout']")

    @property
    def continue_shopping_button(self):
        return self.page.locator("[data-test='continue-shopping']")

    def go_to(self):
        self.page.goto(Urls.CART)
        assert self.page.url == Urls.CART, \
            f"User should be on {Urls.CART}, but got {self.page.url}"

    def proceed_to_checkout(self):
        self.checkout_button.click()
        assert self.page.url == Urls.CHECKOUT_STEP_1, \
            f"User should be redirected to {Urls.CHECKOUT_STEP_1}, but got {self.page.url}"

    def continue_shopping(self):
        self.continue_shopping_button.click()
        assert self.page.url == Urls.INVENTORY, \
            f"User should be redirected to {Urls.INVENTORY}, but got {self.page.url}"

    def remove_item(self, product_name: str):
        # Build the remove data-test id from the product name dynamically,
        # mirroring the JS CartPage.removeItem logic.
        item_id = product_name.lower()
        item_id = re.sub(r"[()]", "", item_id)
        item_id = re.sub(r"\s+", "-", item_id)
        self.page.locator(f"[data-test='remove-{item_id}']").click()

    # ─── Accessors ───────────────────────────────────────────────────────────

    def get_cart_prices(self) -> List[float]:
        raw = self.item_prices.all_text_contents()
        return [float(price.replace("$", "")) for price in raw]

    # ─── Assertions ──────────────────────────────────────────────────────────

    def assert_item_in_cart(self, product_name: str):
        item = self.page.get_by_text(product_name, exact=True)
        assert item.is_visible(), f"Product '{product_name}' should be in cart"

    def assert_item_not_in_cart(self, product_name: str):
        item = self.page.get_by_text(product_name, exact=True)
        assert not item.is_visible(), \
            f"Product '{product_name}' should not be in cart"

    def assert_item_count(self, expected: int):
        count = self.item_names.count()
        assert count == expected, \
            f"Expected {expected} items in cart but found {count}"

    def assert_cart_is_empty(self):
        count = self.item_names.count()
        assert count == 0, f"Cart should be empty but found {count} items"
