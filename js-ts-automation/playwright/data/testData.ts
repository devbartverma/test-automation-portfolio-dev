// fixtures/testData.ts
// Centralised data store — swap values here to change test behaviour everywhere

export const USERS = {
  standard: {
    username: 'standard_user',
    password: 'secret_sauce',
  },
  locked: {
    username: 'locked_out_user',
    password: 'secret_sauce',
  },
  performance_glitch: {
    username: 'performance_glitch_user',
    password: 'secret_sauce',
  },
  invalid: {
    username: 'invalid_user',
    password: 'wrong_password',
  },
} as const;

export const URLS = {
  base: 'https://www.saucedemo.com',
  inventory: 'https://www.saucedemo.com/inventory.html',
  cart: 'https://www.saucedemo.com/cart.html',
  checkoutStep1: 'https://www.saucedemo.com/checkout-step-one.html',
  checkoutStep2: 'https://www.saucedemo.com/checkout-step-two.html',
  checkoutComplete: 'https://www.saucedemo.com/checkout-complete.html',
} as const;

export const CUSTOMER = {
  firstName: 'Devbrat',
  lastName: 'Verma',
  postalCode: '110001',
} as const;

export const PRODUCTS = {
  backpack: {
    dataTest: 'add-to-cart-sauce-labs-backpack',
    name: 'Sauce Labs Backpack',
    price: 29.99,
  },
  bikeLight: {
    dataTest: 'add-to-cart-sauce-labs-bike-light',
    name: 'Sauce Labs Bike Light',
    price: 9.99,
  },
  boltTShirt: {
    dataTest: 'add-to-cart-sauce-labs-bolt-t-shirt',
    name: 'Sauce Labs Bolt T-Shirt',
    price: 15.99,
  },
  fleeceJacket: {
    dataTest: 'add-to-cart-sauce-labs-fleece-jacket',
    name: 'Sauce Labs Fleece Jacket',
    price: 49.99,
  },
  onesie: {
    dataTest: 'add-to-cart-sauce-labs-onesie',
    name: 'Sauce Labs Onesie',
    price: 7.99,
  },
  redTShirt: {
    dataTest: 'add-to-cart-test.allthethings()-t-shirt-(red)',
    name: 'Test.allTheThings() T-Shirt (Red)',
    price: 15.99,
  },
} as const;

export const SORT_OPTIONS = {
  nameAZ: 'az',
  nameZA: 'za',
  priceLoHi: 'lohi',
  priceHiLo: 'hilo',
} as const;

export const ERROR_MESSAGES = {
  lockedOut: 'Epic sadface: Sorry, this user has been locked out.',
  invalidCredentials: 'Epic sadface: Username and password do not match any user in this service',
  missingUsername: 'Epic sadface: Username is required',
  missingPassword: 'Epic sadface: Password is required',
  missingFirstName: 'Error: First Name is required',
  missingLastName: 'Error: Last Name is required',
  missingPostalCode: 'Error: Postal Code is required',
} as const;
