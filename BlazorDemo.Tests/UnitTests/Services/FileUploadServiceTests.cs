using BlazorDemo.Exceptions;
using BlazorDemo.Models;
using BlazorDemo.Services;
using BlazorDemo.Tests.TestHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Tests.UnitTests.Services;

public class FileUploadServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FileUploadService _service;

    public FileUploadServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"BlazorTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        var env = new FakeWebHostEnvironment(_tempDir);
        var options = Options.Create(new UploadOptions { UploadFolder = "uploads" });
        var logger = NullLogger<FileUploadService>.Instance;
        var metadataProtection = new FakeMetadataProtectionService();

        _service = new FileUploadService(env, options, logger, metadataProtection);
    }

    /// <summary>
    /// Fake implementation for testing that just returns the original value.
    /// </summary>
    private class FakeMetadataProtectionService : IFileMetadataProtectionService
    {
        public string ProtectFileName(string originalFileName) => $"protected_{originalFileName}";
        public string? UnprotectFileName(string protectedFileName) => protectedFileName?.Replace("protected_", "");
        public string ProtectMetadata(string metadata) => $"protected_{metadata}";
        public string? UnprotectMetadata(string protectedMetadata) => protectedMetadata?.Replace("protected_", "");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    // --- GenerateUniqueFileName ---

    [Fact]
    public void GenerateUniqueFileName_ReturnsGuidPrefixedName()
    {
        var result = _service.GenerateUniqueFileName("photo.jpg");

        Assert.EndsWith("_photo.jpg", result);
        Assert.True(Guid.TryParse(result.Split('_')[0], out _));
    }

    [Fact]
    public void GenerateUniqueFileName_IsUniqueEachCall()
    {
        var name1 = _service.GenerateUniqueFileName("photo.jpg");
        var name2 = _service.GenerateUniqueFileName("photo.jpg");

        Assert.NotEqual(name1, name2);
    }

    [Fact]
    public void GenerateUniqueFileName_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.GenerateUniqueFileName(null!));
    }

    [Fact]
    public void GenerateUniqueFileName_Empty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.GenerateUniqueFileName(""));
    }

    // --- SaveFileAsync ---

    [Fact]
    public async Task SaveFileAsync_ValidFile_SavesToDisk()
    {
        var file = new FakeBrowserFile("test.png");

        var result = await _service.SaveFileAsync(file, 1024 * 1024);

        Assert.True(result.Success);
        Assert.NotNull(result.StoredPath);
        Assert.True(File.Exists(result.StoredPath));
        Assert.EndsWith("_test.png", result.FileName);
    }

    [Fact]
    public async Task SaveFileAsync_WritesCorrectContent()
    {
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var file = new FakeBrowserFile("data.png", content);

        var result = await _service.SaveFileAsync(file, 1024);

        var savedBytes = await File.ReadAllBytesAsync(result.StoredPath!);
        Assert.Equal(content, savedBytes);
    }

    [Fact]
    public async Task SaveFileAsync_NullFile_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.SaveFileAsync(null!, 1024));
    }

    [Fact]
    public async Task SaveFileAsync_FileTooLarge_ThrowsFileUploadException()
    {
        var bigContent = new byte[2048];
        var file = new FakeBrowserFile("big.png", bigContent);

        await Assert.ThrowsAsync<FileUploadException>(
            () => _service.SaveFileAsync(file, 1024));
    }

    [Fact]
    public async Task SaveFileAsync_CreatesUploadDirectory()
    {
        var uploadDir = _service.GetUploadDirectoryPath();
        if (Directory.Exists(uploadDir))
            Directory.Delete(uploadDir, true);

        var file = new FakeBrowserFile("test.png");
        await _service.SaveFileAsync(file, 1024 * 1024);

        Assert.True(Directory.Exists(uploadDir));
    }

    // --- EnsureUploadDirectoryExists ---

    [Fact]
    public void EnsureUploadDirectoryExists_CreatesDirectory()
    {
        var uploadDir = _service.GetUploadDirectoryPath();
        if (Directory.Exists(uploadDir))
            Directory.Delete(uploadDir, true);

        _service.EnsureUploadDirectoryExists();

        Assert.True(Directory.Exists(uploadDir));
    }

    [Fact]
    public void EnsureUploadDirectoryExists_DoesNotThrowIfExists()
    {
        _service.EnsureUploadDirectoryExists();
        _service.EnsureUploadDirectoryExists(); // call twice
    }

    // --- GetUploadDirectoryPath ---

    [Fact]
    public void GetUploadDirectoryPath_ReturnsCorrectPath()
    {
        var result = _service.GetUploadDirectoryPath();

        Assert.Equal(Path.Combine(_tempDir, "uploads"), result);
    }

    // --- Fake IWebHostEnvironment ---

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
}
