﻿using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using Colir.BLL.RequestModels.Room;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Interfaces;
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
        configMock.Setup(c => c["AppSettings:MaxMessageLength"]).Returns("4096");

        var roomFileMangerMock = new Mock<IRoomFileManager>();
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object, roomFileMangerMock.Object);
        var mapper = AutomapperProfile.InitializeAutoMapper().CreateMapper();
        _messageService = new MessageService(unitOfWork, mapper, configMock.Object);

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
            (await _dbContext.Messages
                .AsNoTracking()
                .Include(nameof(Message.Room))
                .Include(nameof(Message.Author))
                .Include(nameof(Message.RepliedTo))
                .Include(nameof(Message.Attachments))
                .Include(nameof(Message.Reactions))
                .FirstAsync(m => m.Id == 3)).ToMessageModel(),
            (await _dbContext.Messages
                .AsNoTracking()
                .Include(nameof(Message.Room))
                .Include(nameof(Message.Author))
                .Include(nameof(Message.RepliedTo))
                .Include(nameof(Message.Attachments))
                .Include(nameof(Message.Reactions))
                .FirstAsync(m => m.Id == 2)).ToMessageModel()
        };

        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
    public async Task GetLastMessagesAsync_ThrowsArgumentException_WhenCountLessThanZero()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
    public async Task GetLastMessagesAsync_ThrowsArgumentException_WhenSkipLessThanZero()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
    public async Task GetLastMessagesAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 2);
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
    public async Task GetSurroundingMessagesAsync_ThrowsArgumentException_WhenCountLessThanZero()
    {
        // Arrange
        var request = new RequestToGetSurroundingMessages
        {
            IssuerId = 1,
            MessageId = 1,
            Count = -1
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetSurroundingMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetSurroundingMessagesAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var request = new RequestToGetSurroundingMessages
        {
            IssuerId = 1,
            MessageId = 404,
            Count = 1
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetSurroundingMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task GetSurroundingMessagesAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToGetSurroundingMessages
        {
            IssuerId = 3,
            MessageId = 1,
            Count = 1
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetSurroundingMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task GetSurroundingMessagesAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var request = new RequestToGetSurroundingMessages
        {
            IssuerId = 1,
            MessageId = 5,
            Count = 1
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetSurroundingMessagesAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetMessagesRangeAsync_ReturnsMessagesRange()
    {
        // Arrange
        var expected = new List<MessageModel>
        {
            (await _dbContext.Messages
                .AsNoTracking()
                .Include(nameof(Message.Room))
                .Include(nameof(Message.Author))
                .Include(nameof(Message.RepliedTo))
                .Include(nameof(Message.Attachments))
                .Include(nameof(Message.Reactions))
                .FirstAsync(m => m.Id == 2)).ToMessageModel(),
            (await _dbContext.Messages
                .AsNoTracking()
                .Include(nameof(Message.Room))
                .Include(nameof(Message.Author))
                .Include(nameof(Message.RepliedTo))
                .Include(nameof(Message.Attachments))
                .Include(nameof(Message.Reactions))
                .FirstAsync(m => m.Id == 3)).ToMessageModel()
        };

        var request = new RequestToGetMessagesRange
        {
            IssuerId = 1,
            StartId = 2,
            EndId = 3,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        var result = await _messageService.GetMessagesRangeAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageModelEqualityComparer()));
    }

    [Test]
    public async Task GetMessagesRangeAsync_ThrowsArgumentException_WhenStartIdLessThanZero()
    {
        // Arrange
        var request = new RequestToGetMessagesRange
        {
            IssuerId = 1,
            StartId = -1,
            EndId = 3,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessagesRangeAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetMessagesRangeAsync_ThrowsArgumentException_WhenEndIdLessThanZero()
    {
        // Arrange
        var request = new RequestToGetMessagesRange
        {
            IssuerId = 1,
            StartId = 2,
            EndId = -1,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessagesRangeAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetMessagesRangeAsync_ThrowsMessageNotFoundException_WhenStartIdIsNotInRoom()
    {
        // Arrange
        var request = new RequestToGetMessagesRange
        {
            IssuerId = 1,
            StartId = 3,
            EndId = 5,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessagesRangeAsync(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task GetMessagesRangeAsync_ThrowsMessageNotFoundException_WhenEndIdIsNotInRoom()
    {
        // Arrange
        var request = new RequestToGetMessagesRange
        {
            IssuerId = 1,
            StartId = 2,
            EndId = 5,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessagesRangeAsync(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task GetMessagesRangeAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToGetMessagesRange
        {
            IssuerId = 3,
            StartId = 2,
            EndId = 3,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessagesRangeAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    public async Task GetUnreadRepliesAsync_ReturnsUnreadReplies()
    {
        // Arrange
        var expected = new List<MessageModel>
        {
            (await _dbContext.Messages
                .AsNoTracking()
                .Include(nameof(Message.Room))
                .Include(nameof(Message.Author))
                .Include(nameof(Message.RepliedTo))
                .Include(nameof(Message.Attachments))
                .Include(nameof(Message.Reactions))
                .FirstAsync(m => m.Id == 2)).ToMessageModel()
        };

        (await _dbContext.LastTimeUserReadChats.FirstAsync(l => l.UserId == 1)).Timestamp =
            DateTime.Now - TimeSpan.FromDays(1);

        await _dbContext.SaveChangesAsync();

        var request = new RequestToGetUnreadReplies
        {
            IssuerId = 1,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        var result = await _messageService.GetUnreadRepliesAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageModelEqualityComparer()));
    }

    [Test]
    public async Task GetUnreadRepliesAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var request = new RequestToGetUnreadReplies
        {
            IssuerId = 404,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetUnreadRepliesAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetUnreadRepliesAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToGetUnreadReplies
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetUnreadRepliesAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetUnreadRepliesAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToGetUnreadReplies
        {
            IssuerId = 3,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetUnreadRepliesAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task GetMessageById_ReturnsCorrectMessage()
    {
        // Arrange
        var expected = (await _dbContext.Messages
            .AsNoTracking()
            .Include(nameof(Message.Room))
            .Include(nameof(Message.Author))
            .Include(nameof(Message.RepliedTo))
            .Include(nameof(Message.Attachments))
            .Include(nameof(Message.Reactions))
            .FirstAsync(m => m.Id == 2)).ToMessageModel();

        var request = new RequestToGetMessage
        {
            IssuerId = 1,
            MessageId = 2
        };

        // Act
        var result = await _messageService.GetMessageById(request);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageModelEqualityComparer()));
    }

    [Test]
    public async Task GetMessageById_ThrowsMessageNotFoundException_WhenNotFound()
    {
        // Arrange
        var request = new RequestToGetMessage
        {
            IssuerId = 1,
            MessageId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessageById(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task GetMessageById_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var request = new RequestToGetMessage
        {
            IssuerId = 2,
            MessageId = 5
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessageById(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetMessageById_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToGetMessage
        {
            IssuerId = 3,
            MessageId = 2
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.GetMessageById(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task SendAsync_SendsMessage()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var message = await _dbContext.Messages
            .Include(nameof(Message.Attachments)).OrderByDescending(m => m.Id).FirstAsync();

        Assert.That(await _dbContext.Messages.CountAsync() == 6);
        Assert.That(message.Attachments.Count == 1);
        Assert.That(message.Attachments[0].Id == 1);
    }

    [Test]
    public async Task SendAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(s => s.UserId == 1);
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            Content = "Hello"
        };

        // Act
        await _messageService.SendAsync(request);

        // Assert
        var statsAfter = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(s => s.UserId == 1);
        Assert.That(statsAfter.MessagesSent - statsBefore.MessagesSent == 1);
    }

    [Test]
    public async Task SendAsync_ThrowsArgumentException_WhenMessageIsEmpty()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            Content = "",
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task SendAsync_ThorwsAttachmentNotFoundException_WhenAttachmentWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            Content = "Hello",
            AttachmentsIds = new List<long>() { 404 },
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<AttachmentNotFoundException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsAttachmentNotFoundException_WhenAttachmentIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            Content = "Hello",
            AttachmentsIds = new List<long>() { 2 },
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<AttachmentNotFoundException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsStringTooLongException_WhenMessageContentIsTooLong()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 1,
            Content = new string('a', 4097),
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
    public async Task SendAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToSendMessage
        {
            IssuerId = 3,
            RoomGuid = room.Guid,
            Content = "Hello",
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.SendAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task SendAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 2);
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
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 1,
            MessageId = 1,
            NewContent = "New content"
        };

        // Act
        await _messageService.EditAsync(request);

        // Assert
        var messageAfter = await _dbContext.Messages.AsNoTracking().FirstAsync(m => m.Id == 1);
        Assert.That(messageAfter.Content == request.NewContent);
        Assert.That(messageAfter.EditDate != null);
    }

    [Test]
    public async Task EditAsync_ThrowsArgumentException_WhenMessageIsEmpty()
    {
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 3,
            MessageId = 2,
            NewContent = ""
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.EditAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task EditAsync_ThrowsStringTooLongException_WhenMessageContentIsTooLong()
    {
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 1,
            MessageId = 1,
            NewContent = new string('a', 4097)
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.EditAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task EditAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 1,
            MessageId = 404,
            NewContent = "New content"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.EditAsync(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task EditAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 3,
            MessageId = 4,
            NewContent = "New content"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.EditAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 2,
            MessageId = 1,
            NewContent = "New content"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.EditAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task EditAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var request = new RequestToEditMessage
        {
            IssuerId = 1,
            MessageId = 5,
            NewContent = "New content"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.EditAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task DeleteAsync_DeletesMessage()
    {
        // Arrange
        var request = new RequestToDeleteMessage
        {
            IssuerId = 1,
            MessageId = 1
        };

        // Act
        await _messageService.DeleteAsync(request);

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 4);
    }

    [Test]
    public async Task DeleteAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var request = new RequestToDeleteMessage
        {
            IssuerId = 1,
            MessageId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task DeleteAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToDeleteMessage
        {
            IssuerId = 3,
            MessageId = 4
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task DeleteAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        // Arrange
        var request = new RequestToDeleteMessage
        {
            IssuerId = 1,
            MessageId = 2
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task DeleteAsync_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var request = new RequestToDeleteMessage
        {
            IssuerId = 1,
            MessageId = 5
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task AddReaction_AddsReaction()
    {
        // Arrange
        var request = new RequestToAddReactionOnMessage
        {
            IssuerId = 1,
            MessageId = 1,
            Reaction = "😎"
        };

        // Act
        await _messageService.AddReaction(request);

        // Assert
        var messageAfter = await _dbContext.Messages.AsNoTracking()
            .Include(nameof(Message.Reactions))
            .FirstAsync(m => m.Id == 1);

        Assert.That(messageAfter.Reactions.Count == 3);
        Assert.That(messageAfter.Reactions.OrderByDescending(r => r.Id).First().Symbol == request.Reaction);
    }

    [Test]
    public async Task AddReaction_ThrowsInvalidActionException_WhenReactionWithTheSameSymbolIsAlreadySet()
    {
        // Arrange
        var request = new RequestToAddReactionOnMessage
        {
            IssuerId = 1,
            MessageId = 1,
            Reaction = "🤣"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.AddReaction(request);

        // Assert
        Assert.ThrowsAsync<InvalidActionException>(act);
    }

    [Test]
    public async Task AddReaction_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(s => s.UserId == 1);
        var request = new RequestToAddReactionOnMessage
        {
            IssuerId = 1,
            MessageId = 1,
            Reaction = "😎"
        };

        // Act
        await _messageService.AddReaction(request);

        // Assert
        var statsAfter = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(s => s.UserId == 1);
        Assert.That(statsAfter.ReactionsSet - statsBefore.ReactionsSet == 1);
    }

    [Test]
    public async Task AddReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var request = new RequestToAddReactionOnMessage
        {
            IssuerId = 1,
            MessageId = 404,
            Reaction = "😎"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.AddReaction(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task AddReaction_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToAddReactionOnMessage
        {
            IssuerId = 3,
            MessageId = 1,
            Reaction = "😎"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.AddReaction(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task AddReaction_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var request = new RequestToAddReactionOnMessage
        {
            IssuerId = 1,
            MessageId = 5,
            Reaction = "😎"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.AddReaction(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task RemoveReaction_RemovesReaction()
    {
        // Arrange
        var request = new RequestToRemoveReactionFromMessage
        {
            IssuerId = 1,
            ReactionId = 1
        };

        // Act
        await _messageService.RemoveReaction(request);

        // Assert
        var messageAfter = await _dbContext.Messages.AsNoTracking().Include(nameof(Message.Reactions))
            .FirstAsync(m => m.Id == 1);

        Assert.That(messageAfter.Reactions.Count == 1);
        Assert.That(await _dbContext.Reactions.CountAsync() == 2);
    }

    [Test]
    public async Task RemoveReaction_ThrowsReactionNotFoundException_WhenReactionWasNotFound()
    {
        // Arrange
        var request = new RequestToRemoveReactionFromMessage
        {
            IssuerId = 1,
            ReactionId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.RemoveReaction(request);

        // Assert
        Assert.ThrowsAsync<ReactionNotFoundException>(act);
    }

    [Test]
    public async Task RemoveReaction_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var request = new RequestToRemoveReactionFromMessage
        {
            IssuerId = 3,
            ReactionId = 2
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.RemoveReaction(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfReaction()
    {
        // Arrange
        var request = new RequestToRemoveReactionFromMessage
        {
            IssuerId = 2,
            ReactionId = 1
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.RemoveReaction(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task RemoveReaction_ThrowsRoomExpiredException_WhenRoomIsExpired()
    {
        // Arrange
        var request = new RequestToRemoveReactionFromMessage
        {
            IssuerId = 1,
            ReactionId = 3
        };

        // Act
        AsyncTestDelegate act = async () => await _messageService.RemoveReaction(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }
}