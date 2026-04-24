using System.Net;
using System.Net.Http.Json;

namespace FeatureFlags.Api.Tests;

[TestFixture]
public class FeatureFlagsTests
{
    private FeatureFlagsFactory _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new FeatureFlagsFactory();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task Get_ReturnsHelloFromFeatureFlags()
    {
        var response = await _client.GetAsync("/");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Hello from Feature Flags"));
    }

    [Test]
    public async Task Flags_ReturnsPaginatedFlags()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Flag>>("/api/flags");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Items, Has.Count.GreaterThan(0));
        Assert.That(result.Items, Has.All.Matches<Flag>(f => f.Name != null));
        Assert.That(result.Items, Has.Some.Matches<Flag>(f => string.Equals(f.Name, "dark-mode", StringComparison.Ordinal)));
        Assert.That(result.Page, Is.Not.Null);
    }

    [Test]
    public async Task Flags_Toggle_ChangesEnabledState()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Flag>>("/api/flags");
        var darkMode = result!.Items.Single(f => string.Equals(f.Name, "dark-mode", StringComparison.Ordinal));

        var response = await _client.PutAsync("/api/flags/dark-mode/toggle", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var toggled = await response.Content.ReadFromJsonAsync<Flag>();
        Assert.That(toggled!.Enabled, Is.Not.EqualTo(darkMode.Enabled));
    }

    [Test]
    public async Task Flags_Toggle_UnknownFlag_ReturnsNotFound()
    {
        var response = await _client.PutAsync("/api/flags/unknown-flag/toggle", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task HealthLive_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health/live");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task HealthReady_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health/ready");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Flags_WithAllowedOrigin_ReturnsCorsHeader()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/flags");
        request.Headers.Add("Origin", "http://localhost:3000");

        var response = await _client.SendAsync(request);

        Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True);
    }

    private record PagedResult<T>(List<T> Items, PageInfo Page);
    private record PageInfo(int TotalCount);
    private record Flag(string Name, bool Enabled);
}
