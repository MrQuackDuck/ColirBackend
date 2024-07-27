using Colir.BLL.RequestModels.Attachment;
using Colir.BLL.Services;
using Colir.BLL.Tests.Fakes;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class AttachmentServiceTests : IAttachmentServiceTests
{
    private ColirDbContext _dbContext;
    private AttachmentService _attachmentService;
    private IFormFile _fileToUpload = new FakeFormFile("UnitTest.txt", 1000);

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        var roomFileMangerMock = new Mock<IRoomFileManager>();
        
        roomFileMangerMock
            .Setup(fileManager => fileManager.GetFreeStorageSize("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"))
            .Returns(100_000_000);

        roomFileMangerMock
            .Setup(fileManager => fileManager.UploadFileAsync("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", _fileToUpload))
            .ReturnsAsync("./RoomFiles/cbaa8673-ea8b-43f8-b4cc-b8b0797b620e/UnitTest.txt");
        
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object, roomFileMangerMock.Object);
        var mapper = AutomapperProfile.InitializeAutoMapper().CreateMapper();
        _attachmentService = new AttachmentService(unitOfWork, mapper);

        // Add entities
        UnitTestHelper.SeedData(_dbContext);
    }

    [TearDown]
    public void CleanUp()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task UploadAttachmentAsync_UploadsAttachment()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUploadAttachment
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            File = _fileToUpload
        };

        // Act
        var result = await _attachmentService.UploadAttachmentAsync(request);

        // Assert
        Assert.That(result.Filename == _fileToUpload.FileName);
        Assert.That(result.SizeInBytes == 1000);
    }

    [Test]
    public async Task UploadAttachmentAsync_ThrowsArgumentException_WhenNoFreeStorageLeft()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUploadAttachment
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            File = new FakeFormFile("BigFile.exe", 200_000_000)
        };
        
        // Act
        AsyncTestDelegate act = async () => await _attachmentService.UploadAttachmentAsync(request);
        
        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task UploadAttachmentAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 2);
        var request = new RequestToUploadAttachment
        {
            IssuerId = 1,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _attachmentService.UploadAttachmentAsync(request);
        
        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task UploadAttachmentAsync_ThrowsRoomNotFoundException_WhenRoomNotFound()
    {
        // Arrange
        var request = new RequestToUploadAttachment
        {
            IssuerId = 1,
            RoomGuid = "404"
        };
        
        // Act
        AsyncTestDelegate act = async () => await _attachmentService.UploadAttachmentAsync(request);
        
        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task UploadAttachmentAsync_ThrowsUserNotFoundException_WhenIssuerNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUploadAttachment
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _attachmentService.UploadAttachmentAsync(request);
        
        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task UploadAttachmentAsync_ThrowsIssuerNotInRoomExceptionException_WhenIssuerNotInRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUploadAttachment
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _attachmentService.UploadAttachmentAsync(request);
        
        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }
}