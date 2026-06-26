using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

/// <summary>
/// LoginPage — encapsulates all interactions with the SauceDemo login screen.
/// Assertions that belong to the page live here so tests remain concise.
/// </summary>
public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public ILocator UsernameInput => _page.Locator("[data-test='username']");
    public ILocator PasswordInput => _page.Locator("[data-test='password']");
    public ILocator LoginButton => _page.Locator("[data-test='login-button']");
    public ILocator ErrorMessage => _page.Locator("[data-test='error']");

    /// <summary>Navigate directly to the login page.</summary>
    public async Task GoToAsync() => await _page.GotoAsync(Urls.Base);

    /// <summary>Fill credentials and click Login.</summary>
    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    /// <summary>Attempt login then assert the expected error text is displayed.</summary>
    public async Task LoginAndExpectErrorAsync(string username, string password, string expectedText)
    {
        await LoginAsync(username, password);
        Assert.That(await ErrorMessage.IsVisibleAsync(), Is.True);
        Assert.That(await ErrorMessage.TextContentAsync(), Does.Contain(expectedText));
    }

    /// <summary>Assert the page landed on the inventory after successful login.</summary>
    public Task AssertLoggedInAsync()
    {
        Assert.That(_page.Url, Is.EqualTo(Urls.Inventory));
        return Task.CompletedTask;
    }

    /// <summary>Assert the X icon exists on the error banner (visual cue for recruiters).</summary>
    public async Task AssertErrorIconVisibleAsync()
    {
        Assert.That(await ErrorMessage.Locator("svg[data-icon='times']").IsVisibleAsync(), Is.True);
    }
}
