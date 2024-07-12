using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class MessageServiceTests : IMessageServiceTests
{
    private ColirDbContext _dbContext;
    private MessageService _messageService;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        var hexGeneratorMock = new Mock<IHexColorGenerator>();
        hexGeneratorMock.Setup(g => g.GetUniqueHexColor()).Returns("1051b310-ed1a-42ef-9435-fe840ec44009");
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object);
        _messageService = new MessageService(unitOfWork);

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
    public async Task GetLastMessagesAsync_ReturnsLastMessages()
    {
        // Arrange
        var expected = new List<MessageModel>
        {
            _dbContext.Messages.First(m => m.Id == 2).ToMessageModel(),
            _dbContext.Messages.First(m => m.Id == 3).ToMessageModel()
        };

        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastMessages
        {
            IssuerId = 1,
            Count = 2,
            SkipCount = 1,
            RoomGuid = room.Guid
        };
        
        // Act
        var result = await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageModelEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastMessages
        {
            IssuerId = 404,
            Count = 2,
            SkipCount = 1,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToGetLastMessages
        {
            IssuerId = 1,
            Count = 2,
            SkipCount = 1,
            RoomGuid = "404"
        };
        
        // Act
        AsyncTestDelegate act = async () => await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastMessages
        {
            IssuerId = 1,
            Count = -1,
            SkipCount = 1,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastMessages
        {
            IssuerId = 1,
            Count = 2,
            SkipCount = -1,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastMessages
        {
            IssuerId = 3,
            Count = 2,
            SkipCount = 1,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 2);
        var request = new RequestToGetLastMessages
        {
            IssuerId = 3,
            Count = 2,
            SkipCount = 1,
            RoomGuid = room.Guid
        };
        
        // Act
        AsyncTestDelegate act = async () => await _messageService.GetLastMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task SendAsync_SendsMessage()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 2);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            Content = "Hello!",
            AttachmentsIds = new List<long>() { 1 },
            RoomGuid = room.Guid
        };

        // Act
        await _messageService.SendAsync(request);

        // Assert
        var message = _dbContext.Messages
            .Include(nameof(Message.Attachments)).OrderByDescending(m => m.Id).First();
        
        Assert.That(_dbContext.Messages.Count() == 5);
        Assert.That(message.Attachments.Count == 1);
        Assert.That(message.Attachments.First().Id == 1);
    }

    [Test]
    public async Task SendAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = _dbContext.UserStatistics.AsNoTracking().First(s => s.UserId == 1);
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            Content = "Hello"
        };

        // Act
        await _messageService.SendAsync(request);

        // Assert
        var statsAfter = _dbContext.UserStatistics.AsNoTracking().First(s => s.UserId == 1);
        Assert.That(statsAfter.MessagesSent - statsBefore.MessagesSent == 1);
    }

    [Test]
    public async Task SendAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 404,
            RoomGuid = room.Guid,
            Content = "Hello"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsMessageNotFoundException_WhenNotExistingReplyMessageIdProvided()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            Content = "Hello",
            ReplyMessageId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsAttachmentNotFoundException_WhenNotExistingAttachmentIdProvided()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            Content = "Hello",
            AttachmentsIds = new List<long>() { 404 }
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<AttachmentNotFoundException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            RoomGuid = "404",
            Content = "Hello",
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 3,
            RoomGuid = room.Guid,
            Content = "Hello",
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 2);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            Content = "Hello",
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task EditAsync_EditsMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_AddsReaction()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task AddReaction_AddsToStatistics_WhenItsEnabled()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_RemovesReaction()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsReactionNotFoundException_WhenReactionWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfReaction()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        throw new NotImplementedException();
    }
}