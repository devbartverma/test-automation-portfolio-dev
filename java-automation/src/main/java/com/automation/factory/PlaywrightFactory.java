package com.automation.factory;

import com.microsoft.playwright.*;

/**
 * Thread-safe Playwright lifecycle manager.
 *
 * <p>State is held in {@link ThreadLocal}s, so each test thread owns an isolated
 * Playwright / Browser / Context / Page. This lets the suite run in parallel
 * (JUnit Platform parallel execution) without threads clobbering one another.
 *
 * <p>Headless is the default (CI-friendly); set {@code HEADLESS=false} to watch
 * the browser locally.
 */
public class PlaywrightFactory {
    private static final ThreadLocal<Playwright> PLAYWRIGHT = new ThreadLocal<>();
    private static final ThreadLocal<Browser> BROWSER = new ThreadLocal<>();
    private static final ThreadLocal<BrowserContext> CONTEXT = new ThreadLocal<>();
    private static final ThreadLocal<Page> PAGE = new ThreadLocal<>();

    public static Page createPage() {
        boolean headless = !"false".equalsIgnoreCase(System.getenv("HEADLESS"));

        PLAYWRIGHT.set(Playwright.create());
        BROWSER.set(PLAYWRIGHT.get().chromium()
            .launch(new BrowserType.LaunchOptions().setHeadless(headless)));
        CONTEXT.set(BROWSER.get().newContext());

        Page page = CONTEXT.get().newPage();
        page.setDefaultTimeout(30000);
        PAGE.set(page);
        return page;
    }

    /** Current thread's page (used by the screenshot-on-failure extension). */
    public static Page getPage() {
        return PAGE.get();
    }

    public static void closeAll() {
        if (CONTEXT.get() != null) {
            CONTEXT.get().close();
            CONTEXT.remove();
        }
        if (BROWSER.get() != null) {
            BROWSER.get().close();
            BROWSER.remove();
        }
        if (PLAYWRIGHT.get() != null) {
            PLAYWRIGHT.get().close();
            PLAYWRIGHT.remove();
        }
        PAGE.remove();
    }
}
