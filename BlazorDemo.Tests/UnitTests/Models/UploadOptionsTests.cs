using BlazorDemo.Models;

namespace BlazorDemo.Tests.UnitTests.Models;

public class UploadOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new UploadOptions();

        Assert.Equal(1024 * 1024 * 3, options.MaxFileSize);
        Assert.Equal(3, options.MaxFileCount);
        Assert.Equal("uploads", options.UploadFolder);
        Assert.Equal("UploadOptions", UploadOptions.SectionName);
    }

    [Fact]
    public void DefaultExtensions_ContainAllExpected()
    {
        var options = new UploadOptions();

        Assert.Contains(".jpg", options.AllowedExtensions);
        Assert.Contains(".jpeg", options.AllowedExtensions);
        Assert.Contains(".png", options.AllowedExtensions);
        Assert.Contains(".gif", options.AllowedExtensions);
        Assert.Contains(".bmp", options.AllowedExtensions);
        Assert.Contains(".webp", options.AllowedExtensions);
        Assert.Equal(6, options.AllowedExtensions.Length);
    }

    [Fact]
    public void CustomValues_CanBeSet()
    {
        var options = new UploadOptions
        {
            MaxFileSize = 1024 * 1024 * 10,
            MaxFileCount = 5,
            UploadFolder = "custom-uploads",
            AllowedExtensions = [".svg", ".tiff"]
        };

        Assert.Equal(1024 * 1024 * 10, options.MaxFileSize);
        Assert.Equal(5, options.MaxFileCount);
        Assert.Equal("custom-uploads", options.UploadFolder);
        Assert.Equal(2, options.AllowedExtensions.Length);
        Assert.Contains(".svg", options.AllowedExtensions);
    }

    [Fact]
    public void ValidationService_RespectsCustomExtensions()
    {
        var options = new UploadOptions
        {
            AllowedExtensions = [".svg", ".tiff"]
        };

        var service = new BlazorDemo.Services.FileValidationService(
            Microsoft.Extensions.Options.Options.Create(options),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<BlazorDemo.Services.FileValidationService>.Instance);

        Assert.True(service.IsImageFile("photo.svg"));
        Assert.True(service.IsImageFile("photo.tiff"));
        Assert.False(service.IsImageFile("photo.jpg"));
        Assert.False(service.IsImageFile("photo.png"));
    }
}
