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
        
        // Initialize room repository
        _messageRepository = new MessageRepository(_dbContext);
        
        // Add entities
        UnitTestHelper.SeedData(_dbContext);
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
        var expectedMessages = new List<Message> { _dbContext.Messages.FirstOrDefault(message => message.Id == 1)! };

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
                .Include(nameof(Message.Reactions))
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
        AsyncTestDelegate act = async () => await _messageRepository.GetLastMessages("00000000-0000-0000-0000-000000000000", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.GetLastMessages("00000000-0000-0000-0000-000000000000", -1, 1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.GetLastMessages("00000000-0000-0000-0000-000000000000", 1, -1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.GetLastMessages("12ffb712-aca7-416f-b899-8f9aaac6770f", 1, 1);

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
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesAttachmentsToMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesReactionsToMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenAuthorWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenRepliedMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_NotDeletesAnyOtherMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }
}