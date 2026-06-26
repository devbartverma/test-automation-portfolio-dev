package com.automation.pages;

import com.microsoft.playwright.Page;
import com.microsoft.playwright.Locator;
import com.automation.data.TestData;

import static org.junit.jupiter.api.Assertions.*;

public class LoginPage {
    private final Page page;

    public LoginPage(Page page) {
        this.page = page;
    }

    private Locator getUsernameInput() {
        return page.locator("[data-test='username']");
    }

    private Locator getPasswordInput() {
        return page.locator("[data-test='password']");
    }

    private Locator getLoginButton() {
        return page.locator("[data-test='login-button']");
    }

    private Locator getErrorMessage() {
        return page.locator("[data-test='error']");
    }

    public void goTo() {
        page.navigate(TestData.Urls.BASE);
    }

    public void login(String username, String password) {
        getUsernameInput().fill(username);
        getPasswordInput().fill(password);
        getLoginButton().click();
    }

    public void loginAndExpectError(String username, String password, String expectedText) {
        login(username, password);
        assertTrue(getErrorMessage().isVisible(), "Error message should be visible");
        assertTrue(getErrorMessage().textContent().contains(expectedText),
            "Error message should contain: " + expectedText);
    }

    public void assertLoggedIn() {
        assertEquals(TestData.Urls.INVENTORY, page.url(), "User should be redirected to inventory page");
    }

    /** Assert the X icon exists on the error banner (visual cue for recruiters) */
    public void assertErrorIconVisible() {
        Locator icon = getErrorMessage().locator("svg[data-icon='times']");
        assertTrue(icon.isVisible(), "Error icon should be visible on the error banner");
    }
}
