using System.IO.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Colir.BLL.Tests.Fakes;

/// <summary>
/// Fake IFormFile implementation for Unit Tests
/// </summary>
public class FakeFormFile : IFormFile
{
    public Stream OpenReadStream()
    {
        throw new NotImplementedException();
    }

    public void CopyTo(Stream target)
    {
        _fileSystem.File.Create(FileName);
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken())
    {
        _fileSystem.File.Create(FileName);
    }

    public string ContentType { get; } = default!;
    public string ContentDisposition { get; } = default!;
    public IHeaderDictionary Headers { get; } = default!;
    public long Length { get; }
    public string Name { get; } = default!;
    public string FileName { get; }
    private readonly IFileSystem _fileSystem;

    public FakeFormFile(string fileName, long sizeInBytes, IFileSystem fileSystem)
    {
        FileName = fileName;
        Length = sizeInBytes;
        _fileSystem = fileSystem;
    }

    public FakeFormFile(string fileName, long sizeInBytes)
    {
        FileName = fileName;
        Length = sizeInBytes;
        _fileSystem = default!;
    }
}