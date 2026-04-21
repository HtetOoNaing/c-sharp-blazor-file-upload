using BlazorDemo.Models;
using BlazorDemo.Services;
using BlazorDemo.Tests.TestHelpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Tests.UnitTests;

public class FilePreviewServiceTests
{
    private readonly FilePreviewService _service;

    public FilePreviewServiceTests()
    {
        var options = Options.Create(new UploadOptions());
        var validationLogger = NullLogger<FileValidationService>.Instance;
        var validationService = new FileValidationService(options, validationLogger);
        var previewLogger = NullLogger<FilePreviewService>.Instance;

        _service = new FilePreviewService(validationService, previewLogger);
    }

    [Fact]
    public async Task CreatePreviewAsync_ImageFile_GeneratesBase64Preview()
    {
        var content = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var file = new FakeBrowserFile("photo.png", content, "image/png");

        var result = await _service.CreatePreviewAsync(file, 1024 * 1024);

        Assert.Equal("photo.png", result.Name);
        Assert.Equal(content.Length, result.Size);
        Assert.Equal("image/png", result.ContentType);
        Assert.NotNull(result.PreviewUrl);
        Assert.StartsWith("data:image/png;base64,", result.PreviewUrl);
        Assert.Same(file, result.FileData);
    }

    [Fact]
    public async Task CreatePreviewAsync_NonImageFile_NullPreview()
    {
        var file = new FakeBrowserFile("document.pdf", [1, 2, 3], "application/pdf");

        var result = await _service.CreatePreviewAsync(file, 1024 * 1024);

        Assert.Equal("document.pdf", result.Name);
        Assert.Null(result.PreviewUrl);
        Assert.Same(file, result.FileData);
    }

    [Fact]
    public async Task CreatePreviewAsync_NullFile_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreatePreviewAsync(null!, 1024));
    }

    [Fact]
    public async Task CreatePreviewAsync_PreservesFileMetadata()
    {
        var file = new FakeBrowserFile("image.jpg", [0xFF, 0xD8], "image/jpeg");

        var result = await _service.CreatePreviewAsync(file, 1024 * 1024);

        Assert.Equal("image.jpg", result.Name);
        Assert.Equal(2, result.Size);
        Assert.Equal("image/jpeg", result.ContentType);
    }

    [Theory]
    [InlineData("photo.jpg", true)]
    [InlineData("photo.png", true)]
    [InlineData("photo.gif", true)]
    [InlineData("photo.bmp", true)]
    [InlineData("photo.webp", true)]
    [InlineData("file.txt", false)]
    [InlineData("file.pdf", false)]
    public async Task CreatePreviewAsync_PreviewOnlyForImages(string fileName, bool hasPreview)
    {
        var file = new FakeBrowserFile(fileName, [1, 2, 3], "application/octet-stream");

        var result = await _service.CreatePreviewAsync(file, 1024 * 1024);

        if (hasPreview)
            Assert.NotNull(result.PreviewUrl);
        else
            Assert.Null(result.PreviewUrl);
    }

    [Fact]
    public async Task CreatePreviewAsync_EmptyImageFile_StillGeneratesPreview()
    {
        var file = new FakeBrowserFile("empty.png", [], "image/png");

        var result = await _service.CreatePreviewAsync(file, 1024 * 1024);

        Assert.NotNull(result.PreviewUrl);
        Assert.Equal("data:image/png;base64,", result.PreviewUrl);
    }

    [Fact]
    public async Task CreatePreviewAsync_Base64Content_IsCorrect()
    {
        var content = new byte[] { 0x01, 0x02, 0x03 };
        var expectedBase64 = Convert.ToBase64String(content);
        var file = new FakeBrowserFile("img.png", content, "image/png");

        var result = await _service.CreatePreviewAsync(file, 1024 * 1024);

        Assert.Equal($"data:image/png;base64,{expectedBase64}", result.PreviewUrl);
    }
}
