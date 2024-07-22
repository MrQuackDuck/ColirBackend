using System.IO.Abstractions;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DAL.Repositories.Related;

public class RoomFileManager : IRoomFileManager
{
    private IFileSystem _fileSystem;
    
    public RoomFileManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public Task<FileStream> GetFileAsync(string path)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetFreeStorageSizeAsync(string roomGuid)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetFilesSizeAsync(string roomGuid)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(string roomGuid, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(string roomGuid, string path)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllFilesAsync(string roomGuid)
    {
        throw new NotImplementedException();
    }
}