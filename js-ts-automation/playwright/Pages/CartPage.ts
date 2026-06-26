import { Page, expect } from '@playwright/test';
import { URLS } from '../data/testData';

/**
 * CartPage — asserts item presence, quantities, and initiates checkout.
 */
export class CartPage {
  readonly page: Page;

  readonly itemNames = () => this.page.locator('.inventory_item_name');
  readonly itemPrices = () => this.page.locator('.inventory_item_price');
  readonly itemQuantities = () => this.page.locator('.cart_quantity');
  readonly continueShoppingButton = () => this.page.getByTestId('continue-shopping');
  readonly checkoutButton = () => this.page.getByTestId('checkout');

  constructor(page: Page) {
    this.page = page;
  }

  async goto(): Promise<void> {
    await this.page.goto(URLS.cart);
  }

  async proceedToCheckout(): Promise<void> {
    await this.checkoutButton().click();
    await expect(this.page).toHaveURL(URLS.checkoutStep1);
  }

  async continueShopping(): Promise<void> {
    await this.continueShoppingButton().click();
    await expect(this.page).toHaveURL(URLS.inventory);
  }

  async removeItem(productName: string): Promise<void> {
    // Build the data-test id from the product name dynamically
    const id = productName
      .toLowerCase()
      .replace(/[()]/g, '')
      .replace(/\s+/g, '-');
    await this.page.getByTestId(`remove-${id}`).click();
  }

  // ─── Assertions ──────────────────────────────────────────────────────────────

  async assertItemInCart(productName: string): Promise<void> {
    await expect(this.page.getByText(productName, { exact: true })).toBeVisible();
  }

  async assertItemNotInCart(productName: string): Promise<void> {
    await expect(this.page.getByText(productName, { exact: true })).not.toBeVisible();
  }

  async assertItemCount(expected: number): Promise<void> {
    await expect(this.itemNames()).toHaveCount(expected);
  }

  async assertItemAtIndex(index: number, name: string): Promise<void> {
    await expect(this.itemNames().nth(index)).toHaveText(name);
  }

  async assertCartIsEmpty(): Promise<void> {
    await expect(this.itemNames()).toHaveCount(0);
  }

  /** Retrieve all cart prices as numbers for downstream math assertions */
  async getCartPrices(): Promise<number[]> {
    const raw = await this.itemPrices().allTextContents();
    return raw.map(p => parseFloat(p.replace('$', '')));
  }
}
