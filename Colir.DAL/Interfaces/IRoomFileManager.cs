using System.IO.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DAL.Interfaces;

public interface IRoomFileManager
{
    Task<FileStream> GetFileAsync(string path);
    Task<long> GetFreeStorageSizeAsync(string roomGuid);
    Task<long> GetFilesSizeAsync(string roomGuid);
    Task<string> UploadFileAsync(string roomGuid, IFormFile file);
    Task DeleteFileAsync(string roomGuid, string path);
    Task DeleteAllFilesAsync(string roomGuid);
}