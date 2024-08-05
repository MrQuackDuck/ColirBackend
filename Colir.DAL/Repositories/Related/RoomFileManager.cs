using System.IO.Abstractions;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories.Related;

public class RoomFileManager : IRoomFileManager
{
    private IFileSystem _fileSystem;
    private IConfiguration _config;
    private string _filesFolderName;

    public RoomFileManager(IFileSystem fileSystem, IConfiguration config)
    {
        _fileSystem = fileSystem;
        _config = config;
        _filesFolderName = config["AppSettings:RoomFilesFolderName"];
        ArgumentNullException.ThrowIfNull(_filesFolderName);

        // Create the directory where rooms files will be stored
        _fileSystem.Directory.CreateDirectory(_filesFolderName);
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
        var maxStorageCapacity = int.Parse(_config["AppSettings:MaxRoomStorageCapacityInBytes"]!);

        return maxStorageCapacity - GetOccupiedStorageSize(roomGuid);
    }

    /// <summary>
    /// Gets total size of files
    /// </summary>
    /// <param name="roomGuid">Guid of the room</param>
    public long GetOccupiedStorageSize(string roomGuid)
    {
        // Create the directory if not exists
        _fileSystem.Directory.CreateDirectory($"./{_filesFolderName}/{roomGuid}");

        string pathToDirectory = $"./{_filesFolderName}/{roomGuid}/";
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
        _fileSystem.Directory.CreateDirectory($"./{_filesFolderName}/{roomGuid}");

        var path = $"./{_filesFolderName}/{roomGuid}/{fileName}";
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
        var filesPaths = _fileSystem.Directory.GetFiles($"./{_filesFolderName}/{roomGuid}/");

        foreach (var path in filesPaths)
        {
            _fileSystem.File.Delete(path);
        }
    }
}