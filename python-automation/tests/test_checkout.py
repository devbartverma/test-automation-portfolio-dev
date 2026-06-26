from typing import List
from src.automation.pages.login_page import LoginPage
from src.automation.pages.inventory_page import InventoryPage
from src.automation.pages.cart_page import CartPage
from src.automation.pages.checkout_page import CheckoutPage
from src.automation.data.test_data import Users, CustomerData, ErrorMessages
from src.automation.data.product_data import Products, SortOptions


def login_and_add_items(page, product_data_test_ids: List[str]):
    """
    Helper: login as the standard user, add the given products to the cart,
    navigate to the cart, and proceed to checkout step one.
    Mirrors the JS-TS loginAndAddItems helper.
    """
    login_page = LoginPage(page)
    login_page.go_to()
    login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)

    inventory_page = InventoryPage(page)
    inventory_page.add_multiple_to_cart(product_data_test_ids)
    inventory_page.go_to_cart()

    cart_page = CartPage(page)
    cart_page.proceed_to_checkout()

    checkout_page = CheckoutPage(page)
    return inventory_page, cart_page, checkout_page


class TestCheckout:
    """Checkout validation, pricing, and full end-to-end scenarios."""

    def test_shows_first_name_required_error(self, page):
        """
        Given: I am on the checkout step one page
        When: I submit without a first name
        Then: a first name required error should appear
        """
        # Given
        _, _, checkout_page = login_and_add_items(page, [Products.BACKPACK.data_test])

        # When
        checkout_page.fill_customer_info("", CustomerData.LAST_NAME, CustomerData.POSTAL_CODE)
        checkout_page.submit_empty_form()

        # Then
        checkout_page.assert_step1_error(ErrorMessages.MISSING_FIRST_NAME)

    def test_shows_correct_subtotal_for_one_item(self, page):
        """
        Given: I have one item in the cart
        When: I continue to the overview
        Then: the subtotal should match the item price
        """
        # Given
        _, _, checkout_page = login_and_add_items(page, [Products.BACKPACK.data_test])

        # When
        checkout_page.fill_and_continue(
            CustomerData.FIRST_NAME, CustomerData.LAST_NAME, CustomerData.POSTAL_CODE
        )

        # Then
        summary = checkout_page.get_price_summary()
        assert round(summary["subtotal"], 2) == round(Products.BACKPACK.price, 2)

    def test_subtotal_plus_tax_equals_total(self, page):
        """
        Given: I have two items in the cart
        When: I continue to the overview
        Then: subtotal plus tax should equal the total
        """
        # Given
        _, _, checkout_page = login_and_add_items(page, [
            Products.FLEECE_JACKET.data_test,
            Products.BACKPACK.data_test,
        ])

        # When
        checkout_page.fill_and_continue(
            CustomerData.FIRST_NAME, CustomerData.LAST_NAME, CustomerData.POSTAL_CODE
        )

        # Then
        checkout_page.assert_total_calculation_is_correct()

    def test_completes_checkout_with_two_items_and_confirmation(self, page):
        """
        Given: I am logged in
        When: I sort, add two items, and complete checkout
        Then: the order confirmation page should be shown
        """
        # Given
        login_page = LoginPage(page)
        login_page.go_to()
        login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)

        inventory_page = InventoryPage(page)
        inventory_page.sort_by(SortOptions.PRICE_HI_LO)
        inventory_page.assert_prices_sorted_descending()

        inventory_page.add_multiple_to_cart([
            Products.FLEECE_JACKET.data_test,
            Products.BACKPACK.data_test,
        ])
        inventory_page.assert_cart_badge_count(2)

        # When
        inventory_page.go_to_cart()
        cart_page = CartPage(page)
        cart_page.assert_item_in_cart(Products.FLEECE_JACKET.name)
        cart_page.assert_item_in_cart(Products.BACKPACK.name)
        cart_page.assert_item_count(2)

        cart_page.proceed_to_checkout()
        checkout_page = CheckoutPage(page)
        checkout_page.fill_and_continue(
            CustomerData.FIRST_NAME, CustomerData.LAST_NAME, CustomerData.POSTAL_CODE
        )
        checkout_page.assert_total_calculation_is_correct()
        expected = round(Products.FLEECE_JACKET.price + Products.BACKPACK.price, 2)
        assert round(checkout_page.get_price_summary()["subtotal"], 2) == expected

        checkout_page.finish_order()

        # Then
        checkout_page.assert_confirmation_page()
