from typing import Optional

from playwright.sync_api import (
    sync_playwright,
    Playwright,
    Browser,
    BrowserContext,
    Page,
)


class PlaywrightFactory:
    """
    Instance-based Playwright lifecycle manager.

    Each instance owns its own Playwright / Browser / Context / Page, so every
    test gets a fully isolated browser session. This makes the suite safe to run
    in parallel (e.g. ``pytest -n auto`` with pytest-xdist) — there is no shared
    static state to clobber between workers.
    """

    def __init__(self) -> None:
        self._playwright: Optional[Playwright] = None
        self._browser: Optional[Browser] = None
        self._context: Optional[BrowserContext] = None
        self._page: Optional[Page] = None

    def create_page(self, headless: bool = True, slow_mo: int = 0) -> Page:
        """Start Playwright and return a fresh, isolated page."""
        self._playwright = sync_playwright().start()
        self._browser = self._playwright.chromium.launch(
            headless=headless,
            slow_mo=slow_mo,
            args=["--disable-blink-features=AutomationControlled"],
        )
        self._context = self._browser.new_context()
        self._page = self._context.new_page()
        self._page.set_default_timeout(30000)
        return self._page

    @property
    def page(self) -> Optional[Page]:
        return self._page

    def close_all(self) -> None:
        """Tear down all resources owned by this instance."""
        if self._context is not None:
            self._context.close()
            self._context = None
        if self._browser is not None:
            self._browser.close()
            self._browser = None
        if self._playwright is not None:
            self._playwright.stop()
            self._playwright = None
        self._page = None
