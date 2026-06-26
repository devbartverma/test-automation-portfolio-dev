package com.automation.tests

import com.automation.base.BaseTest
import com.automation.pages.LoginPage
import com.automation.data.TestData
import com.microsoft.playwright.options.AriaRole
import com.microsoft.playwright.Page
import org.junit.jupiter.api.DisplayName
import org.junit.jupiter.api.Test

import static org.junit.jupiter.api.Assertions.*

@DisplayName("Authentication Tests - BDD Style")
class AuthTests extends BaseTest {
    private LoginPage loginPage

    @Override
    void setUp() {
        super.setUp()
        loginPage = new LoginPage(page)
        loginPage.goTo()
    }

    @Test
    @DisplayName("Given I navigate to login page When I login with valid credentials Then I should see the Products page")
    void "logs in standard user and shows inventory"() {
        // Given - page is already at login
        // When
        loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)
        // Then
        loginPage.assertLoggedIn()
        assertEquals("Products", page.locator(".title").textContent(),
            "Products title should be displayed")
    }

    @Test
    @DisplayName("Given I navigate to login page When I login with a locked out user Then I should see a locked out error with icon")
    void "shows locked-out error with icon for locked_out_user"() {
        // Given - page is already at login
        // When & Then
        loginPage.loginAndExpectError(TestData.Users.LOCKED_USERNAME, TestData.Users.LOCKED_PASSWORD,
            TestData.ErrorMessages.LOCKED_OUT)
        loginPage.assertErrorIconVisible()
    }

    @Test
    @DisplayName("Given I navigate to login page When I login with invalid credentials Then I should see an invalid credentials error")
    void "shows invalid credentials error"() {
        // Given - page is already at login
        // When & Then
        loginPage.loginAndExpectError(TestData.Users.INVALID_USERNAME, TestData.Users.INVALID_PASSWORD,
            TestData.ErrorMessages.INVALID_CREDENTIALS)
    }

    @Test
    @DisplayName("Given I am logged in When I log out Then access to inventory should be blocked")
    void "logs out and blocks access to inventory"() {
        // Given
        loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)
        // When
        page.getByRole(AriaRole.BUTTON, new Page.GetByRoleOptions().setName("Open Menu")).click()
        page.getByRole(AriaRole.LINK, new Page.GetByRoleOptions().setName("Logout")).click()
        // Then
        assertEquals(TestData.Urls.BASE, page.url().replaceAll("/\$", ""), "User should be redirected to login after logout")
        page.navigate(TestData.Urls.INVENTORY)
        assertEquals(TestData.Urls.BASE, page.url().replaceAll("/\$", ""), "Inventory access should be blocked when logged out")
    }
}
