import pytest
from src.automation.pages.login_page import LoginPage
from src.automation.pages.inventory_page import InventoryPage
from src.automation.pages.cart_page import CartPage
from src.automation.data.test_data import Users
from src.automation.data.product_data import Products


class TestCart:
    """Shopping cart test suite for add, remove, and persistence behaviour."""

    @pytest.fixture(autouse=True)
    def setup(self, page):
        """Setup: Login before each cart test."""
        login_page = LoginPage(page)
        login_page.go_to()
        login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)
        self.inventory_page = InventoryPage(page)
        self.cart_page = CartPage(page)
        self.page = page

    def test_shows_added_items_in_cart_with_names(self, page):
        """
        Given: I have added two items to the cart
        When: I navigate to the cart
        Then: both items should be shown with correct names
        """
        # Given
        self.inventory_page.add_multiple_to_cart([
            Products.FLEECE_JACKET.data_test,
            Products.BACKPACK.data_test,
        ])

        # When
        self.inventory_page.go_to_cart()

        # Then
        self.cart_page.assert_item_in_cart(Products.FLEECE_JACKET.name)
        self.cart_page.assert_item_in_cart(Products.BACKPACK.name)
        self.cart_page.assert_item_count(2)

    def test_removes_item_from_cart_and_updates_count(self, page):
        """
        Given: I have two items in the cart
        When: I remove one item
        Then: the count and badge should update to 1
        """
        # Given
        self.inventory_page.add_multiple_to_cart([
            Products.BACKPACK.data_test,
            Products.BIKE_LIGHT.data_test,
        ])
        self.inventory_page.go_to_cart()

        # When
        self.cart_page.remove_item(Products.BACKPACK.name)

        # Then
        self.cart_page.assert_item_not_in_cart(Products.BACKPACK.name)
        self.cart_page.assert_item_count(1)
        assert page.locator(".shopping_cart_badge").text_content() == "1"

    def test_preserves_cart_contents_after_continue_shopping(self, page):
        """
        Given: I have one item in the cart
        When: I continue shopping from the cart
        Then: the cart badge should still show 1
        """
        # Given
        self.inventory_page.add_to_cart_by_data_test(Products.BACKPACK.data_test)
        self.inventory_page.go_to_cart()

        # When
        self.cart_page.continue_shopping()

        # Then
        self.inventory_page.assert_cart_badge_count(1)
