using Microsoft.Playwright;
using NUnit.Framework;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Pages;

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

    public async Task GoToAsync() => await _page.GotoAsync(Urls.Base);

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task LoginAndExpectErrorAsync(string username, string password, string expectedText)
    {
        await LoginAsync(username, password);
        Assert.That(await ErrorMessage.IsVisibleAsync(), Is.True);
        Assert.That(await ErrorMessage.TextContentAsync(), Does.Contain(expectedText));
    }

    public Task AssertLoggedInAsync()
    {
        Assert.That(_page.Url, Is.EqualTo(Urls.Inventory));
        return Task.CompletedTask;
    }
}
