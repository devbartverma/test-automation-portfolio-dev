using Microsoft.Playwright;
using NUnit.Framework;

namespace CSharpPlaywright.Tests;

/// <summary>
/// REST API contract tests against JSONPlaceholder, using Playwright's
/// IAPIRequestContext. No browser is launched.
/// </summary>
public class ApiTests
{
    private const string BaseUrl = "https://jsonplaceholder.typicode.com";

    private IPlaywright _playwright = null!;
    private IAPIRequestContext _request = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _request = await _playwright.APIRequest.NewContextAsync(new() { BaseURL = BaseUrl });
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _request.DisposeAsync();
        _playwright.Dispose();
    }

    // Data-driven: the same contract is asserted for several resources.
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [Description("GET /posts/{id} returns a post matching the schema")]
    public async Task GetPostByIdReturnsMatchingSchema(int postId)
    {
        var response = await _request.GetAsync($"/posts/{postId}");
        Assert.That(response.Status, Is.EqualTo(200));

        var body = await response.JsonAsync();
        Assert.That(body!.Value.GetProperty("id").GetInt32(), Is.EqualTo(postId));
        Assert.That(body.Value.GetProperty("userId").GetInt32(), Is.GreaterThan(0));
        Assert.That(body.Value.GetProperty("title").GetString(), Is.Not.Empty);
        Assert.That(body.Value.GetProperty("body").GetString(), Is.Not.Empty);
    }

    [Test]
    [Description("GET /posts returns the full collection of 100 posts")]
    public async Task GetAllPostsReturnsFullCollection()
    {
        var response = await _request.GetAsync("/posts");
        Assert.That(response.Status, Is.EqualTo(200));

        var body = await response.JsonAsync();
        Assert.That(body!.Value.GetArrayLength(), Is.EqualTo(100));
    }

    [Test]
    [Description("POST /posts creates a resource and echoes the payload")]
    public async Task CreatePostReturns201AndEchoesPayload()
    {
        var payload = new { title = "SDET portfolio", body = "created via API test", userId = 1 };
        var response = await _request.PostAsync("/posts", new() { DataObject = payload });
        Assert.That(response.Status, Is.EqualTo(201));

        var body = await response.JsonAsync();
        Assert.That(body!.Value.GetProperty("title").GetString(), Is.EqualTo(payload.title));
        Assert.That(body.Value.GetProperty("body").GetString(), Is.EqualTo(payload.body));
        Assert.That(body.Value.GetProperty("id").GetInt32(), Is.GreaterThan(0));
    }
}
