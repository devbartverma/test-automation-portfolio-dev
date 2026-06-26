import { test, expect } from '@playwright/test';
import { LoginPage } from '../Pages/LoginPage';
import { USERS, URLS, ERROR_MESSAGES } from '../data/testData';

test.describe('Authentication', () => {
  let loginPage: LoginPage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    await loginPage.goto();
  });

  test('logs in standard user and shows inventory', async ({ page }) => {
    await loginPage.login(USERS.standard.username, USERS.standard.password);
    await loginPage.assertSuccessfulLogin();
    await expect(page.locator('.title')).toHaveText('Products');
  });

  test('shows locked-out error with icon for locked_out_user', async () => {
    await loginPage.loginAndExpectError(
      USERS.locked.username,
      USERS.locked.password,
      ERROR_MESSAGES.lockedOut
    );
    await loginPage.assertErrorIconVisible();
  });

  test('shows invalid credentials error', async () => {
    await loginPage.loginAndExpectError(
      USERS.invalid.username,
      USERS.invalid.password,
      ERROR_MESSAGES.invalidCredentials
    );
  });

  test('logs out and blocks access to inventory', async ({ page }) => {
    await loginPage.login(USERS.standard.username, USERS.standard.password);
    await page.getByRole('button', { name: 'Open Menu' }).click();
    await page.getByRole('link', { name: 'Logout' }).click();
    await expect(page).toHaveURL(/www\.saucedemo\.com\/?$/);
    await page.goto(URLS.inventory);
    await expect(page).toHaveURL(/www\.saucedemo\.com\/?$/);
  });
});
