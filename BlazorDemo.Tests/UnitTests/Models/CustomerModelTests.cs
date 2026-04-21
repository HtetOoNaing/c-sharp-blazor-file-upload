using BlazorDemo.Models;

namespace BlazorDemo.Tests.UnitTests.Models;

public class CustomerModelTests
{
    [Fact]
    public void CustomerModel_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var model = new CustomerModel();

        // Assert
        Assert.Equal(0, model.Id);
        Assert.Null(model.UserName);
        Assert.Null(model.FirstName);
        Assert.Null(model.LastName);
        Assert.NotNull(model.FileNames);
        Assert.Empty(model.FileNames);
    }

    [Fact]
    public void CustomerModel_SetProperties_Works()
    {
        // Arrange
        var model = new CustomerModel
        {
            Id = 1,
            UserName = "testuser",
            FirstName = "John",
            LastName = "Doe",
            FileNames = ["file1.txt", "file2.txt"]
        };

        // Assert
        Assert.Equal(1, model.Id);
        Assert.Equal("testuser", model.UserName);
        Assert.Equal("John", model.FirstName);
        Assert.Equal("Doe", model.LastName);
        Assert.Equal(2, model.FileNames.Count);
        Assert.Contains("file1.txt", model.FileNames);
        Assert.Contains("file2.txt", model.FileNames);
    }

    [Fact]
    public void CustomerModel_FileNames_CanBeEmpty()
    {
        // Arrange
        var model = new CustomerModel
        {
            FileNames = []
        };

        // Assert
        Assert.NotNull(model.FileNames);
        Assert.Empty(model.FileNames);
    }

    [Fact]
    public void CustomerModel_NullableProperties_AcceptNull()
    {
        // Arrange
        var model = new CustomerModel
        {
            UserName = null,
            FirstName = null,
            LastName = null
        };

        // Assert
        Assert.Null(model.UserName);
        Assert.Null(model.FirstName);
        Assert.Null(model.LastName);
    }
}
