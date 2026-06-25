# Test Automation Portfolio – Devbrat Verma

A multi-language Playwright automation portfolio demonstrating Page Object Model design, 
data-driven testing, and quality engineering best practices across TypeScript, C# (.NET 8), 
Java (Groovy BDD), and Python.

---

## 🛠️ Frameworks

| Language | Location | Test Runner | Tests |
|----------|----------|-------------|-------|
| TypeScript | `/js-ts-automation` | Playwright Test | 35+ |
| C# / .NET 8 | `/dotnet-automation` | NUnit | 5 |
| Java + Groovy BDD | `/java-automation` | JUnit 5 | 5 |
| Python | `/python-automation` | PyTest | 5 |

---

## 🎯 Target Application

* **URL:** [https://www.saucedemo.com](https://www.saucedemo.com)
* **Credentials:** `standard_user` / `secret_sauce`

---

## ✅ Test Coverage

### TypeScript (full coverage)
- Authentication — positive, negative, locked user, session, logout, route protection
- Inventory — all 4 sort options, product detail, burger menu reset
- Cart — add/remove, badge validation, persistence across navigation
- Checkout — form validation, price math (`subtotal + tax = total`), full E2E flow

### C# / Java / Python (core coverage)
- Authentication — valid login, locked user, invalid credentials
- Inventory — product count, add to cart
- Checkout — full E2E flow, form validation

---

## ⚡ Key Practices

- Page Object Model consistently applied across all 4 languages
- Centralised test data (Users, URLs, Products, ErrorMessages) per framework
- BDD-style Given/When/Then in Java (Groovy) and Python (docstrings)
- Resilient locators using `data-test` attributes
- Mathematical price verification (`subtotal + tax = total`) — TypeScript suite
- HTML + TRX automated test reports — C# suite

---

## 👨‍💻 Author

**Devbrat Verma** — QA Automation Engineer  
[GitHub](https://github.com/devbartverma)

---

## 📄 License
MIT