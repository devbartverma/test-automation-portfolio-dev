using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace CSharpPlaywright.Tests;

/// <summary>
/// Base class for UI tests. Inherits Playwright's <see cref="PageTest"/> (which
/// gives every test an isolated browser context — safe for parallel execution)
/// and captures a full-page screenshot under ./screenshots when a test fails.
/// </summary>
public class BasePageTest : PageTest
{
    [TearDown]
    public async Task CaptureScreenshotOnFailureAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, $"{TestContext.CurrentContext.Test.Name}.png");

            await Page.ScreenshotAsync(new() { Path = file, FullPage = true });
            TestContext.AddTestAttachment(file);
        }
    }
}
