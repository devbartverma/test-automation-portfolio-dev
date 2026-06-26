import pytest
from src.automation.pages.login_page import LoginPage
from src.automation.data.test_data import Users, Urls, ErrorMessages


class TestAuthentication:
    """Authentication test suite for login functionality."""

    @pytest.fixture(autouse=True)
    def setup(self, page):
        """Setup: Navigate to the login page before each test."""
        self.login_page = LoginPage(page)
        self.login_page.go_to()

    def test_standard_user_can_login(self, page):
        """
        Given: I am on the login page
        When: I login with valid standard credentials
        Then: I should land on the inventory page showing Products
        """
        # When
        self.login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)

        # Then
        self.login_page.assert_logged_in()
        assert page.locator(".title").text_content() == "Products"

    def test_locked_out_user_shows_error_with_icon(self, page):
        """
        Given: I am on the login page
        When: I login with a locked out user
        Then: I should see a locked out error with the error icon
        """
        # When & Then
        self.login_page.login_and_expect_error(
            Users.LOCKED_USERNAME,
            Users.LOCKED_PASSWORD,
            ErrorMessages.LOCKED_OUT
        )
        self.login_page.assert_error_icon_visible()

    def test_invalid_credentials_show_error(self, page):
        """
        Given: I am on the login page
        When: I login with invalid credentials
        Then: I should see an invalid credentials error
        """
        # When & Then
        self.login_page.login_and_expect_error(
            Users.INVALID_USERNAME,
            Users.INVALID_PASSWORD,
            ErrorMessages.INVALID_CREDENTIALS
        )

    def test_logs_out_and_blocks_inventory_access(self, page):
        """
        Given: I am logged in
        When: I log out and try to access the inventory directly
        Then: I should be redirected back to the login page
        """
        # Given
        self.login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)

        # When
        page.get_by_role("button", name="Open Menu").click()
        page.get_by_role("link", name="Logout").click()

        # Then
        assert page.url.rstrip("/") == Urls.BASE
        page.goto(Urls.INVENTORY)
        assert page.url.rstrip("/") == Urls.BASE
