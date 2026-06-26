import { Page, Locator, expect } from '@playwright/test';
import { URLS } from '../data/testData';

export type SortOption = 'az' | 'za' | 'lohi' | 'hilo';

/**
 * InventoryPage — product listing, sorting, and add-to-cart interactions.
 */
export class InventoryPage {
  private readonly page: Page;

  readonly sortDropdown = () => this.page.getByTestId('product-sort-container');
  readonly cartBadge = () => this.page.locator('.shopping_cart_badge');
  readonly cartLink = () => this.page.locator('.shopping_cart_link');
  readonly productItems = () => this.page.locator('.inventory_item');
  readonly productNames = () => this.page.locator('.inventory_item_name');
  readonly productPrices = () => this.page.locator('.inventory_item_price');
  readonly burgerMenuButton = () => this.page.getByRole('button', { name: 'Open Menu' });
  readonly resetAppStateLink = () => this.page.getByRole('link', { name: 'Reset App State' });
  readonly logoutLink = () => this.page.getByRole('link', { name: 'Logout' });

  constructor(page: Page) {
    this.page = page;
  }

  async goto(): Promise<void> {
    await this.page.goto(URLS.inventory);
  }

  /** Select a sort option from the dropdown */
  async sortBy(option: SortOption): Promise<void> {
    await this.sortDropdown().selectOption(option);
  }

  /** Click the Add-to-Cart button for a product by its data-test id */
  async addToCartByDataTest(dataTestId: string): Promise<void> {
    await this.page.getByTestId(dataTestId).click();
  }

  /** Add multiple products by passing an array of data-test ids */
  async addMultipleToCart(dataTestIds: string[]): Promise<void> {
    for (const id of dataTestIds) {
      await this.addToCartByDataTest(id);
    }
  }

  /** Remove an item from the inventory page via its remove button data-test id */
  async removeFromCartByDataTest(dataTestId: string): Promise<void> {
    await this.page.getByTestId(dataTestId).click();
  }

  /** Click an item name to open the product detail page */
  async openProductDetail(productName: string): Promise<void> {
    await this.page.getByText(productName, { exact: true }).click();
  }

  /** Navigate to cart */
  async goToCart(): Promise<void> {
    await this.cartLink().click();
    await expect(this.page).toHaveURL(URLS.cart);
  }

  /** Open burger menu and click Logout */
  async logout(): Promise<void> {
    await this.burgerMenuButton().click();
    await this.logoutLink().click();
    await expect(this.page).toHaveURL(URLS.base);
  }

  /** Open burger menu and reset app state (clears cart without navigating) */
  async resetAppState(): Promise<void> {
    await this.burgerMenuButton().click();
    await this.resetAppStateLink().click();
  }

  // ─── Assertions ──────────────────────────────────────────────────────────────

  async assertCartBadgeCount(expected: number): Promise<void> {
    await expect(this.cartBadge()).toBeVisible();
    await expect(this.cartBadge()).toHaveText(String(expected));
  }

  async assertCartBadgeNotVisible(): Promise<void> {
    await expect(this.cartBadge()).not.toBeVisible();
  }

  /**
   * Retrieve all prices as numbers and assert they are sorted descending.
   * Returns the parsed array so tests can do further assertions.
   */
  async assertPricesSortedDescending(): Promise<number[]> {
    const rawPrices = await this.productPrices().allTextContents();
    const prices = rawPrices.map(p => parseFloat(p.replace('$', '')));
    const isSorted = prices.every((val, i, arr) => i === 0 || arr[i - 1] >= val);
    expect(isSorted, `Prices not sorted descending: ${prices}`).toBe(true);
    return prices;
  }

  async assertPricesSortedAscending(): Promise<number[]> {
    const rawPrices = await this.productPrices().allTextContents();
    const prices = rawPrices.map(p => parseFloat(p.replace('$', '')));
    const isSorted = prices.every((val, i, arr) => i === 0 || arr[i - 1] <= val);
    expect(isSorted, `Prices not sorted ascending: ${prices}`).toBe(true);
    return prices;
  }

  async assertNamesAlphabeticallyAscending(): Promise<string[]> {
    const names = await this.productNames().allTextContents();
    const sorted = [...names].sort((a, b) => a.localeCompare(b));
    expect(names).toEqual(sorted);
    return names;
  }

  async assertNamesAlphabeticallyDescending(): Promise<string[]> {
    const names = await this.productNames().allTextContents();
    const sorted = [...names].sort((a, b) => b.localeCompare(a));
    expect(names).toEqual(sorted);
    return names;
  }

  async assertProductCount(expected: number): Promise<void> {
    await expect(this.productItems()).toHaveCount(expected);
  }
}
