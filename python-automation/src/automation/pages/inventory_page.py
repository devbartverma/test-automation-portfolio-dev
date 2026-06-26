from typing import List
from playwright.sync_api import Page
from src.automation.data.test_data import Urls


class InventoryPage:
    def __init__(self, page: Page):
        self.page = page

    @property
    def product_items(self):
        return self.page.locator(".inventory_item")

    @property
    def product_names(self):
        return self.page.locator(".inventory_item_name")

    @property
    def product_prices(self):
        return self.page.locator(".inventory_item_price")

    @property
    def cart_badge(self):
        return self.page.locator(".shopping_cart_badge")

    @property
    def cart_link(self):
        return self.page.locator(".shopping_cart_link")

    @property
    def sort_dropdown(self):
        return self.page.locator("[data-test='product-sort-container']")

    @property
    def burger_menu_button(self):
        return self.page.get_by_role("button", name="Open Menu")

    @property
    def reset_app_state_link(self):
        return self.page.get_by_role("link", name="Reset App State")

    @property
    def logout_link(self):
        return self.page.get_by_role("link", name="Logout")

    def go_to(self):
        self.page.goto(Urls.INVENTORY)

    def sort_by(self, option: str):
        self.sort_dropdown.select_option(option)

    # Backwards-compatible alias for the original method name
    def select_sort(self, option: str):
        self.sort_by(option)

    def add_to_cart_by_data_test(self, data_test_id: str):
        self.page.locator(f"[data-test='{data_test_id}']").click()

    def add_multiple_to_cart(self, data_test_ids: List[str]):
        for data_test_id in data_test_ids:
            self.add_to_cart_by_data_test(data_test_id)

    def remove_from_cart_by_data_test(self, data_test_id: str):
        self.page.locator(f"[data-test='{data_test_id}']").click()

    def open_product_detail(self, product_name: str):
        self.page.get_by_text(product_name, exact=True).click()

    def go_to_cart(self):
        self.cart_link.click()
        assert self.page.url == Urls.CART, \
            f"User should be redirected to {Urls.CART}, but got {self.page.url}"

    def logout(self):
        self.burger_menu_button.click()
        self.logout_link.click()
        assert self.page.url == Urls.BASE, \
            f"User should be redirected to {Urls.BASE}, but got {self.page.url}"

    def reset_app_state(self):
        self.burger_menu_button.click()
        self.reset_app_state_link.click()

    # ─── Accessors ───────────────────────────────────────────────────────────

    def get_product_names(self) -> List[str]:
        return self.product_names.all_text_contents()

    def get_product_prices(self) -> List[float]:
        raw = self.product_prices.all_text_contents()
        return [float(price.replace("$", "")) for price in raw]

    # ─── Assertions ──────────────────────────────────────────────────────────

    def assert_product_count(self, expected: int):
        count = self.product_items.count()
        assert count == expected, \
            f"Expected {expected} products but found {count}"

    def assert_cart_badge_count(self, expected: int):
        assert self.cart_badge.is_visible(), "Cart badge should be visible"
        badge_text = self.cart_badge.text_content()
        assert badge_text == str(expected), \
            f"Cart badge should show {expected} items but shows {badge_text}"

    def assert_cart_badge_not_visible(self):
        assert not self.cart_badge.is_visible(), \
            "Cart badge should not be visible"

    def assert_prices_sorted_descending(self) -> List[float]:
        prices = self.get_product_prices()
        is_sorted = all(prices[i - 1] >= prices[i] for i in range(1, len(prices)))
        assert is_sorted, f"Prices not sorted descending: {prices}"
        return prices

    def assert_prices_sorted_ascending(self) -> List[float]:
        prices = self.get_product_prices()
        is_sorted = all(prices[i - 1] <= prices[i] for i in range(1, len(prices)))
        assert is_sorted, f"Prices not sorted ascending: {prices}"
        return prices

    def assert_names_alphabetically_ascending(self) -> List[str]:
        names = self.get_product_names()
        assert names == sorted(names), \
            f"Names not sorted ascending: {names}"
        return names

    def assert_names_alphabetically_descending(self) -> List[str]:
        names = self.get_product_names()
        assert names == sorted(names, reverse=True), \
            f"Names not sorted descending: {names}"
        return names
