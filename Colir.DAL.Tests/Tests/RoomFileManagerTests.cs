using System.IO.Abstractions.TestingHelpers;
using Colir.BLL.Tests.Fakes;
using Colir.DAL.Tests.Interfaces;
using DAL.Repositories.Related;

namespace Colir.DAL.Tests.Tests;

public class RoomFileManagerTests : IRoomFileManagerTests
{
    private RoomFileManager _roomFileManager;
    private MockFileSystem _mockFileSystem;
    
    private readonly string folderName = "RoomFiles";

    [SetUp]
    public void SetUp()
    {
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        
        _roomFileManager = new RoomFileManager(_mockFileSystem);
    }
    
    [Test]
    public async Task GetFileAsync_ReturnsFileAsync()
    {
        // Assert
        var mockFile = new MockFileData("Random Content");
        _mockFileSystem.AddFile($"./{folderName}/00000000-0000-0000-0000-000000000000/File-1.txt", mockFile);
        
        // Act
        var file = await _roomFileManager.GetFileAsync("./RoomsFiles/00000000-0000-0000-0000-000000000000/File-1.txt");
        
        // Assert
        Assert.That(file.Name == "File-1.txt");
    }

    [Test]
    public async Task GetFreeStorageSizeAsync_ReturnsFreeStorageSize()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var path = $"./{folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockFile);
        var fileSize = _mockFileSystem.FileInfo.New(path).Length;
        var expectedFreeSize = 100_000_000 - fileSize;
        
        // Act
        var result = await _roomFileManager.GetFreeStorageSizeAsync("00000000-0000-0000-0000-000000000000");

        // Assert
        Assert.That(result == expectedFreeSize);
    }

    [Test]
    public async Task GetFilesSizeAsync_ReturnsFilesTotalSize()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var path = $"./{folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockFile);
        var expectedFileSize = _mockFileSystem.FileInfo.New(path).Length;
        
        // Act
        var result = await _roomFileManager.GetFreeStorageSizeAsync("00000000-0000-0000-0000-000000000000");

        // Assert
        Assert.That(result == expectedFileSize);
    }

    [Test]
    public async Task UploadFileAsync_UploadsFile()
    {
        // Arrange
        var fileName = "File.txt";
        var fileSize = 100;

        // Act
        await _roomFileManager.UploadFileAsync("00000000-0000-0000-0000-000000000000", new FakeFormFile(fileName, fileSize));

        // Assert
        var path = $"./{folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        var file = _mockFileSystem.FileInfo.New(path);
        Assert.That(file.Name == fileName);
        Assert.That(file.Length == fileSize);
    }

    [Test]
    public async Task DeleteFileAsync_DeletesTheFileRelatedToRoom()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var path = $"./{folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockFile);
        
        // Act
        await _roomFileManager.DeleteFileAsync(path);

        // Assert
        Assert.That(_mockFileSystem.FileExists(path) == false);
    }

    [Test]
    public async Task DeleteAllFilesAsync_DeletesAllFilesRelatedToRoom()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        List<string> filePaths = new List<string>()
        {
            $"./{folderName}/00000000-0000-0000-0000-000000000000/File-1.txt",
            $"./{folderName}/00000000-0000-0000-0000-000000000000/File-2.txt",
            $"./{folderName}/00000000-0000-0000-0000-000000000000/File-3.txt"
        };

        foreach (var path in filePaths)
        {
            _mockFileSystem.AddFile(path, mockFile);   
        }
        
        // Act
        await _roomFileManager.DeleteAllFilesAsync("00000000-0000-0000-0000-000000000000");

        // Assert
        foreach (var path in filePaths)
        {
            Assert.That(!_mockFileSystem.FileExists(path));
        }
    }
}