import pytest
from src.automation.pages.login_page import LoginPage
from src.automation.pages.inventory_page import InventoryPage
from src.automation.data.test_data import Users
from src.automation.data.product_data import Products, SortOptions


class TestInventory:
    """Inventory test suite for product listing and sorting functionality."""

    @pytest.fixture(autouse=True)
    def setup(self, page):
        """Setup: Login before each inventory test."""
        login_page = LoginPage(page)
        login_page.go_to()
        login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)
        self.inventory_page = InventoryPage(page)
        self.page = page

    def test_shows_six_products(self, page):
        """
        Given: I am logged in
        When: the inventory page is loaded
        Then: I should see six products
        """
        # Then
        self.inventory_page.assert_product_count(6)

    def test_sorts_products_by_price_high_to_low(self, page):
        """
        Given: I am on the inventory page
        When: I sort by price high to low
        Then: prices should be in descending order
        """
        # When
        self.inventory_page.sort_by(SortOptions.PRICE_HI_LO)

        # Then
        prices = self.inventory_page.assert_prices_sorted_descending()
        assert prices[0] > prices[-1]

    def test_sorts_products_by_name_a_to_z(self, page):
        """
        Given: I am on the inventory page
        When: I sort by name A to Z
        Then: names should be alphabetically ascending
        """
        # When
        self.inventory_page.sort_by(SortOptions.NAME_AZ)

        # Then
        self.inventory_page.assert_names_alphabetically_ascending()

    def test_opens_product_detail_for_backpack(self, page):
        """
        Given: I am on the inventory page
        When: I open the Sauce Labs Backpack detail page
        Then: the name, price, description, and back button should be shown
        """
        # When
        self.inventory_page.open_product_detail(Products.BACKPACK.name)

        # Then
        assert page.locator(".inventory_details_name").text_content() == Products.BACKPACK.name
        assert page.locator(".inventory_details_price").text_content() == f"${Products.BACKPACK.price}"
        assert page.locator(".inventory_details_desc").text_content().strip() != ""
        assert page.locator("[data-test='back-to-products']").is_visible()
