# Java Playwright Test Automation Framework

Enterprise-grade test automation framework using Playwright Java with BDD-style Groovy tests.

## Project Structure

```
java-automation/
├── src/
│   ├── main/
│   │   └── java/com/automation/
│   │       ├── data/                    # Test data and fixtures
│   │       │   ├── TestData.java       # Test data constants
│   │       │   └── ProductData.java    # Product and sort options
│   │       ├── pages/                   # Page Object Models
│   │       │   ├── LoginPage.java
│   │       │   ├── InventoryPage.java
│   │       │   ├── CartPage.java
│   │       │   ├── CheckoutPage.java
│   │       │   └── ProductDetailPage.java
│   │       └── factory/
│   │           └── PlaywrightFactory.java  # Browser instance management
│   └── test/
│       ├── java/com/automation/
│       │   └── base/
│       │       └── BaseTest.java        # JUnit 5 base test class
│       └── groovy/com/automation/
│           └── tests/                   # BDD-style test specs
│               ├── AuthTests.groovy
│               ├── InventoryTests.groovy
│               ├── CartTests.groovy
│               ├── CheckoutTests.groovy
│               └── ApiSpec.groovy        # true Spock BDD API tests
├── pom.xml                              # Maven configuration
└── README.md                            # This file
```

## Technology Stack

- **Java 11+** - Programming language
- **Playwright** - Browser automation framework
- **Groovy** - BDD-style test writing
- **JUnit 5 (Jupiter)** - Test framework
- **Maven** - Build and dependency management
- **Logback** - Logging framework

## Prerequisites

- Java 11 or higher
- Maven 3.6+
- Git

## Installation

1. Clone the repository:
```bash
git clone https://github.com/devbartverma/test-automation-portfolio-devbrat.git
cd java-automation
```

2. Install dependencies:
```bash
mvn clean install
```

3. Download Playwright browsers (one-time setup):
```bash
mvn exec:java -Dexec.mainClass=com.microsoft.playwright.CLI -Dexec.args="install"
```

## Running Tests

### Run all tests:
```bash
mvn test
```

### Run specific test class:
```bash
mvn test -Dtest=AuthTests
```

### Run with specific pattern:
```bash
mvn test -Dtest=*Tests
```

## 🧪 Test Suite — 15 Scenarios

Target app: **SauceDemo** (`standard_user` / `secret_sauce`). These 15 scenarios are implemented
identically in all four language suites of this portfolio. Method names are the BDD strings used in
each `@DisplayName` / `void "…"()` test.

### 🔐 Authentication — `AuthTests.groovy` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `logs in standard user and shows inventory` | Logs in with valid standard credentials | URL == inventory page; `.title` reads **Products** |
| `shows locked-out error with icon for locked_out_user` | Attempts login as the locked-out user | error text contains the locked-out message **and** the `svg[data-icon='times']` icon is visible |
| `shows invalid credentials error` | Logs in with a wrong username/password | error contains "Username and password do not match…" |
| `logs out and blocks access to inventory` | Logs out via burger menu, then deep-links to `inventory.html` | after logout URL == base; direct navigation redirects back to base (**route guard**) |

### 📦 Inventory — `InventoryTests.groovy` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `shows six inventory products` | Loads the catalog after login | exactly **6** products |
| `sorts products by price high to low` | Selects "Price (high to low)" | parsed prices are monotonically **descending**; first > last |
| `sorts products by name A to Z` | Selects "Name (A to Z)" | rendered names equal their alphabetically-ascending copy |
| `opens product detail page for Sauce Labs Backpack` | Opens the Backpack detail page | name == "Sauce Labs Backpack"; price == **$29.99**; description not empty; back button visible |

### 🛒 Shopping Cart — `CartTests.groovy` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `shows added items in cart with correct names` | Adds Fleece Jacket + Backpack, opens cart | both product names present; line-item count == **2** |
| `removes item from cart and updates count` | Adds 2 items, removes the Backpack | removed item gone; count == 1; badge reads **"1"** |
| `preserves cart contents after returning from cart` | Adds item, opens cart, Continue Shopping | badge still == 1 — **state survives navigation** |

### 💳 Checkout — `CheckoutTests.groovy` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `shows first name required error` | Submits step-one with a blank first name | error == "Error: First Name is required" |
| `shows correct subtotal for one item` | Checks out a single Backpack to the overview | parsed subtotal == item price (±0.01) |
| `verifies subtotal plus tax equals total` | Checks out 2 items to the overview | **subtotal + tax == total** (2-decimal tolerance) — financial integrity |
| `completes checkout with two items and verifies confirmation` | Sort → add 2 → full purchase flow | sort verified → badge 2 → both items in cart → subtotal == sum → confirmation header == "Thank you for your order!" |

### 🌐 REST API — `ApiSpec.groovy` (3, true Spock BDD)

A real **Spock** specification with `given:` / `when:` / `then:` / `where:` blocks — genuine BDD
and genuinely data-driven (the `where:` table), using Playwright's `APIRequestContext`.

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `GET /posts/#postId …` | Fetches a single post — **`where:` table** over ids 1 / 2 / 3 | status 200; `id` matches; `userId` Integer; `title`/`body` non-empty (schema) |
| `GET /posts …` | Fetches the collection | status 200; exactly **100** items |
| `POST /posts …` | Creates a resource | status **201**; payload echoed; numeric `id` returned |

Target: `https://jsonplaceholder.typicode.com`. **No browser launched.**

## 🔧 CI, parallel execution & failure artifacts

- **CI:** GitHub Actions runs `mvn test` on every push/PR — see [`.github/workflows/ci.yml`](../.github/workflows/ci.yml).
- **Parallel-safe:** `PlaywrightFactory` holds all state in `ThreadLocal`s, so each test thread owns an isolated browser — no cross-thread clobbering.
- **Artifacts on failure:** the `ScreenshotOnFailure` JUnit extension captures a full-page screenshot under `target/screenshots` (runs before teardown closes the page).
- **Headless by default** (CI-friendly); set `HEADLESS=false` to watch locally.

> Note: the UI tests use Groovy + JUnit 5 with Given/When/Then `@DisplayName`s (BDD-*style*);
> the API spec above is *true* Spock BDD.

## Why this is a gold-standard SDET suite

- **Page Object Model** — tests read as business intent; every locator lives in one page class.
- **Centralized, data-driven inputs** — users, URLs, products and error copy live in `data/`, never hard-coded in tests.
- **Assertions that mean something** — not "the page loaded": exact counts, sort-order math, and a real `subtotal + tax = total` financial check.
- **Positive *and* negative coverage** — happy paths plus locked-out, invalid credentials, form validation, and a logout route-guard.
- **Resilient locators** — `data-test` attributes over brittle XPath, with Playwright auto-waiting (no hard sleeps → no flakiness).
- **Deterministic & isolated** — each test logs in fresh in `setUp()`; no shared state leaks between tests.
- **Readable BDD intent** — every test carries a Given/When/Then `@DisplayName`.
- **Cross-language parity** — the same 15 scenarios exist in TypeScript, Python, Java and C#.

## Page Objects

### LoginPage
Handles login functionality and authentication flows.

### InventoryPage
Manages product listing, sorting, and cart operations.

### CartPage
Handles shopping cart operations and checkout navigation.

### CheckoutPage
Manages checkout process including customer info and order completion.

### ProductDetailPage
Handles product detail page interactions and assertions.

## Test Data

Test data is centralized in the `data` package:

- **TestData.java** - Users, URLs, error messages, and customer data
- **ProductData.java** - Product names, add-to-cart IDs, and sort options

## BDD Style

Tests are written in Groovy using BDD (Behavior-Driven Development) style with Given-When-Then structure:

```groovy
@Test
@DisplayName("Given I navigate to login page When I login with valid credentials Then I should be logged in successfully")
void "standard user can login"() {
    // Given - page is already at login
    // When
    loginPage.login(TestData.Users.STANDARD_USERNAME, TestData.Users.STANDARD_PASSWORD)
    // Then
    loginPage.assertLoggedIn()
}
```

## Best Practices Implemented

✅ **Page Object Model** - Clear separation of test logic and page interactions  
✅ **Test Data Management** - Centralized test data and constants  
✅ **Factory Pattern** - Browser instance management with PlaywrightFactory  
✅ **BDD Style** - Human-readable test descriptions using Given-When-Then  
✅ **JUnit 5** - Modern Java testing framework with annotations and lifecycle hooks  
✅ **Assertions** - Clear and descriptive assertions  
✅ **Base Test Class** - Reusable setup and teardown logic  

## Debugging

To run tests in headed mode (see browser), modify `PlaywrightFactory.java`:

```java
browser = playwright.chromium().launch(new BrowserType.LaunchOptions().setHeadless(false));
```

For slow motion and debugging:
```java
browser = playwright.chromium().launch(new BrowserType.LaunchOptions()
    .setHeadless(false)
    .setSlowMo(1000));
```

## Continuous Integration

The framework is ready for CI/CD integration. Run tests in headless mode in CI pipelines:

```bash
mvn test -DargLine=--headless
```

## Contributing

1. Create a new branch for your feature
2. Write tests using the BDD style
3. Follow the page object model pattern
4. Run all tests before submitting a pull request

## License

MIT License - See LICENSE file for details
