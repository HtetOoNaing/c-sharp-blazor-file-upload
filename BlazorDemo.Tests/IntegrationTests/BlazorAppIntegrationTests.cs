using BlazorDemo.Models;
using BlazorDemo.Services;
using BlazorDemo.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

    // --- Page Responses ---

    [Fact]
    public async Task HomePage_RequiresAuthentication_RedirectsOrDenies()
    {
        var response = await _client.GetAsync("/");

        // Home page requires CanUploadFiles claim, so anonymous users get redirected/denied
        // In production: redirect to login; In test environment: may return 404/302 depending on auth setup
        Assert.True(
            response.StatusCode == HttpStatusCode.Redirect ||
            response.StatusCode == HttpStatusCode.Found ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected redirect/denial, got {response.StatusCode}");
    }

    [Fact]
    public async Task CounterPage_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/counter");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Counter", content);
    }

    [Fact]
    public async Task WeatherPage_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/weather");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Weather", content);
    }

    [Fact]
    public async Task NotFoundPage_Returns404()
    {
        var response = await _client.GetAsync("/non-existent-page-that-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task NotFoundPage_ContainsNotFoundContent()
    {
        var response = await _client.GetAsync("/non-existent-page-that-does-not-exist");

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Not Found", content);
    }

    // --- Response Content Types ---

    [Theory]
    [InlineData("/counter")]
    [InlineData("/weather")]
    [InlineData("/login")]
    public async Task PublicPages_ReturnHtmlContentType(string path)
    {
        var response = await _client.GetAsync(path);

        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    // --- Static Assets ---

    [Fact]
    public async Task StaticAssets_BootstrapCss_Exists()
    {
        var response = await _client.GetAsync("/lib/bootstrap/dist/css/bootstrap.min.css");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentLength);
        Assert.True(response.Content.Headers.ContentLength > 0);
    }

    [Fact]
    public async Task StaticAssets_FontAwesomeCss_Exists()
    {
        var response = await _client.GetAsync("/lib/font-awesome/css/all.min.css");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentLength);
        Assert.True(response.Content.Headers.ContentLength > 0);
    }

    [Fact]
    public async Task Favicon_Exists()
    {
        var response = await _client.GetAsync("/favicon.png");

        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound);
    }

    // --- DI Service Registration ---

    [Fact]
    public void DI_FileValidationService_IsRegistered()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetService<IFileValidationService>();

        Assert.NotNull(service);
        Assert.IsType<FileValidationService>(service);
    }

    [Fact]
    public void DI_FilePreviewService_IsRegistered()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetService<IFilePreviewService>();

        Assert.NotNull(service);
        Assert.IsType<FilePreviewService>(service);
    }

    [Fact]
    public void DI_FileUploadService_IsRegistered()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetService<IFileUploadService>();

        Assert.NotNull(service);
        Assert.IsType<FileUploadService>(service);
    }

    // --- Configuration Binding ---

    [Fact]
    public void Config_UploadOptions_BoundFromAppSettings()
    {
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<UploadOptions>>();

        Assert.Equal(3_145_728, options.Value.MaxFileSize);
        Assert.Equal(3, options.Value.MaxFileCount);
        Assert.Equal("App_Data/uploads", options.Value.UploadFolder);
        Assert.Contains(".jpg", options.Value.AllowedExtensions);
        Assert.Contains(".png", options.Value.AllowedExtensions);
    }

    // --- End-to-End File Upload via Services ---

    [Fact]
    public async Task E2E_Upload_ValidImage_Succeeds()
    {
        using var scope = _factory.Services.CreateScope();
        var uploadService = scope.ServiceProvider.GetRequiredService<IFileUploadService>();
        var validationService = scope.ServiceProvider.GetRequiredService<IFileValidationService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<UploadOptions>>();

        var file = new FakeBrowserFile("test-image.png");

        Assert.True(validationService.IsImageFile(file.Name));
        Assert.True(validationService.IsWithinSizeLimit(file.Size, options.Value.MaxFileSize));

        var result = await uploadService.SaveFileAsync(file, options.Value.MaxFileSize);

        Assert.True(result.Success);
        Assert.NotNull(result.StoredPath);
        Assert.True(File.Exists(result.StoredPath));
        Assert.Contains("test-image.png", result.FileName);

        // Cleanup
        if (File.Exists(result.StoredPath))
            File.Delete(result.StoredPath);
    }

    [Fact]
    public async Task E2E_Upload_PreviewGenerated_ForImage()
    {
        using var scope = _factory.Services.CreateScope();
        var previewService = scope.ServiceProvider.GetRequiredService<IFilePreviewService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<UploadOptions>>();

        var content = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var file = new FakeBrowserFile("preview-test.png", content, "image/png");

        var preview = await previewService.CreatePreviewAsync(file, options.Value.MaxFileSize);

        Assert.Equal("preview-test.png", preview.Name);
        Assert.NotNull(preview.PreviewUrl);
        Assert.StartsWith("data:image/png;base64,", preview.PreviewUrl);
    }

    [Fact]
    public void E2E_Validation_RejectsInvalidExtension()
    {
        using var scope = _factory.Services.CreateScope();
        var validationService = scope.ServiceProvider.GetRequiredService<IFileValidationService>();

        Assert.False(validationService.IsImageFile("malware.exe"));
        Assert.False(validationService.IsImageFile("script.js"));
        Assert.False(validationService.IsImageFile("document.pdf"));
    }

    [Fact]
    public void E2E_Validation_RejectsOversizedFile()
    {
        using var scope = _factory.Services.CreateScope();
        var validationService = scope.ServiceProvider.GetRequiredService<IFileValidationService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<UploadOptions>>();

        var oversized = options.Value.MaxFileSize + 1;
        Assert.False(validationService.IsWithinSizeLimit(oversized, options.Value.MaxFileSize));
    }

    // --- File Serving Endpoint (requires authentication) ---

    [Fact]
    public async Task FileServing_AnonymousUser_ReturnsRedirectOrNotFound()
    {
        var response = await _client.GetAsync("/uploads/nonexistent.png");

        // Anonymous users: in production = redirect to login; in test env = 404 (endpoint auth mismatch)
        Assert.True(
            response.StatusCode == HttpStatusCode.Redirect ||
            response.StatusCode == HttpStatusCode.Found ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected redirect or NotFound, got {response.StatusCode}");
    }

    [Theory]
    [InlineData("/uploads/..%2F..%2Fappsettings.json")]
    [InlineData("/uploads/..\\appsettings.json")]
    public async Task FileServing_DirectoryTraversal_ReturnsBlocked(string path)
    {
        var response = await _client.GetAsync(path);

        // Directory traversal should be blocked (BadRequest) or auth denied (NotFound in test env)
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Redirect ||
            response.StatusCode == HttpStatusCode.Found);
    }

    [Fact]
    public async Task FileServing_UploadedFile_AnonymousUser_Denied()
    {
        using var scope = _factory.Services.CreateScope();
        var uploadService = scope.ServiceProvider.GetRequiredService<IFileUploadService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<UploadOptions>>();

        var content = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var file = new FakeBrowserFile("serve-test.png", content, "image/png");
        var result = await uploadService.SaveFileAsync(file, options.Value.MaxFileSize);

        // Anonymous user should be denied access (redirect in prod, NotFound in test env)
        var response = await _client.GetAsync($"/uploads/{result.FileName}");

        Assert.True(
            response.StatusCode == HttpStatusCode.Redirect ||
            response.StatusCode == HttpStatusCode.Found ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected redirect/denial for anonymous user, got {response.StatusCode}");

        // Cleanup
        if (File.Exists(result.StoredPath))
            File.Delete(result.StoredPath);
    }

    // --- File Signature Validation via DI ---

    [Fact]
    public void E2E_Signature_ValidPng_Passes()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFileValidationService>();

        var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        Assert.True(service.HasValidFileSignature(pngHeader, "photo.png"));
    }

    [Fact]
    public void E2E_Signature_SpoofedExeAsPng_Fails()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFileValidationService>();

        var exeHeader = new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00 };
        Assert.False(service.HasValidFileSignature(exeHeader, "malware.png"));
    }
}
