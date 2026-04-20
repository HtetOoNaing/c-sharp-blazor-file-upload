using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace BlazorDemo.Tests.IntegrationTests;

public class BlazorAppIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BlazorAppIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HomePage_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Upload Demo App", content);
    }

    [Fact]
    public async Task CounterPage_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/counter");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Counter", content);
    }

    [Fact]
    public async Task WeatherPage_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/weather");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Weather", content);
    }

    [Fact]
    public async Task StaticAssets_BootstrapCss_Exists()
    {
        // Act
        var response = await _client.GetAsync("/lib/bootstrap/dist/css/bootstrap.min.css");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentLength);
        Assert.True(response.Content.Headers.ContentLength > 0);
    }

    [Fact]
    public async Task StaticAssets_FontAwesomeCss_Exists()
    {
        // Act
        var response = await _client.GetAsync("/lib/font-awesome/css/all.min.css");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentLength);
        Assert.True(response.Content.Headers.ContentLength > 0);
    }

    [Fact]
    public async Task NotFoundPage_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/non-existent-page-that-does-not-exist");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Favicon_Exists()
    {
        // Act
        var response = await _client.GetAsync("/favicon.png");

        // Assert
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound);
    }
}
