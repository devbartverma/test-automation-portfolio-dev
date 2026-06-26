import { Page, expect } from '@playwright/test';
import { URLS } from '../data/testData';

/**
 * LoginPage — encapsulates all interactions with the SauceDemo login screen.
 * Assertions that belong to the page live here so tests remain concise.
 */
export class LoginPage {
  private readonly page: Page;

  // Locators as readonly fields — one place to update if selectors change
  readonly usernameInput = () => this.page.getByTestId('username');
  readonly passwordInput = () => this.page.getByTestId('password');
  readonly loginButton = () => this.page.getByTestId('login-button');
  readonly errorMessage = () => this.page.getByTestId('error');

  constructor(page: Page) {
    this.page = page;
  }

  /** Navigate directly to the login page */
  async goto(): Promise<void> {
    await this.page.goto(URLS.base);
  }

  /** Fill credentials and click Login */
  async login(username: string, password: string): Promise<void> {
    await this.usernameInput().fill(username);
    await this.passwordInput().fill(password);
    await this.loginButton().click();
  }

  /** Attempt login then assert the expected error text is displayed */
  async loginAndExpectError(username: string, password: string, errorText: string): Promise<void> {
    await this.login(username, password);
    await expect(this.errorMessage()).toBeVisible();
    await expect(this.errorMessage()).toContainText(errorText);
  }

  /** Assert the page landed on the inventory after successful login */
  async assertSuccessfulLogin(): Promise<void> {
    await expect(this.page).toHaveURL(URLS.inventory);
  }

  /** Assert the X icon exists on the error banner (visual cue for recruiters) */
  async assertErrorIconVisible(): Promise<void> {
    await expect(this.errorMessage().locator('svg[data-icon="times"]')).toBeVisible();
  }
}
