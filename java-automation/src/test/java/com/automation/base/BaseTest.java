package com.automation.base;

import com.microsoft.playwright.Page;
import com.automation.factory.PlaywrightFactory;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.extension.ExtendWith;

@ExtendWith(ScreenshotOnFailure.class)
public class BaseTest {
    protected Page page;

    @BeforeEach
    public void setUp() {
        page = PlaywrightFactory.createPage();
    }

    @AfterEach
    public void tearDown() {
        PlaywrightFactory.closeAll();
    }
}
