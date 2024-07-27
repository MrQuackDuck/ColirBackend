using System.IO.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DAL.Interfaces;

public interface IRoomFileManager
{
    FileSystemStream GetFile(string path);
    long GetFreeStorageSize(string roomGuid);
    long GetOccupiedStorageSize(string roomGuid);
    Task<string> UploadFileAsync(string roomGuid, IFormFile file);
    void DeleteFile(string path);
    void DeleteAllFiles(string roomGuid);
}