using System.IO.Abstractions.TestingHelpers;
using Colir.DAL.Tests.Interfaces;
using DAL.Repositories.Related;

namespace Colir.DAL.Tests.Tests;

public class RoomFileManagerTests : IRoomFileManagerTests
{
    private RoomFileManager _roomFileManager;
    private MockFileSystem _mockFileSystem;

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
        _mockFileSystem.AddFile("./RoomsFiles/00000000-0000-0000-0000-000000000000/File-1.txt", mockFile);
        
        // Act
        var file = await _roomFileManager.GetFileAsync("./RoomsFiles/00000000-0000-0000-0000-000000000000/File-1.txt");
        
        // Assert
        Assert.That(file.Name == "File-1.txt");
    }

    [Test]
    public async Task GetFreeStorageSizeAsync_ReturnsFreeStorageSize()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetFilesSizeAsync_ReturnsFilesTotalSize()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task UploadFileAsync_UploadsFile()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteFileAsync_DeletesTheFileRelatedToRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAllFilesAsync_DeletesAllFilesRelatedToRoom()
    {
        throw new NotImplementedException();
    }
}