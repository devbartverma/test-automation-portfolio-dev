import os
from pathlib import Path

import pytest
from playwright.sync_api import Page

from src.automation.factory.playwright_factory import PlaywrightFactory

SCREENSHOTS_DIR = Path("screenshots")


@pytest.fixture(scope="function")
def page(request) -> Page:
    """
    Provides a fully isolated Playwright page per test (its own browser and
    context), so the suite is safe to run in parallel with pytest-xdist.

    On failure, a full-page screenshot is saved under ./screenshots.
    Headless is the default (CI-friendly); use --headed or HEADLESS=false to see
    the browser, and --slowmo <ms> to slow execution down.
    """
    headless = _resolve_headless(request)
    slow_mo = request.config.getoption("--slowmo")

    factory = PlaywrightFactory()
    page_instance = factory.create_page(headless=headless, slow_mo=slow_mo)

    yield page_instance

    report = getattr(request.node, "rep_call", None)
    if report is not None and report.failed:
        SCREENSHOTS_DIR.mkdir(exist_ok=True)
        safe_name = request.node.name.replace("/", "_").replace("::", "_")
        try:
            page_instance.screenshot(
                path=str(SCREENSHOTS_DIR / f"{safe_name}.png"), full_page=True
            )
        except Exception:
            pass

    factory.close_all()


@pytest.hookimpl(hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """Expose each test phase's result on the item so fixtures can react to failures."""
    outcome = yield
    report = outcome.get_result()
    setattr(item, f"rep_{report.when}", report)


def _resolve_headless(request) -> bool:
    if request.config.getoption("--headed"):
        return False
    return os.getenv("HEADLESS", "true").lower() != "false"


def pytest_addoption(parser):
    """Add custom command-line options for pytest."""
    parser.addoption(
        "--headed",
        action="store_true",
        default=False,
        help="Run tests in headed mode (see browser)",
    )
    parser.addoption(
        "--slowmo",
        action="store",
        type=int,
        default=0,
        help="Slow down execution by the specified number of milliseconds",
    )
