import { Page, expect } from '@playwright/test';
import { URLS } from '../data/testData';

export interface CustomerInfo {
  firstName: string;
  lastName: string;
  postalCode: string;
}

/**
 * CheckoutPage — covers Step 1 (info), Step 2 (overview), and confirmation.
 */
export class CheckoutPage {
  private readonly page: Page;

  readonly firstNameInput = () => this.page.getByTestId('firstName');
  readonly lastNameInput = () => this.page.getByTestId('lastName');
  readonly postalCodeInput = () => this.page.getByTestId('postalCode');
  readonly continueButton = () => this.page.getByTestId('continue');
  readonly cancelButton = () => this.page.getByTestId('cancel');
  readonly step1ErrorMessage = () => this.page.getByTestId('error');

  readonly subtotalLabel = () => this.page.locator('.summary_subtotal_label');
  readonly taxLabel = () => this.page.locator('.summary_tax_label');
  readonly totalLabel = () => this.page.locator('.summary_total_label');
  readonly finishButton = () => this.page.getByTestId('finish');

  readonly completeHeader = () => this.page.getByTestId('complete-header');
  readonly completeText = () => this.page.getByTestId('complete-text');
  readonly backHomeButton = () => this.page.getByTestId('back-to-products');

  constructor(page: Page) {
    this.page = page;
  }

  // ─── Step 1 actions ─────────────────────────────────────────────────────────

  async fillCustomerInfo(info: CustomerInfo): Promise<void> {
    await this.firstNameInput().fill(info.firstName);
    await this.lastNameInput().fill(info.lastName);
    await this.postalCodeInput().fill(info.postalCode);
  }

  async continueToOverview(): Promise<void> {
    await this.continueButton().click();
    await expect(this.page).toHaveURL(URLS.checkoutStep2);
  }

  async fillAndContinue(info: CustomerInfo): Promise<void> {
    await this.fillCustomerInfo(info);
    await this.continueToOverview();
  }

  /** Submit empty form to trigger validation; returns so callers can chain */
  async submitEmptyForm(): Promise<void> {
    await this.continueButton().click();
  }

  // ─── Step 2 actions ─────────────────────────────────────────────────────────

  async finishOrder(): Promise<void> {
    await this.finishButton().click();
    await expect(this.page).toHaveURL(URLS.checkoutComplete);
  }

  /** Returns { subtotal, tax, total } as numbers for math verification */
  async getPriceSummary(): Promise<{ subtotal: number; tax: number; total: number }> {
    const subtotalText = (await this.subtotalLabel().textContent()) ?? '';
    const taxText = (await this.taxLabel().textContent()) ?? '';
    const totalText = (await this.totalLabel().textContent()) ?? '';

    return {
      subtotal: parseFloat(subtotalText.replace('Item total: $', '').trim()),
      tax: parseFloat(taxText.replace('Tax: $', '').trim()),
      total: parseFloat(totalText.replace('Total: $', '').trim()),
    };
  }

  // ─── Assertions ──────────────────────────────────────────────────────────────

  async assertStep1Error(errorText: string): Promise<void> {
    await expect(this.step1ErrorMessage()).toBeVisible();
    await expect(this.step1ErrorMessage()).toContainText(errorText);
  }

  /** Core financial assertion: subtotal + tax must equal total */
  async assertTotalCalculationIsCorrect(): Promise<void> {
    const { subtotal, tax, total } = await this.getPriceSummary();
    expect(
      subtotal + tax,
      `Expected subtotal (${subtotal}) + tax (${tax}) = ${subtotal + tax}, got ${total}`
    ).toBeCloseTo(total, 2);
  }

  async assertConfirmationPage(): Promise<void> {
    await expect(this.page).toHaveURL(URLS.checkoutComplete);
    await expect(this.completeHeader()).toBeVisible();
    await expect(this.completeHeader()).toHaveText('Thank you for your order!');
  }

  async assertOrderConfirmationDetails(): Promise<void> {
    await this.assertConfirmationPage();
    await expect(this.completeText()).toContainText('Your order has been dispatched');
    await expect(this.backHomeButton()).toBeVisible();
  }
}
