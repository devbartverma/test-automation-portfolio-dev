import { test, expect } from '@playwright/test';
import { LoginPage } from '../Pages/LoginPage';
import { InventoryPage } from '../Pages/InventoryPage';
import { CartPage } from '../Pages/CartPage';
import { CheckoutPage } from '../Pages/CheckoutPage';
import { CUSTOMER, ERROR_MESSAGES, PRODUCTS, USERS } from '../data/testData';

async function loginAndAddItems(
  page: import('@playwright/test').Page,
  productDataTestIds: string[]
): Promise<{ inventoryPage: InventoryPage; cartPage: CartPage; checkoutPage: CheckoutPage }> {
  const loginPage = new LoginPage(page);
  await loginPage.goto();
  await loginPage.login(USERS.standard.username, USERS.standard.password);

  const inventoryPage = new InventoryPage(page);
  await inventoryPage.addMultipleToCart(productDataTestIds);
  await inventoryPage.goToCart();

  const cartPage = new CartPage(page);
  await cartPage.proceedToCheckout();

  const checkoutPage = new CheckoutPage(page);
  return { inventoryPage, cartPage, checkoutPage };
}

test.describe('Checkout Flow', () => {
  test('shows first name required error', async ({ page }) => {
    const { checkoutPage } = await loginAndAddItems(page, [PRODUCTS.backpack.dataTest]);
    await checkoutPage.fillCustomerInfo({ firstName: '', lastName: CUSTOMER.lastName, postalCode: CUSTOMER.postalCode });
    await checkoutPage.submitEmptyForm();
    await checkoutPage.assertStep1Error(ERROR_MESSAGES.missingFirstName);
  });

  test('shows correct subtotal for one item', async ({ page }) => {
    const { checkoutPage } = await loginAndAddItems(page, [PRODUCTS.backpack.dataTest]);
    await checkoutPage.fillAndContinue(CUSTOMER);

    const { subtotal } = await checkoutPage.getPriceSummary();
    expect(subtotal).toBeCloseTo(PRODUCTS.backpack.price, 2);
  });

  test('verifies subtotal plus tax equals total', async ({ page }) => {
    const { checkoutPage } = await loginAndAddItems(page, [
      PRODUCTS.fleeceJacket.dataTest,
      PRODUCTS.backpack.dataTest,
    ]);
    await checkoutPage.fillAndContinue(CUSTOMER);
    await checkoutPage.assertTotalCalculationIsCorrect();
  });

  test('completes checkout with two items and verifies confirmation', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.login(USERS.standard.username, USERS.standard.password);

    const inventoryPage = new InventoryPage(page);
    await inventoryPage.sortBy('hilo');
    await inventoryPage.assertPricesSortedDescending();

    await inventoryPage.addMultipleToCart([
      PRODUCTS.fleeceJacket.dataTest,
      PRODUCTS.backpack.dataTest,
    ]);
    await inventoryPage.assertCartBadgeCount(2);

    await inventoryPage.goToCart();
    const cartPage = new CartPage(page);
    await cartPage.assertItemInCart(PRODUCTS.fleeceJacket.name);
    await cartPage.assertItemInCart(PRODUCTS.backpack.name);
    await cartPage.assertItemCount(2);

    await cartPage.proceedToCheckout();
    const checkoutPage = new CheckoutPage(page);
    await checkoutPage.fillAndContinue(CUSTOMER);
    await checkoutPage.assertTotalCalculationIsCorrect();
    expect((await checkoutPage.getPriceSummary()).subtotal).toBeCloseTo(PRODUCTS.fleeceJacket.price + PRODUCTS.backpack.price, 2);

    await checkoutPage.finishOrder();
    await checkoutPage.assertConfirmationPage();
  });
});
