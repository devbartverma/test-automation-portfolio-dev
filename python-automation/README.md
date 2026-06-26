# Python Playwright Test Automation Framework

Enterprise-grade test automation framework using Playwright Python with BDD-style pytest tests.

## Project Structure

```
python-automation/
├── src/
│   └── automation/
│       ├── data/
│       │   ├── __init__.py
│       │   ├── test_data.py              # Test data constants
│       │   └── product_data.py           # Product and sort options
│       ├── pages/
│       │   ├── __init__.py
│       │   ├── login_page.py
│       │   ├── inventory_page.py
│       │   ├── cart_page.py
│       │   ├── checkout_page.py
│       │   └── product_detail_page.py
│       ├── factory/
│       │   ├── __init__.py
│       │   └── playwright_factory.py     # Browser instance management
│       └── base/
│           └── __init__.py
├── tests/
│   ├── __init__.py
│   ├── conftest.py                       # Pytest configuration and fixtures
│   ├── test_auth.py                      # BDD-style authentication tests
│   ├── test_inventory.py                 # BDD-style inventory tests
│   ├── test_cart.py                      # BDD-style shopping cart tests
│   ├── test_checkout.py                  # BDD-style checkout tests
│   └── test_api.py                       # REST API contract tests
├── requirements.txt                       # Python dependencies
├── pytest.ini                             # Pytest configuration
├── .env.example                           # Environment variables example
└── README.md                              # This file
```

## Technology Stack

- **Python 3.8+** - Programming language
- **Playwright Python** - Browser automation framework
- **pytest** - Testing framework with BDD plugins
- **python-dotenv** - Environment variable management

## Prerequisites

- Python 3.8 or higher
- pip (Python package manager)
- Git

## Installation

1. Clone the repository:
```bash
git clone https://github.com/devbartverma/test-automation-portfolio-devbrat.git
cd python-automation
```

2. Create a virtual environment:
```bash
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
```

3. Install dependencies:
```bash
pip install -r requirements.txt
```

4. Download Playwright browsers (one-time setup):
```bash
playwright install
```

## Running Tests

### Run all tests:
```bash
pytest
```

### Run with verbose output:
```bash
pytest -v
```

### Run specific test file:
```bash
pytest tests/test_auth.py
```

### Run specific test:
```bash
pytest tests/test_auth.py::TestAuthentication::test_standard_user_can_login
```

### Run with HTML report:
```bash
pytest --html=report.html --self-contained-html
```

### Run in headed mode (see browser):
```bash
pytest --headed
```

## 🧪 Test Suite — 15 Scenarios

Target app: **SauceDemo** (`standard_user` / `secret_sauce`). These 15 scenarios are implemented
identically in all four language suites of this portfolio.

### 🔐 Authentication — `test_auth.py` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `test_standard_user_can_login` | Logs in with valid standard credentials | URL == inventory page; `.title` reads **Products** |
| `test_locked_out_user_shows_error_with_icon` | Attempts login as the locked-out user | error text contains the locked-out message **and** the `svg[data-icon='times']` icon is visible |
| `test_invalid_credentials_show_error` | Logs in with a wrong username/password | error contains "Username and password do not match…" |
| `test_logs_out_and_blocks_inventory_access` | Logs out via burger menu, then deep-links to `inventory.html` | after logout URL == base; direct navigation redirects back to base (**route guard**) |

### 📦 Inventory — `test_inventory.py` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `test_shows_six_products` | Loads the catalog after login | exactly **6** products |
| `test_sorts_products_by_price_high_to_low` | Selects "Price (high to low)" | parsed prices are monotonically **descending**; first > last |
| `test_sorts_products_by_name_a_to_z` | Selects "Name (A to Z)" | rendered names equal their alphabetically-ascending copy |
| `test_opens_product_detail_for_backpack` | Opens the Backpack detail page | name == "Sauce Labs Backpack"; price == **$29.99**; description not empty; back button visible |

### 🛒 Shopping Cart — `test_cart.py` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `test_shows_added_items_in_cart_with_names` | Adds Fleece Jacket + Backpack, opens cart | both product names present; line-item count == **2** |
| `test_removes_item_from_cart_and_updates_count` | Adds 2 items, removes the Backpack | removed item gone; count == 1; badge reads **"1"** |
| `test_preserves_cart_contents_after_continue_shopping` | Adds item, opens cart, Continue Shopping | badge still == 1 — **state survives navigation** |

### 💳 Checkout — `test_checkout.py` (4)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `test_shows_first_name_required_error` | Submits step-one with a blank first name | error == "Error: First Name is required" |
| `test_shows_correct_subtotal_for_one_item` | Checks out a single Backpack to the overview | parsed subtotal == item price (±0.01) |
| `test_subtotal_plus_tax_equals_total` | Checks out 2 items to the overview | **subtotal + tax == total** (2-decimal tolerance) — financial integrity |
| `test_completes_checkout_with_two_items_and_confirmation` | Sort → add 2 → full purchase flow | sort verified → badge 2 → both items in cart → subtotal == sum → confirmation header == "Thank you for your order!" |

### 🌐 REST API — `tests/test_api.py` (3)

| Test | What it does | Key assertions |
|------|--------------|----------------|
| `test_get_post_by_id_matches_schema` | Fetches a single post — **`@pytest.mark.parametrize`** over ids 1 / 2 / 3 | status 200; `id` matches; `userId` int; `title`/`body` non-empty (schema) |
| `test_get_all_posts_returns_full_collection` | Fetches the collection | status 200; exactly **100** items |
| `test_create_post_returns_201_and_echoes_payload` | Creates a resource | status **201**; payload echoed; numeric `id` returned |

Target: `https://jsonplaceholder.typicode.com` (via `requests`). **No browser launched.**

## 🔧 CI, parallel execution & failure artifacts

- **CI:** GitHub Actions runs this suite on every push/PR — see [`.github/workflows/ci.yml`](../.github/workflows/ci.yml).
- **Parallel-safe:** each test gets its own isolated browser + context (instance-based `PlaywrightFactory`), so `pytest -n auto` (pytest-xdist) runs cleanly with no shared state.
- **Artifacts on failure:** a `pytest_runtest_makereport` hook saves a full-page screenshot under `./screenshots`.
- **Headless by default** (CI-friendly); use `--headed` or `HEADLESS=false` locally.

## Why this is a gold-standard SDET suite

- **Page Object Model** — tests read as business intent; every locator lives in one page class.
- **Centralized, data-driven inputs** — users, URLs, products and error copy live in `data/`, never hard-coded in tests.
- **Assertions that mean something** — not "the page loaded": exact counts, sort-order math, and a real `subtotal + tax = total` financial check.
- **Positive *and* negative coverage** — happy paths plus locked-out, invalid credentials, form validation, and a logout route-guard.
- **Resilient locators** — `data-test` attributes over brittle XPath, with Playwright auto-waiting (no hard sleeps → no flakiness).
- **Deterministic & isolated** — each test logs in fresh via an autouse fixture; no shared state leaks between tests.
- **Readable BDD intent** — every test carries a Given/When/Then docstring.
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

- **test_data.py** - Users, URLs, error messages, and customer data
- **product_data.py** - Product names, add-to-cart IDs, and sort options

## BDD Style

Tests are written using descriptive test names and docstrings following Given-When-Then structure:

```python
def test_standard_user_can_login(self, page):
    """
    Given: I navigate to login page
    When: I login with valid credentials
    Then: I should be logged in successfully
    """
    # Given - page is already at login
    login_page = LoginPage(page)
    login_page.go_to()
    
    # When
    login_page.login(Users.STANDARD_USERNAME, Users.STANDARD_PASSWORD)
    
    # Then
    login_page.assert_logged_in()
```

## Best Practices Implemented

✅ **Page Object Model** - Clear separation of test logic and page interactions
✅ **Test Data Management** - Centralized test data and constants
✅ **Factory Pattern** - Browser instance management with PlaywrightFactory
✅ **BDD Style** - Descriptive test names and Given-When-Then structure
✅ **pytest Fixtures** - Reusable fixtures for setup and teardown
✅ **Assertions** - Clear and descriptive assertions
✅ **Conftest** - Centralized pytest configuration and fixtures
✅ **Virtual Environment** - Isolated Python environment

## Debugging

To run tests in headed mode (see browser):

```bash
pytest --headed
```

For debugging with pdb:
```bash
pytest --pdb
```

For slow motion:
```bash
pytest --slowmo 1000
```

## Environment Variables

Create a `.env` file based on `.env.example`:

```env
HEADLESS=False
SLOWMO=0
TIMEOUT=30000
```

## Continuous Integration

The framework is ready for CI/CD integration. Run tests in headless mode:

```bash
pytest --headed=False
```

## Contributing

1. Create a new branch for your feature
2. Write tests using descriptive names
3. Follow the page object model pattern
4. Run all tests before submitting a pull request

## License

MIT License - See LICENSE file for details
