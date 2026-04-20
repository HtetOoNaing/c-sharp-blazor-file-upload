namespace BlazorDemo.Tests.UnitTests;

public class FileValidationTests
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    private const long MaxFileSize = 1024 * 1024 * 3; // 3 MB
    private const int MaxAllowedFiles = 3;

    private bool IsImageFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        return AllowedImageExtensions.Contains(Path.GetExtension(fileName)?.ToLower());
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
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
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsImageFile_ValidatesExtensions(string? fileName, bool expected)
    {
        // Act
        var result = IsImageFile(fileName!);

        // Assert
        Assert.Equal(expected, result);
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
        var result = FormatFileSize(bytes);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatFileSize_DecimalPlaces_AreCorrect()
    {
        // Act
        var result = FormatFileSize(1536); // 1.5 KB

        // Assert
        Assert.Equal("1.5 KB", result);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1024, true)]
    [InlineData(1024 * 1024 * 3, true)]
    [InlineData(1024 * 1024 * 3 + 1, false)]
    [InlineData(1024 * 1024 * 10, false)]
    public void FileSize_WithinLimit(long size, bool withinLimit)
    {
        // Act
        var isWithinLimit = size <= MaxFileSize;

        // Assert
        Assert.Equal(withinLimit, isWithinLimit);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(5, false)]
    public void FileCount_WithinLimit(int count, bool withinLimit)
    {
        // Act
        var isWithinLimit = count <= MaxAllowedFiles;

        // Assert
        Assert.Equal(withinLimit, isWithinLimit);
    }

    [Fact]
    public void Filename_Generation_WithGuid_IsUnique()
    {
        // Arrange
        var originalName = "photo.jpg";
        var fileNames = new List<string>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var newName = $"{Guid.NewGuid()}_{originalName}";
            fileNames.Add(newName);
        }

        // Assert - all names should be unique
        Assert.Equal(100, fileNames.Distinct().Count());
        Assert.All(fileNames, name => Assert.EndsWith("_photo.jpg", name));
        Assert.All(fileNames, name => Assert.True(Guid.TryParse(name.Split('_')[0], out _)));
    }
}
