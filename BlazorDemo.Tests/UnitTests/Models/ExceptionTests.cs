using BlazorDemo.Exceptions;

namespace BlazorDemo.Tests.UnitTests.Models;

public class ExceptionTests
{
    // --- FileValidationException ---

    [Fact]
    public void FileValidationException_MessageOnly_SetsMessage()
    {
        var ex = new FileValidationException("File too large");

        Assert.Equal("File too large", ex.Message);
        Assert.Null(ex.FileName);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void FileValidationException_WithFileName_SetsBothProperties()
    {
        var ex = new FileValidationException("File too large", "photo.jpg");

        Assert.Equal("File too large", ex.Message);
        Assert.Equal("photo.jpg", ex.FileName);
    }

    [Fact]
    public void FileValidationException_WithInnerException_PreservesChain()
    {
        var inner = new InvalidOperationException("inner detail");
        var ex = new FileValidationException("File too large", "photo.jpg", inner);

        Assert.Equal("File too large", ex.Message);
        Assert.Equal("photo.jpg", ex.FileName);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void FileValidationException_IsException()
    {
        var ex = new FileValidationException("test");
        Assert.IsAssignableFrom<Exception>(ex);
    }

    // --- FileUploadException ---

    [Fact]
    public void FileUploadException_MessageOnly_SetsMessage()
    {
        var ex = new FileUploadException("Disk full");

        Assert.Equal("Disk full", ex.Message);
        Assert.Null(ex.FileName);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void FileUploadException_WithFileName_SetsBothProperties()
    {
        var ex = new FileUploadException("Disk full", "data.png");

        Assert.Equal("Disk full", ex.Message);
        Assert.Equal("data.png", ex.FileName);
    }

    [Fact]
    public void FileUploadException_WithInnerException_PreservesChain()
    {
        var inner = new IOException("disk error");
        var ex = new FileUploadException("Failed to save", "data.png", inner);

        Assert.Equal("Failed to save", ex.Message);
        Assert.Equal("data.png", ex.FileName);
        Assert.IsType<IOException>(ex.InnerException);
    }

    [Fact]
    public void FileUploadException_IsException()
    {
        var ex = new FileUploadException("test");
        Assert.IsAssignableFrom<Exception>(ex);
    }

    // --- Catch pattern tests ---

    [Fact]
    public void FileUploadException_CanBeCaughtByType()
    {
        void Act() => throw new FileUploadException("fail", "test.jpg");

        var ex = Assert.Throws<FileUploadException>(Act);
        Assert.Equal("test.jpg", ex.FileName);
    }

    [Fact]
    public void FileValidationException_CanBeCaughtByType()
    {
        void Act() => throw new FileValidationException("invalid", "bad.exe");

        var ex = Assert.Throws<FileValidationException>(Act);
        Assert.Equal("bad.exe", ex.FileName);
    }
}
