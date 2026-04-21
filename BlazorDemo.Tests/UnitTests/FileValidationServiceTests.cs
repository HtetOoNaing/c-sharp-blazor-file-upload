using BlazorDemo.Models;
using BlazorDemo.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Tests.UnitTests;

public class FileValidationServiceTests
{
    private readonly IFileValidationService _service;

    public FileValidationServiceTests()
    {
        // Create IOptions<UploadOptions> with default values (same as appsettings.json)
        var options = Options.Create(new UploadOptions());
        var logger = NullLogger<FileValidationService>.Instance;
        _service = new FileValidationService(options, logger);
    }

    [Theory]
    [InlineData("photo.jpg", true)]
    [InlineData("photo.jpeg", true)]
    [InlineData("photo.png", true)]
    [InlineData("photo.gif", true)]
    [InlineData("photo.bmp", true)]
    [InlineData("photo.webp", true)]
    [InlineData("photo.JPG", true)]
    [InlineData("photo.PNG", true)]
    [InlineData("document.pdf", false)]
    [InlineData("script.exe", false)]
    [InlineData("archive.zip", false)]
    public void IsImageFile_ValidatesExtensions(string fileName, bool expected)
    {
        // Act
        var result = _service.IsImageFile(fileName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsImageFile_NullOrEmpty_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_service.IsImageFile(null));
        Assert.False(_service.IsImageFile(""));
        Assert.False(_service.IsImageFile("   "));
    }

    [Theory]
    [InlineData(100, "100 B")]
    [InlineData(1024, "1 KB")]
    [InlineData(1024 * 1024, "1 MB")]
    [InlineData(1024 * 1024 * 3, "3 MB")]
    [InlineData(1024 * 1024 * 1024, "1 GB")]
    public void FormatFileSize_FormatsCorrectly(long bytes, string expected)
    {
        // Act
        var result = _service.FormatFileSize(bytes);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatFileSize_DecimalPlaces_AreCorrect()
    {
        // Act
        var result = _service.FormatFileSize(1536); // 1.5 KB

        // Assert
        Assert.Equal("1.5 KB", result);
    }

    [Fact]
    public void FormatFileSize_Zero_ReturnsZeroBytes()
    {
        // Act
        var result = _service.FormatFileSize(0);

        // Assert
        Assert.Equal("0 B", result);
    }

    [Fact]
    public void FormatFileSize_Negative_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.FormatFileSize(-1));
    }

    [Theory]
    [InlineData(0, 1024, true)]      // 0 bytes within 1KB limit
    [InlineData(1024, 1024, true)]   // Exactly at limit
    [InlineData(1023, 1024, true)]   // Just under limit
    [InlineData(1025, 1024, false)]  // Just over limit
    [InlineData(1024 * 1024 * 3, 1024 * 1024 * 3, true)]  // 3MB at 3MB limit
    public void IsWithinSizeLimit_ChecksCorrectly(long fileSize, long maxSize, bool expected)
    {
        // Act
        var result = _service.IsWithinSizeLimit(fileSize, maxSize);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetAllowedExtensions_ReturnsExpectedList()
    {
        // Act
        var extensions = _service.GetAllowedExtensions();

        // Assert
        Assert.NotNull(extensions);
        Assert.Contains(".jpg", extensions);
        Assert.Contains(".jpeg", extensions);
        Assert.Contains(".png", extensions);
        Assert.Contains(".gif", extensions);
        Assert.Contains(".bmp", extensions);
        Assert.Contains(".webp", extensions);
    }

    [Fact]
    public void GetAllowedExtensions_ReturnsCopy_NotReference()
    {
        // Act
        var extensions1 = _service.GetAllowedExtensions();
        var extensions2 = _service.GetAllowedExtensions();

        // Assert - should be different arrays
        Assert.NotSame(extensions1, extensions2);
        // But with same content
        Assert.Equal(extensions1, extensions2);
    }
}
