using BlazorDemo.Components.Pages;
using Bunit;

namespace BlazorDemo.Tests.ComponentTests;

public class WeatherTests : TestContext
{
    [Fact]
    public void RendersLoadingState_Initially()
    {
        var cut = Render<Weather>();

        Assert.Contains("Loading...", cut.Markup);
    }

    [Fact]
    public void RendersPageTitle()
    {
        var cut = Render<Weather>();

        var title = cut.Find("h1");
        Assert.Equal("Weather", title.TextContent);
    }

    [Fact]
    public async Task RendersTable_AfterDataLoads()
    {
        var cut = Render<Weather>();

        // Wait for the simulated async delay to complete
        cut.WaitForState(() => !cut.Markup.Contains("Loading..."), TimeSpan.FromSeconds(3));

        var table = cut.Find("table");
        Assert.NotNull(table);
    }

    [Fact]
    public async Task RendersCorrectTableHeaders()
    {
        var cut = Render<Weather>();

        cut.WaitForState(() => !cut.Markup.Contains("Loading..."), TimeSpan.FromSeconds(3));

        var headers = cut.FindAll("thead th");
        Assert.Equal(4, headers.Count);
        Assert.Equal("Date", headers[0].TextContent);
        Assert.Equal("Summary", headers[3].TextContent);
    }

    [Fact]
    public async Task RendersFiveForecasts()
    {
        var cut = Render<Weather>();

        cut.WaitForState(() => !cut.Markup.Contains("Loading..."), TimeSpan.FromSeconds(3));

        var rows = cut.FindAll("tbody tr");
        Assert.Equal(5, rows.Count);
    }

    [Fact]
    public async Task ForecastRows_HaveFourColumns()
    {
        var cut = Render<Weather>();

        cut.WaitForState(() => !cut.Markup.Contains("Loading..."), TimeSpan.FromSeconds(3));

        var firstRowCells = cut.FindAll("tbody tr:first-child td");
        Assert.Equal(4, firstRowCells.Count);
    }
}
