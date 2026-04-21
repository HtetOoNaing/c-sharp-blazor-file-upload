using BlazorDemo.Components.Layout;
using Bunit;

namespace BlazorDemo.Tests.ComponentTests;

public class NavMenuTests : TestContext
{
    [Fact]
    public void RendersAppBrandName()
    {
        var cut = Render<NavMenu>();

        var brand = cut.Find(".navbar-brand");
        Assert.Equal("BlazorDemo", brand.TextContent);
    }

    [Fact]
    public void RendersHomeLink()
    {
        var cut = Render<NavMenu>();

        var links = cut.FindAll(".nav-link");
        Assert.Contains(links, link => link.TextContent.Contains("Home"));
    }

    [Fact]
    public void RendersCounterLink()
    {
        var cut = Render<NavMenu>();

        var links = cut.FindAll(".nav-link");
        Assert.Contains(links, link => link.TextContent.Contains("Counter"));
    }

    [Fact]
    public void RendersWeatherLink()
    {
        var cut = Render<NavMenu>();

        var links = cut.FindAll(".nav-link");
        Assert.Contains(links, link => link.TextContent.Contains("Weather"));
    }

    [Fact]
    public void HasThreeNavLinks()
    {
        var cut = Render<NavMenu>();

        var links = cut.FindAll(".nav-link");
        Assert.Equal(3, links.Count);
    }

    [Fact]
    public void CounterLink_HasCorrectHref()
    {
        var cut = Render<NavMenu>();

        var counterLink = cut.FindAll(".nav-link")
            .First(l => l.TextContent.Contains("Counter"));

        Assert.Equal("counter", counterLink.GetAttribute("href"));
    }

    [Fact]
    public void WeatherLink_HasCorrectHref()
    {
        var cut = Render<NavMenu>();

        var weatherLink = cut.FindAll(".nav-link")
            .First(l => l.TextContent.Contains("Weather"));

        Assert.Equal("weather", weatherLink.GetAttribute("href"));
    }
}
