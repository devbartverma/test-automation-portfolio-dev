# C# Playwright Automation – Devbrat Verma

A modern UI test automation framework built with **C#, .NET 8, NUnit, and Playwright**, demonstrating scalable test architecture, maintainable automation design patterns, and QA engineering best practices used in enterprise software development.

> Framework designed and implemented by Devbrat Verma to demonstrate enterprise-grade UI test automation using Playwright for .NET, C#, Page Object Model (POM), reporting, and scalable test architecture.

## Overview

This repository showcases my approach to designing and implementing robust automated testing solutions using Playwright for .NET. The framework includes end-to-end test scenarios against public demo applications and demonstrates clean code practices, maintainability, and reliability in test automation.

## Technology Stack

* C#
* .NET 8
* Playwright for .NET
* NUnit
* Page Object Model (POM)
* HTML Reporting
* Data-Driven Testing
* Cross-Browser Execution

## Framework Features

### Architecture & Design

* Page Object Model (POM) implementation
* Reusable and maintainable page classes
* Centralized test data management
* Clear separation of test logic and page interactions
* Scalable framework structure suitable for enterprise projects

### Automation Best Practices

* Reliable locator strategies to minimize flaky tests
* Data-driven test execution
* Cross-browser testing (Chromium, Firefox, WebKit)
* Detailed execution reporting and test artifacts
* Positive and negative scenario coverage
* Reusable test components and utilities

## 🧪 Test Suite — 15 Scenarios

Target app: **SauceDemo** (`standard_user` / `secret_sauce`). These 15 scenarios are implemented
identically in all four language suites of this portfolio.

### 🔐 Authentication — `AuthTests.cs` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `LogsInStandardUserAndShowsInventory` | Logs in with valid standard credentials | URL == inventory page; `.title` reads **Products** |
| `ShowsLockedOutErrorWithIcon` | Attempts login as the locked-out user | error text contains the locked-out message **and** the `svg[data-icon='times']` error icon is visible |
| `ShowsInvalidCredentialsError` | Logs in with a wrong username/password | error contains "Username and password do not match…" |
| `LogsOutAndBlocksAccessToInventory` | Logs out via burger menu, then deep-links to `inventory.html` | after logout URL == base; direct navigation redirects back to base (**route guard**) |

### 📦 Inventory — `InventoryTests.cs` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `ShowsSixInventoryProducts` | Loads the catalog after login | exactly **6** `.inventory_item` elements |
| `SortsProductsByPriceHighToLow` | Selects "Price (high to low)" | parsed prices are monotonically **descending**; first > last |
| `SortsProductsByNameAToZ` | Selects "Name (A to Z)" | rendered names equal their alphabetically-ascending copy |
| `OpensProductDetailPageForBackpack` | Opens the Backpack detail page | name == "Sauce Labs Backpack"; price == **$29.99**; description not empty; back-to-products visible |

### 🛒 Shopping Cart — `CartTests.cs` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `ShowsAddedItemsInCartWithCorrectNames` | Adds Fleece Jacket + Backpack, opens cart | both product names present; line-item count == **2** |
| `RemovesItemFromCartAndUpdatesCount` | Adds 2 items, removes the Backpack | removed item gone; count == 1; badge reads **"1"** |
| `PreservesCartContentsAfterReturningFromCart` | Adds item, opens cart, Continue Shopping | badge still == 1 — **state survives navigation** |

### 💳 Checkout — `CheckoutTests.cs` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `ShowsFirstNameRequiredError` | Submits step-one with a blank first name | error == "Error: First Name is required" |
| `ShowsCorrectSubtotalForOneItem` | Checks out a single Backpack to the overview | parsed subtotal == item price (±0.01) |
| `VerifiesSubtotalPlusTaxEqualsTotal` | Checks out 2 items to the overview | **subtotal + tax == total** (2-decimal tolerance) — financial integrity |
| `CompletesCheckoutWithTwoItemsAndVerifiesConfirmation` | Sort → add 2 → full purchase flow | sort verified → badge 2 → both items in cart → subtotal == sum → confirmation header == "Thank you for your order!" |

### 🌐 REST API — `ApiTests.cs` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `GetPostByIdReturnsMatchingSchema` | Fetches a single post — **`[TestCase]`** over ids 1 / 2 / 3 | status 200; `id` matches; `userId` > 0; `title`/`body` non-empty (schema) |
| `GetAllPostsReturnsFullCollection` | Fetches the collection | status 200; exactly **100** items |
| `CreatePostReturns201AndEchoesPayload` | Creates a resource | status **201**; payload echoed; numeric `id` returned |

Target: `https://jsonplaceholder.typicode.com` (Playwright `IAPIRequestContext`). **No browser launched.**

## 🔧 CI, parallel execution & failure artifacts

- **CI:** GitHub Actions runs `dotnet test` on every push/PR — see [`.github/workflows/ci.yml`](../.github/workflows/ci.yml).
- **Parallel-safe:** every UI test inherits `PageTest`, which gives each test its own isolated browser context — run wider with `dotnet test -- NUnit.NumberOfTestWorkers=N`.
- **Artifacts on failure:** `BasePageTest` captures a full-page screenshot under `./screenshots` and attaches it to the NUnit result; CI also emits a `.trx` report.

## Why this is a gold-standard SDET suite

- **Page Object Model** — tests read as business intent; every locator lives in one page class.
- **Centralized, data-driven inputs** — users, URLs, products and error copy live in `data/`, never hard-coded in tests.
- **Assertions that mean something** — not "the page loaded": exact counts, sort-order math, and a real `subtotal + tax = total` financial check.
- **Positive *and* negative coverage** — happy paths plus locked-out, invalid credentials, form validation, and a logout route-guard.
- **Resilient locators** — `data-test` attributes over brittle XPath, with Playwright auto-waiting (no hard `sleep`s → no flakiness).
- **Deterministic & isolated** — each test logs in fresh in `[SetUp]`; no shared state leaks between tests.
- **Cross-language parity** — the same 15 scenarios exist in TypeScript, Python, Java and C#, proving framework-design skill independent of any one stack.

## Project Structure

```text
csharp-automation/
├── TestResults/
│   ├── HtmlReport/
│   ├── HtmlReportTrx/
│   └── TestResults.trx
│
├── playwright/
│   ├── Pages/
│   │   ├── LoginPage.cs
│   │   ├── InventoryPage.cs
│   │   ├── ProductDetailPage.cs
│   │   ├── CartPage.cs
│   │   └── CheckoutPage.cs
│   │
│   ├── data/
│   │   ├── TestData.cs
│   │   └── ProductData.cs
│   │
│   └── tests/
│       ├── BasePageTest.cs        # screenshot-on-failure base
│       ├── AuthTests.cs
│       ├── InventoryTests.cs
│       ├── CartTests.cs
│       ├── CheckoutTests.cs
│       └── ApiTests.cs            # REST API tests
│
├── CSharpPlaywright.csproj
└── README.md
```

## Purpose

This portfolio project demonstrates my expertise in designing, developing, and maintaining modern test automation frameworks using Playwright for .NET and C#. It reflects the engineering practices I use to build scalable, reliable, and maintainable automation solutions for web applications.

## Author

**Devbrat Verma**

Senior QA Automation Engineer | SDET | Quality Engineering Lead

Specializing in Test Automation, Quality Engineering, and DevOps practices using Playwright, Cypress, Serenity BDD, SpecFlow, Reqnroll, JavaScript, TypeScript, Java, Groovy, and C#, with expertise in scalable automation frameworks, CI/CD pipelines, and AI-assisted testing.
