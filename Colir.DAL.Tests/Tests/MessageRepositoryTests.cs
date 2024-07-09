using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

public class MessageRepositoryTests : IMessageRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private MessageRepository _messageRepository = default!;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize message repository
        _messageRepository = new MessageRepository(_dbContext);

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
    public async Task GetAllAsync_ReturnsAllMessages()
    {
        // Arrange
        List<Message> expected = _dbContext.Messages.ToList();

        // Act
        var result = await _messageRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessages()
    {
        // Arrange
        var expectedMessages = new List<Message> 
        { 
            _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefault(message => message.Id == 1)!
        };

        // Act
        var result = await _messageRepository.GetLastMessages("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, 1);

        // Assert
        Assert.That(result, Is.EqualTo(expectedMessages).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessagesWithAttachments()
    {
        // Arrange
        var expectedMessages = new List<Message>
        {
            _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefault(message => message.Id == 1)!
        };

        // Act
        var result = await _messageRepository.GetLastMessages("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, 1);

        // Assert
        Assert.That(result, Is.EqualTo(expectedMessages).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessagesWithReactions()
    {
        // Arrange
        var expectedMessages = new List<Message>
        {
            _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefault(message => message.Id == 1)!
        };

        // Act
        var result = await _messageRepository.GetLastMessages("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, 1);

        // Assert
        Assert.That(result, Is.EqualTo(expectedMessages).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessages_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessages("00000000-0000-0000-0000-000000000000", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessages("00000000-0000-0000-0000-000000000000", -1, 1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessages("00000000-0000-0000-0000-000000000000", 1, -1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessages("12ffb712-aca7-416f-b899-8f9aaac6770f", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsMessage_WhenFound()
    {
        // Arrange
        Message expected = _dbContext.Messages.First(m => m.Id == 1);

        // Act
        var result = await _messageRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.GetByIdAsync(100);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewMessage()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 1, // "Room #1"
            AuthorId = 1, // "First User"
        };

        // Act
        await _messageRepository.AddAsync(messageToAdd);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 3);
    }

    [Test]
    public async Task AddAsync_AppliesAttachmentsToMessage()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 1, // "Room #1"
            AuthorId = 1, // "First User"
            Attachments = new List<Attachment>
            {
                new Attachment()
                {
                    Id = 2,
                    Filename = "Test.exe",
                    MessageId = 3,
                    Path = "./root/Test.exe",
                    SizeInKb = 203
                }
            }
        };

        // Act
        await _messageRepository.AddAsync(messageToAdd);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 3);
        Assert.That(_dbContext.Attachments.Count() == 2);
    }

    [Test]
    public async Task AddAsync_AppliesReactionsToMessage()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 1, // "Room #1"
            AuthorId = 1, // "First User"
            Reactions = new List<Reaction>()
            {
                new Reaction()
                {
                    Id = 2,
                    AuthorId = 1,
                    MessageId = 3,
                    Symbol = "😮"
                }
            }
        };

        // Act
        await _messageRepository.AddAsync(messageToAdd);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 3);
        Assert.That(_dbContext.Reactions.Count() == 2);
    }

    [Test]
    public async Task AddAsync_ThrowsUserNotFoundException_WhenAuthorWasNotFound()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 1, // "Room #1"
            AuthorId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _messageRepository.AddAsync(messageToAdd);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 404,
            AuthorId = 1, // "First User"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageRepository.AddAsync(messageToAdd);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsMessageNotFoundException_WhenRepliedMessageWasNotFound()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 1, // "Room #1"
            AuthorId = 1, // "First User"
            RepliedMessageId = 404 // Non-existent message ID
        };

        // Act
        AsyncTestDelegate act = async () => await _messageRepository.AddAsync(messageToAdd);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 3,
            Content = "Message in Room #2",
            PostDate = DateTime.Now,
            RoomId = 2, // "Room #2" (expired)
            AuthorId = 1 // "First User"
        };

        // Act
        AsyncTestDelegate act = async () => await _messageRepository.AddAsync(messageToAdd);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task Delete_DeletesMessage()
    {
        // Arrange
        var messageToDelete = _dbContext.Messages.First(m => m.Id == 1);

        // Act
        _messageRepository.Delete(messageToDelete);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 1);
    }

    [Test]
    public async Task Delete_DeletesAllRelatedReactions()
    {
        // Arrange
        var messageToDelete = _dbContext.Messages.First(m => m.Id == 1);

        // Act
        _messageRepository.Delete(messageToDelete);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 0);
    }

    [Test]
    public async Task Delete_DeletesAllRelatedAttachments()
    {
        // Arrange
        var messageToDelete = _dbContext.Messages.First(m => m.Id == 1);

        // Act
        _messageRepository.Delete(messageToDelete);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Attachments.Count() == 0);
    }

    [Test]
    public async Task Delete_NotDeletesAnyOtherMessages()
    {
        // Arrange
        var messageToDelete = _dbContext.Messages.First(m => m.Id == 1);

        // Act
        _messageRepository.Delete(messageToDelete);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Any(m => m.Id == 2), Is.True);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        // Arrange
        var messageToDelete = new Message { Id = 404 };

        // Act
        TestDelegate act = () => _messageRepository.Delete(messageToDelete);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task Delete_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var messageToDelete = _dbContext.Messages.First(m => m.RoomId == 2);

        // Act
        TestDelegate act = () => _messageRepository.Delete(messageToDelete);

        // Assert
        Assert.Throws<RoomExpiredException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesMessage()
    {
        // Act
        await _messageRepository.DeleteByIdAsync(1);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 1);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        // Act
        await _messageRepository.DeleteByIdAsync(1);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        // Act
        await _messageRepository.DeleteByIdAsync(1);
        _messageRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Attachments.Count() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var expiredMessageId = _dbContext.Messages.First(m => m.RoomId == 2).Id;

        // Act
        AsyncTestDelegate act = async () => await _messageRepository.DeleteByIdAsync(expiredMessageId);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task Update_UpdatesMessage()
    {
        // Arrange
        var messageToUpdate = _dbContext.Messages.First(m => m.Id == 1);
        messageToUpdate.Content = "Updated content";

        // Act
        _messageRepository.Update(messageToUpdate);
        _messageRepository.SaveChanges();

        // Assert
        var updatedMessage = _dbContext.Messages.First(m => m.Id == 1);
        Assert.That(updatedMessage.Content, Is.EqualTo("Updated content"));
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        // Arrange
        var messageToUpdate = new Message { Id = 404, Content = "Updated content" };

        // Act
        TestDelegate act = () => _messageRepository.Update(messageToUpdate);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var messageToUpdate = _dbContext.Messages.First(m => m.RoomId == 2);
        messageToUpdate.Content = "Updated content";

        // Act
        TestDelegate act = () => _messageRepository.Update(messageToUpdate);

        // Assert
        Assert.Throws<RoomExpiredException>(act);
    }
}