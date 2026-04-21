using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Tests.TestHelpers;

public class FakeBrowserFile : IBrowserFile
{
    private readonly byte[] _content;

    public FakeBrowserFile(string name, byte[]? content = null, string contentType = "image/png")
    {
        Name = name;
        ContentType = contentType;
        _content = content ?? [0x89, 0x50, 0x4E, 0x47]; // PNG header bytes
        Size = _content.Length;
        LastModified = DateTimeOffset.UtcNow;
    }

    public string Name { get; }
    public DateTimeOffset LastModified { get; }
    public long Size { get; }
    public string ContentType { get; }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        if (Size > maxAllowedSize)
            throw new IOException($"Supplied file with size {Size} bytes exceeds the maximum of {maxAllowedSize} bytes.");

        return new MemoryStream(_content);
    }
}
