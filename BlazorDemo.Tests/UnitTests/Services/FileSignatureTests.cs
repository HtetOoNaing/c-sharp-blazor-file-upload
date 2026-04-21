using BlazorDemo.Models;
using BlazorDemo.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Tests.UnitTests.Services;

public class FileSignatureTests
{
    private readonly FileValidationService _service;

    public FileSignatureTests()
    {
        var options = Options.Create(new UploadOptions());
        _service = new FileValidationService(options, NullLogger<FileValidationService>.Instance);
    }

    [Fact]
    public void Png_ValidSignature_ReturnsTrue()
    {
        var header = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        Assert.True(_service.HasValidFileSignature(header, "image.png"));
    }

    [Fact]
    public void Jpeg_ValidSignature_ReturnsTrue()
    {
        var header = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 };
        Assert.True(_service.HasValidFileSignature(header, "photo.jpg"));
        Assert.True(_service.HasValidFileSignature(header, "photo.jpeg"));
    }

    [Fact]
    public void Gif87a_ValidSignature_ReturnsTrue()
    {
        var header = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61, 0x00, 0x00 };
        Assert.True(_service.HasValidFileSignature(header, "anim.gif"));
    }

    [Fact]
    public void Gif89a_ValidSignature_ReturnsTrue()
    {
        var header = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x00, 0x00 };
        Assert.True(_service.HasValidFileSignature(header, "anim.gif"));
    }

    [Fact]
    public void Bmp_ValidSignature_ReturnsTrue()
    {
        var header = new byte[] { 0x42, 0x4D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        Assert.True(_service.HasValidFileSignature(header, "bitmap.bmp"));
    }

    [Fact]
    public void Webp_ValidSignature_ReturnsTrue()
    {
        var header = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00 };
        Assert.True(_service.HasValidFileSignature(header, "image.webp"));
    }

    // --- Spoofed files ---

    [Fact]
    public void PngExtension_WithJpegBytes_ReturnsFalse()
    {
        var jpegHeader = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 };
        Assert.False(_service.HasValidFileSignature(jpegHeader, "fake.png"));
    }

    [Fact]
    public void JpgExtension_WithPngBytes_ReturnsFalse()
    {
        var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        Assert.False(_service.HasValidFileSignature(pngHeader, "fake.jpg"));
    }

    [Fact]
    public void PngExtension_WithExeBytes_ReturnsFalse()
    {
        var exeHeader = new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00 };
        Assert.False(_service.HasValidFileSignature(exeHeader, "malware.png"));
    }

    [Fact]
    public void PngExtension_WithZeroBytes_ReturnsFalse()
    {
        var zeros = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        Assert.False(_service.HasValidFileSignature(zeros, "empty.png"));
    }

    // --- Edge cases ---

    [Fact]
    public void UnknownExtension_ReturnsFalse()
    {
        var header = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        Assert.False(_service.HasValidFileSignature(header, "file.xyz"));
    }

    [Fact]
    public void NoExtension_ReturnsFalse()
    {
        var header = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        Assert.False(_service.HasValidFileSignature(header, "noext"));
    }

    [Fact]
    public void NullHeaderBytes_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => _service.HasValidFileSignature(null!, "file.png"));
    }

    [Fact]
    public void NullFileName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => _service.HasValidFileSignature([0x89], null!));
    }

    [Fact]
    public void EmptyFileName_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => _service.HasValidFileSignature([0x89], ""));
    }

    [Fact]
    public void EmptyHeaderBytes_ReturnsFalse()
    {
        Assert.False(_service.HasValidFileSignature([], "image.png"));
    }

    [Fact]
    public void TooShortHeader_ReturnsFalse()
    {
        Assert.False(_service.HasValidFileSignature([0x89], "image.png"));
    }
}
