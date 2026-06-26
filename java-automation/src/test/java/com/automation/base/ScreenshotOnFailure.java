package com.automation.base;

import com.automation.factory.PlaywrightFactory;
import com.microsoft.playwright.Page;
import org.junit.jupiter.api.extension.AfterTestExecutionCallback;
import org.junit.jupiter.api.extension.ExtensionContext;

import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

/**
 * Captures a full-page screenshot under {@code target/screenshots} whenever a
 * test fails. Runs as an {@link AfterTestExecutionCallback}, i.e. before the
 * {@code @AfterEach} teardown closes the page, so the page is still alive.
 */
public class ScreenshotOnFailure implements AfterTestExecutionCallback {

    @Override
    public void afterTestExecution(ExtensionContext context) {
        if (context.getExecutionException().isPresent()) {
            Page page = PlaywrightFactory.getPage();
            if (page != null) {
                try {
                    Path dir = Paths.get("target", "screenshots");
                    Files.createDirectories(dir);
                    String name = context.getDisplayName().replaceAll("[^a-zA-Z0-9.-]", "_");
                    page.screenshot(new Page.ScreenshotOptions()
                        .setPath(dir.resolve(name + ".png"))
                        .setFullPage(true));
                } catch (Exception ignored) {
                    // Screenshot is best-effort; never mask the real test failure.
                }
            }
        }
    }
}
