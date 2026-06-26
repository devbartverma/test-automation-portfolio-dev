import { test, expect } from '@playwright/test';
import { LoginPage } from '../Pages/LoginPage';
import { InventoryPage } from '../Pages/InventoryPage';
import { CartPage } from '../Pages/CartPage';
import { PRODUCTS, USERS } from '../data/testData';

test.describe('Shopping Cart', () => {
  let inventoryPage: InventoryPage;
  let cartPage: CartPage;

  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.login(USERS.standard.username, USERS.standard.password);
    inventoryPage = new InventoryPage(page);
    cartPage = new CartPage(page);
  });

  test('shows added items in cart with correct names', async () => {
    await inventoryPage.addMultipleToCart([
      PRODUCTS.fleeceJacket.dataTest,
      PRODUCTS.backpack.dataTest,
    ]);
    await inventoryPage.goToCart();

    await cartPage.assertItemInCart(PRODUCTS.fleeceJacket.name);
    await cartPage.assertItemInCart(PRODUCTS.backpack.name);
    await cartPage.assertItemCount(2);
  });

  test('removes item from cart and updates count', async () => {
    await inventoryPage.addMultipleToCart([
      PRODUCTS.backpack.dataTest,
      PRODUCTS.bikeLight.dataTest,
    ]);
    await inventoryPage.goToCart();

    await cartPage.removeItem(PRODUCTS.backpack.name);

    await cartPage.assertItemNotInCart(PRODUCTS.backpack.name);
    await cartPage.assertItemCount(1);
    await expect(cartPage.page.locator('.shopping_cart_badge')).toHaveText('1');
  });

  test('preserves cart contents after returning from cart', async () => {
    await inventoryPage.addToCartByDataTest(PRODUCTS.backpack.dataTest);
    await inventoryPage.goToCart();
    await cartPage.continueShopping();
    await inventoryPage.assertCartBadgeCount(1);
  });
});
