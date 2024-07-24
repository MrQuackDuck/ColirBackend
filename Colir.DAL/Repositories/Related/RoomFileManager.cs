using System.IO.Abstractions;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories.Related;

public class RoomFileManager : IRoomFileManager
{
    private IFileSystem _fileSystem;
    private IConfiguration _config;
    private string filesFolderName = "RoomFiles";
    
    public RoomFileManager(IFileSystem fileSystem, IConfiguration config)
    {
        _fileSystem = fileSystem;
        _config = config;

        // Create the directory where rooms files will be stored
        _fileSystem.Directory.CreateDirectory(filesFolderName);
    }
    
    /// <summary>
    /// Gets a file by path
    /// </summary>
    /// <param name="path">A path to a file</param>
    public FileSystemStream GetFile(string path)
    {
        return _fileSystem.FileStream.New(path, FileMode.Open);
    }

    /// <summary>
    /// Gets free storage left for the room
    /// </summary>
    /// <param name="roomGuid">Guid of the room</param>
    public long GetFreeStorageSize(string roomGuid)
    {
        var maxStorageCapacity = int.Parse(_config["MaxRoomStorageCapacityInBytes"]!);
        
        return maxStorageCapacity - GetFilesSize(roomGuid);
    }

    /// <summary>
    /// Gets total size of files
    /// </summary>
    /// <param name="roomGuid">Guid of the room</param>
    public long GetFilesSize(string roomGuid)
    {
        string pathToDirectory = $"./{filesFolderName}/{roomGuid}/";
        var files = _fileSystem.DirectoryInfo.New(pathToDirectory).GetFiles();

        long directorySize = 0;
        foreach (var file in files)
        {
            directorySize += file.Length;
        }

        return directorySize;
    }

    /// <summary>
    /// Uploads a file and returns a path
    /// </summary>
    /// <param name="roomGuid">Guid of the room to upload into</param>
    /// <param name="file">A file to upload</param>
    public async Task<string> UploadFileAsync(string roomGuid, IFormFile file)
    {
        // Generating a random name for the file
        var fileName = Guid.NewGuid();
        
        // Create the directory if not exists
        _fileSystem.Directory.CreateDirectory($"./{filesFolderName}/{roomGuid}");
        
        var path = $"./{filesFolderName}/{roomGuid}/{fileName}";
        using (FileSystemStream fs = _fileSystem.FileStream.New(path, FileMode.CreateNew))
        {
            await file.CopyToAsync(fs);
        }

        return path;
    }

    /// <summary>
    /// Deletes the file
    /// </summary>
    /// <param name="path">Path of the file</param>
    public void DeleteFile(string path)
    {
        _fileSystem.File.Delete(path);
    }

    /// <summary>
    /// Deletes all files in the room
    /// </summary>
    /// <param name="roomGuid">Guid of the room</param>
    public void DeleteAllFiles(string roomGuid)
    {
        var filesPaths = _fileSystem.Directory.GetFiles($"./{filesFolderName}/{roomGuid}/");

        foreach (var path in filesPaths)
        {
            _fileSystem.File.Delete(path);
        }
    }
}