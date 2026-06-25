using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using CSharpPlaywright.Pages;
using CSharpPlaywright.Fixtures;

namespace CSharpPlaywright.Tests;

public class AuthTests : PageTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.GoToAsync();
    }

    [Test]
    public async Task StandardUserCanLogin()
    {
        await _loginPage.LoginAsync(Users.StandardUsername, Users.StandardPassword);
        await _loginPage.AssertLoggedInAsync();
        Assert.That(await Page.Locator(".title").TextContentAsync(), Is.EqualTo("Products"));
    }

    [Test]
    public async Task LockedOutUserShowsError()
    {
        await _loginPage.LoginAndExpectErrorAsync(Users.LockedUsername, Users.LockedPassword, ErrorMessages.LockedOut);
    }

    [Test]
    public async Task InvalidCredentialsShowError()
    {
        await _loginPage.LoginAndExpectErrorAsync(Users.InvalidUsername, Users.InvalidPassword, ErrorMessages.InvalidCredentials);
    }
}
