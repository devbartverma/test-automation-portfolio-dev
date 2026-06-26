using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class AuthTests : BasePageTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.GoToAsync();
    }

    [Test]
    [Description("logs in standard user and shows inventory")]
    public async Task LogsInStandardUserAndShowsInventory()
    {
        await _loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);
        await _loginPage.AssertLoggedInAsync();
        Assert.That(await Page.Locator(".title").TextContentAsync(), Is.EqualTo("Products"));
    }

    [Test]
    [Description("shows locked-out error with icon for locked_out_user")]
    public async Task ShowsLockedOutErrorWithIcon()
    {
        await _loginPage.LoginAndExpectErrorAsync(Users.LockedUsername, Users.LockedPassword, ErrorMessages.LockedOut);
        await _loginPage.AssertErrorIconVisibleAsync();
    }

    [Test]
    [Description("shows invalid credentials error")]
    public async Task ShowsInvalidCredentialsError()
    {
        await _loginPage.LoginAndExpectErrorAsync(Users.InvalidUsername, Users.InvalidPassword, ErrorMessages.InvalidCredentials);
    }

    [Test]
    [Description("logs out and blocks access to inventory")]
    public async Task LogsOutAndBlocksAccessToInventory()
    {
        await _loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Logout" }).ClickAsync();
        Assert.That(Page.Url.TrimEnd('/'), Is.EqualTo(Urls.Base));

        await Page.GotoAsync(Urls.Inventory);
        Assert.That(Page.Url.TrimEnd('/'), Is.EqualTo(Urls.Base));
    }
}
