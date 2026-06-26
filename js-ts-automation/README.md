# TypeScript Playwright Test Automation Framework

UI test automation framework built with **TypeScript + Playwright Test**, using the Page Object
Model, centralized data-driven inputs, and cross-browser execution. This is the reference suite
for the portfolio — the same 15 scenarios are mirrored in the Python, Java, and C# suites.

## Project Structure

```
js-ts-automation/
├── playwright/
│   ├── Pages/                      # Page Object Models
│   ├── api/                        # REST API tests (api.spec.ts)
│   │   ├── LoginPage.ts
│   │   ├── InventoryPage.ts
│   │   ├── CartPage.ts
│   │   └── CheckoutPage.ts
│   ├── data/
│   │   └── testData.ts             # Users, URLs, products, error messages
│   └── tests/
│       ├── auth.spec.ts
│       ├── inventory.spec.ts
│       ├── cart.spec.ts
│       └── checkout.spec.ts
├── playwright.config.ts            # Browsers, reporter, base config
├── package.json
└── README.md
```

## Technology Stack

- **TypeScript** — strongly typed test code
- **Playwright Test** — runner, fixtures, auto-waiting, tracing
- **Page Object Model (POM)**
- **Cross-browser** — Chromium, Firefox, WebKit
- **HTML reporter** with trace-on-first-retry

## Prerequisites

- Node.js 18+
- npm

## Installation

```bash
cd js-ts-automation
npm install
npx playwright install   # one-time browser download
```

## Running Tests

```bash
npm test                 # all tests (UI cross-browser + API)
npm run test:ui          # UI suites only
npm run test:api         # API suite only
npm run report           # open the HTML report

# or call Playwright directly:
npx playwright test auth.spec.ts         # a single spec
npx playwright test --project=chromium   # one browser
```

## 🧪 Test Suite — 15 Scenarios

Target app: **SauceDemo** (`standard_user` / `secret_sauce`). These 15 scenarios are implemented
identically in all four language suites of this portfolio.

### 🔐 Authentication — `auth.spec.ts` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `logs in standard user and shows inventory` | Logs in with valid standard credentials | URL == inventory page; `.title` reads **Products** |
| `shows locked-out error with icon for locked_out_user` | Attempts login as the locked-out user | error text contains the locked-out message **and** the `svg[data-icon='times']` icon is visible |
| `shows invalid credentials error` | Logs in with a wrong username/password | error contains "Username and password do not match…" |
| `logs out and blocks access to inventory` | Logs out via burger menu, then deep-links to `inventory.html` | after logout URL == base; direct navigation redirects back to base (**route guard**) |

### 📦 Inventory — `inventory.spec.ts` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `shows six inventory products` | Loads the catalog after login | exactly **6** products |
| `sorts products by price high to low` | Selects "Price (high to low)" | parsed prices are monotonically **descending**; first > last |
| `sorts products by name A to Z` | Selects "Name (A to Z)" | rendered names equal their alphabetically-ascending copy |
| `opens product detail page for Sauce Labs Backpack` | Opens the Backpack detail page | name == "Sauce Labs Backpack"; price == **$29.99**; description not empty; back button visible |

### 🛒 Shopping Cart — `cart.spec.ts` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `shows added items in cart with correct names` | Adds Fleece Jacket + Backpack, opens cart | both product names present; line-item count == **2** |
| `removes item from cart and updates count` | Adds 2 items, removes the Backpack | removed item gone; count == 1; badge reads **"1"** |
| `preserves cart contents after returning from cart` | Adds item, opens cart, Continue Shopping | badge still == 1 — **state survives navigation** |

### 💳 Checkout — `checkout.spec.ts` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `shows first name required error` | Submits step-one with a blank first name | error == "Error: First Name is required" |
| `shows correct subtotal for one item` | Checks out a single Backpack to the overview | parsed subtotal == item price (±0.01) |
| `verifies subtotal plus tax equals total` | Checks out 2 items to the overview | **subtotal + tax == total** (2-decimal tolerance) — financial integrity |
| `completes checkout with two items and verifies confirmation` | Sort → add 2 → full purchase flow | sort verified → badge 2 → both items in cart → subtotal == sum → confirmation header == "Thank you for your order!" |

### 🌐 REST API — `playwright/api/api.spec.ts` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `GET /posts/{id}` | Fetches a single post — **data-driven** over ids 1 / 2 / 3 | status 200; `id` matches; `userId` numeric; `title`/`body` non-empty (schema) |
| `GET /posts` | Fetches the collection | status 200; exactly **100** items |
| `POST /posts` | Creates a resource | status **201**; payload echoed; numeric `id` returned |

Target: `https://jsonplaceholder.typicode.com`. Runs in the `api` project — **no browser launched**.

## 🔧 CI, parallel execution & failure artifacts

- **CI:** GitHub Actions runs this suite cross-browser on every push/PR — see [`.github/workflows/ci.yml`](../.github/workflows/ci.yml).
- **Parallel-safe:** `fullyParallel: true` — Playwright shards across workers out of the box.
- **Artifacts on failure:** `trace`, `screenshot`, and `video` are all `retain-on-failure`; open with `npx playwright show-report`.

## Why this is a gold-standard SDET suite

- **Page Object Model** — tests read as business intent; every locator lives in one page class.
- **Centralized, data-driven inputs** — users, URLs, products and error copy live in `data/testData.ts`, never hard-coded in tests.
- **Assertions that mean something** — not "the page loaded": exact counts, sort-order math, and a real `subtotal + tax = total` financial check.
- **Positive *and* negative coverage** — happy paths plus locked-out, invalid credentials, form validation, and a logout route-guard.
- **Resilient locators** — `data-test` attributes (`testIdAttribute`) over brittle XPath, with Playwright auto-waiting (no hard sleeps → no flakiness).
- **Deterministic & isolated** — each test logs in fresh in `beforeEach`; no shared state between tests.
- **Cross-browser & traceable** — Chromium / Firefox / WebKit with trace-on-first-retry for fast triage.
- **Cross-language parity** — the same 15 scenarios exist in TypeScript, Python, Java and C#.

## Author

**Devbrat Verma** — Senior QA Automation Engineer | SDET

## License

MIT License — see the repository `LICENSE` file.
