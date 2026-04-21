using BlazorDemo.Services;
using Microsoft.AspNetCore.DataProtection;

namespace BlazorDemo.Tests.UnitTests.Services;

public class FileMetadataProtectionServiceTests
{
    private readonly FileMetadataProtectionService _service;

    public FileMetadataProtectionServiceTests()
    {
        // Use a fake provider for testing without external dependencies
        var fakeProvider = new FakeDataProtectionProvider();
        _service = new FileMetadataProtectionService(fakeProvider);
    }

    /// <summary>
    /// Simple fake that reverses strings to simulate encryption/decryption.
    /// </summary>
    private class FakeDataProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector CreateProtector(string purpose)
        {
            return new FakeDataProtector();
        }

        private class FakeDataProtector : IDataProtector
        {
            public byte[] Protect(byte[] plaintext) => plaintext.Reverse().ToArray();

            public byte[] Unprotect(byte[] protectedData) => protectedData.Reverse().ToArray();

            public string Protect(string plaintext)
            {
                if (string.IsNullOrEmpty(plaintext)) return plaintext;
                // Simulate real protector: reverse then base64 encode
                var reversed = plaintext.Reverse().ToArray();
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reversed));
            }

            public string Unprotect(string protectedData)
            {
                if (string.IsNullOrEmpty(protectedData)) return protectedData;
                // Simulate real protector: base64 decode then reverse
                var bytes = Convert.FromBase64String(protectedData);
                var str = System.Text.Encoding.UTF8.GetString(bytes);
                return new string(str.Reverse().ToArray());
            }

            public IDataProtector CreateProtector(string purpose) => this;
        }
    }

    [Fact]
    public void ProtectFileName_TransformsValue()
    {
        var original = "sensitive.pdf";

        var result = _service.ProtectFileName(original);

        Assert.NotEqual(original, result);
        // Result should be different and non-empty
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void ProtectFileName_EmptyString_ReturnsEmpty()
    {
        var result = _service.ProtectFileName("");

        Assert.Equal("", result);
    }

    [Fact]
    public void UnprotectFileName_RestoresOriginal()
    {
        var protectedName = _service.ProtectFileName("document.pdf");

        var result = _service.UnprotectFileName(protectedName);

        Assert.Equal("document.pdf", result);
    }

    [Fact]
    public void UnprotectFileName_InvalidBase64_ReturnsNull()
    {
        // Invalid base64 data should throw and return null
        var result = _service.UnprotectFileName("!!!invalid!!!");
        // Our fake throws on invalid base64, service catches and returns null
        Assert.Null(result);
    }

    [Fact]
    public void UnprotectFileName_EmptyString_ReturnsEmpty()
    {
        var result = _service.UnprotectFileName("");

        Assert.Equal("", result);
    }

    [Fact]
    public void ProtectMetadata_TransformsValue()
    {
        var metadata = "user:123";

        var result = _service.ProtectMetadata(metadata);

        Assert.NotEqual(metadata, result);
        // Result should be different and non-empty
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void UnprotectMetadata_RestoresOriginal()
    {
        var protectedData = _service.ProtectMetadata("email@test.com");

        var result = _service.UnprotectMetadata(protectedData);

        Assert.Equal("email@test.com", result);
    }

    [Fact]
    public void ProtectAndUnprotect_RoundTrip_PreservesData()
    {
        var original = "confidential.pdf";

        var protectedValue = _service.ProtectFileName(original);
        var unprotectedValue = _service.UnprotectFileName(protectedValue);

        Assert.Equal(original, unprotectedValue);
    }
}
