using BlazorDemo.Components.Pages;
using BlazorDemo.Models;
using BlazorDemo.Services;
using Bunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Tests.ComponentTests;

public class HomeTests : TestContext, IDisposable
{
    private readonly string _tempDir;

    public HomeTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"BlazorHomeTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        var options = Options.Create(new UploadOptions());

        var env = new FakeWebHostEnvironment(_tempDir);

        Services.AddSingleton<IOptions<UploadOptions>>(options);
        Services.AddSingleton<IFileValidationService>(
            new FileValidationService(options, NullLogger<FileValidationService>.Instance));
        Services.AddSingleton<IFilePreviewService>(sp =>
            new FilePreviewService(
                sp.GetRequiredService<IFileValidationService>(),
                NullLogger<FilePreviewService>.Instance));
        Services.AddSingleton<IFileUploadService>(
            new FileUploadService(env, options, NullLogger<FileUploadService>.Instance));
        Services.AddLogging();
    }

    public new void Dispose()
    {
        base.Dispose();
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public FakeWebHostEnvironment(string webRootPath)
        {
            WebRootPath = webRootPath;
            ContentRootPath = webRootPath;
            EnvironmentName = "Testing";
            ApplicationName = "BlazorDemo.Tests";
        }

        public string WebRootPath { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public void RendersPageTitle()
    {
        var cut = Render<Home>();

        var title = cut.Find("h1");
        Assert.Equal("Upload Demo App", title.TextContent);
    }

    [Fact]
    public void RendersFirstNameInput()
    {
        var cut = Render<Home>();

        var input = cut.Find("#firstName");
        Assert.NotNull(input);
    }

    [Fact]
    public void RendersLastNameInput()
    {
        var cut = Render<Home>();

        var input = cut.Find("#lastName");
        Assert.NotNull(input);
    }

    [Fact]
    public void RendersFileInput()
    {
        var cut = Render<Home>();

        var fileInput = cut.Find("input[type='file']");
        Assert.NotNull(fileInput);
    }

    [Fact]
    public void RendersUploadButton()
    {
        var cut = Render<Home>();

        var button = cut.Find("button[type='submit']");
        Assert.Contains("Upload Files", button.TextContent);
    }

    [Fact]
    public void UploadButton_IsDisabledByDefault()
    {
        var cut = Render<Home>();

        var button = cut.Find("button[type='submit']");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void RendersMaxFilesInfo()
    {
        var cut = Render<Home>();

        Assert.Contains("Max files: 3", cut.Markup);
        Assert.Contains("Max file size: 3MB", cut.Markup);
    }

    [Fact]
    public void NoErrorsOrSuccessMessages_Initially()
    {
        var cut = Render<Home>();

        Assert.DoesNotContain("alert-danger", cut.Markup);
        Assert.DoesNotContain("alert-success", cut.Markup);
    }

    [Fact]
    public void NoSelectedFiles_Initially()
    {
        var cut = Render<Home>();

        Assert.DoesNotContain("Selected Files", cut.Markup);
    }
}
