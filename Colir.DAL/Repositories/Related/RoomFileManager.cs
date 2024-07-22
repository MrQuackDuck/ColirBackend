using DAL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DAL.Repositories.Related;

public class RoomFileManager : IRoomFileManager
{
    public Task<IFormFile> GetFileAsync(string path)
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