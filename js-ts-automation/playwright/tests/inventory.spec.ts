import { test, expect } from '@playwright/test';
import { LoginPage } from '../Pages/LoginPage';
import { InventoryPage } from '../Pages/InventoryPage';
import { PRODUCTS, SORT_OPTIONS, USERS } from '../data/testData';

test.describe('Inventory / Product Listing', () => {
  let inventoryPage: InventoryPage;

  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.login(USERS.standard.username, USERS.standard.password);
    inventoryPage = new InventoryPage(page);
  });

  test('shows six inventory products', async () => {
    await inventoryPage.assertProductCount(6);
  });

  test('sorts products by price high to low', async () => {
    await inventoryPage.sortBy(SORT_OPTIONS.priceHiLo);
    const prices = await inventoryPage.assertPricesSortedDescending();
    expect(prices[0]).toBeGreaterThan(prices[prices.length - 1]);
  });

  test('sorts products by name A to Z', async () => {
    await inventoryPage.sortBy(SORT_OPTIONS.nameAZ);
    await inventoryPage.assertNamesAlphabeticallyAscending();
  });

  test('opens product detail page for Sauce Labs Backpack', async ({ page }) => {
    await inventoryPage.openProductDetail(PRODUCTS.backpack.name);

    await expect(page.locator('.inventory_details_name')).toHaveText(PRODUCTS.backpack.name);
    await expect(page.locator('.inventory_details_price')).toHaveText(`$${PRODUCTS.backpack.price}`);
    await expect(page.locator('.inventory_details_desc')).not.toBeEmpty();
    await expect(page.getByTestId('back-to-products')).toBeVisible();
  });
});
