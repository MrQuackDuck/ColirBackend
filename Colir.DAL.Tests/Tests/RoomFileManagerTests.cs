using System.IO.Abstractions.TestingHelpers;
using Colir.BLL.Tests.Fakes;
using Colir.DAL.Tests.Interfaces;
using DAL.Repositories.Related;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.DAL.Tests.Tests;

public class RoomFileManagerTests : IRoomFileManagerTests
{
    private RoomFileManager _roomFileManager;
    private MockFileSystem _mockFileSystem;
    
    private readonly string _folderName = "RoomFiles";

    [SetUp]
    public void SetUp()
    {
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(config => config["AppSettings:MaxRoomStorageCapacityInBytes"]).Returns("100000000");
        configMock.Setup(config => config["AppSettings:RoomFilesFolderName"]).Returns("RoomFiles");
        
        _roomFileManager = new RoomFileManager(_mockFileSystem, configMock.Object);
    }
    
    [Test]
    public async Task GetFile_ReturnsFileAsync()
    {
        // Assert
        var mockData = new MockFileData("Random Content");
        var path = $"./{_folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockData);
        
        // Act
        var file = _roomFileManager.GetFile(path);
        
        // Assert
        Assert.That(file.Name.Contains("File-1.txt"));
    }

    [Test]
    public async Task GetFreeStorageSize_ReturnsFreeStorageSize()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var path = $"./{_folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockFile);
        var fileSize = _mockFileSystem.FileInfo.New(path).Length;
        var expectedFreeSize = 100_000_000 - fileSize;
        
        // Act
        var result = _roomFileManager.GetFreeStorageSize("00000000-0000-0000-0000-000000000000");

        // Assert
        Assert.That(result == expectedFreeSize);
    }

    [Test]
    public async Task GetFilesSize_ReturnsFilesTotalSize()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var path = $"./{_folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockFile);
        var expectedFileSize = _mockFileSystem.FileInfo.New(path).Length;
        
        // Act
        var result = _roomFileManager.GetOccupiedStorageSize("00000000-0000-0000-0000-000000000000");

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
        var resultPath = await _roomFileManager.UploadFileAsync("00000000-0000-0000-0000-000000000000", new FakeFormFile(fileName, fileSize, _mockFileSystem));

        // Assert
        _mockFileSystem.File.Exists(resultPath);
    }

    [Test]
    public async Task DeleteFile_DeletesTheFileRelatedToRoom()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var path = $"./{_folderName}/00000000-0000-0000-0000-000000000000/File-1.txt";
        _mockFileSystem.AddFile(path, mockFile);
        
        // Act
        _roomFileManager.DeleteFile(path);

        // Assert
        Assert.That(_mockFileSystem.FileExists(path) == false);
    }

    [Test]
    public async Task DeleteAllFiles_DeletesAllFilesRelatedToRoom()
    {
        // Arrange
        var mockFile = new MockFileData("Random Content");
        var roomGuid = "00000000-0000-0000-0000-000000000000";
        List<string> filePaths = new List<string>()
        {
            $"./{_folderName}/{roomGuid}/File-1.txt",
            $"./{_folderName}/{roomGuid}/File-2.txt",
            $"./{_folderName}/{roomGuid}/File-3.txt"
        };

        foreach (var path in filePaths)
        {
            _mockFileSystem.AddFile(path, mockFile);   
        }
        
        // Act
        _roomFileManager.DeleteAllFiles(roomGuid);

        // Assert
        foreach (var path in filePaths)
        {
            Assert.That(!_mockFileSystem.FileExists(path));
        }
    }
}