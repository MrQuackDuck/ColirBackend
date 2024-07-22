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

    public void CopyTo(Stream target) { }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken()) { }

    public string ContentType { get; } = default!;
    public string ContentDisposition { get; } = default!;
    public IHeaderDictionary Headers { get; } = default!;
    public long Length { get; }
    public string Name { get; } = default!;
    public string FileName { get; }
    
    public FakeFormFile(string fileName, long sizeInBytes)
    {
        FileName = fileName;
        Length = sizeInBytes;
    }
}