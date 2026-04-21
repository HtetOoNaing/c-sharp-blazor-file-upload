using BlazorDemo.Components.Pages;
using Bunit;

namespace BlazorDemo.Tests.ComponentTests;

public class CounterTests : TestContext
{
    [Fact]
    public void RendersInitialCountOfZero()
    {
        var cut = Render<Counter>();

        cut.Find("p[role='status']").MarkupMatches(
            "<p role=\"status\">Current count: 0</p>");
    }

    [Fact]
    public void ClickingButton_IncrementsCount()
    {
        var cut = Render<Counter>();

        cut.Find("button").Click();

        cut.Find("p[role='status']").MarkupMatches(
            "<p role=\"status\">Current count: 1</p>");
    }

    [Fact]
    public void ClickingButton_MultipleTimes_IncrementsCorrectly()
    {
        var cut = Render<Counter>();

        cut.Find("button").Click();
        cut.Find("button").Click();
        cut.Find("button").Click();

        cut.Find("p[role='status']").MarkupMatches(
            "<p role=\"status\">Current count: 3</p>");
    }

    [Fact]
    public void RendersPageTitle()
    {
        var cut = Render<Counter>();

        var title = cut.Find("h1");
        Assert.Equal("Counter", title.TextContent);
    }

    [Fact]
    public void ButtonHasCorrectText()
    {
        var cut = Render<Counter>();

        var button = cut.Find("button");
        Assert.Equal("Click me", button.TextContent);
    }

    [Fact]
    public void ButtonHasPrimaryClass()
    {
        var cut = Render<Counter>();

        var button = cut.Find("button");
        Assert.Contains("btn-primary", button.ClassName);
    }
}
