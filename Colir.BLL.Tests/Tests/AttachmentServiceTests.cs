using Colir.BLL.RequestModels.Attachment;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class AttachmentServiceTests : IAttachmentServiceTests
{
    private ColirDbContext _dbContext;
    private AttachmentService _attachmentService;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object);
        _attachmentService = new AttachmentService(unitOfWork);

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
        var room = _dbContext.Rooms.First(r => r.Id == 2);
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
    public async Task UploadAttachmentAsync_ThrowsNotEnoughPermissionsExcpetion_WhenIssuerNotInRoom()
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
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }
}