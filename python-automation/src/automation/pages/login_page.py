from playwright.sync_api import Page
from src.automation.data.test_data import Users, Urls, ErrorMessages


class LoginPage:
    def __init__(self, page: Page):
        self.page = page

    @property
    def username_input(self):
        return self.page.locator("[data-test='username']")

    @property
    def password_input(self):
        return self.page.locator("[data-test='password']")

    @property
    def login_button(self):
        return self.page.locator("[data-test='login-button']")

    @property
    def error_message(self):
        return self.page.locator("[data-test='error']")

    def go_to(self):
        self.page.goto(Urls.BASE)

    def login(self, username: str, password: str):
        self.username_input.fill(username)
        self.password_input.fill(password)
        self.login_button.click()

    def login_and_expect_error(self, username: str, password: str, expected_text: str):
        self.login(username, password)
        assert self.error_message.is_visible(), "Error message should be visible"
        assert expected_text in self.error_message.text_content(), \
            f"Error message should contain: {expected_text}"

    def assert_logged_in(self):
        assert self.page.url == Urls.INVENTORY, \
            f"User should be redirected to {Urls.INVENTORY}, but got {self.page.url}"

    def assert_error_icon_visible(self):
        icon = self.error_message.locator("svg[data-icon='times']")
        assert icon.is_visible(), "Error icon (svg[data-icon='times']) should be visible"
