﻿using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
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
        List<Message> expected = await _dbContext.Messages
            .Include(nameof(Message.Author))
            .Include(nameof(Message.Reactions))
            .Include(nameof(Message.Attachments))
            .ToListAsync();

        // Act
        var result = await _messageRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));

        Assert.That(result.Select(r => r.Author).OrderBy(r => r!.Id),
            Is.EqualTo(expected.Select(r => r.Author).OrderBy(r => r!.Id)).Using(new UserEqualityComparer()));

        Assert.That(result.SelectMany(r => r.Reactions).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.Reactions).OrderBy(r => r.Id)).Using(new ReactionEqualityComparer()));

        Assert.That(result.SelectMany(r => r.Attachments).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.Attachments).OrderBy(r => r.Id))
                .Using(new AttachmentEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessages()
    {
        // Arrange
        var expected = new List<Message>
        {
            (await _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefaultAsync(message => message.Id == 2))!
        };

        // Act
        var result = await _messageRepository.GetLastMessagesAsync("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, 0);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));

        Assert.That(result.SelectMany(r => r.Reactions).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.Reactions).OrderBy(r => r.Id)).Using(new ReactionEqualityComparer()));

        Assert.That(result.SelectMany(r => r.Attachments).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.Attachments).OrderBy(r => r.Id))
                .Using(new AttachmentEqualityComparer()));
    }

    [Test]
    public async Task GetLastMessages_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessagesAsync("00000000-0000-0000-0000-000000000000", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentException_WhenCountLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessagesAsync("00000000-0000-0000-0000-000000000000", -1, 1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentException_WhenSkipLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessagesAsync("00000000-0000-0000-0000-000000000000", 1, -1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetLastMessagesAsync("12ffb712-aca7-416f-b899-8f9aaac6770f", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetMessagesRange_ReturnsMessagesRange()
    {
        // Arrange
        var expected = new List<Message>
        {
            (await _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefaultAsync(message => message.Id == 1))!,
            (await _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefaultAsync(message => message.Id == 2))!
        };

        // Act
        var result = await _messageRepository.GetMessagesRangeAsync("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, 2);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetMessagesRange_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetMessagesRangeAsync("00000000-0000-0000-0000-000000000000", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetMessagesRange_ThrowsArgumentException_WhenStartIdLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetMessagesRangeAsync("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", -1, 1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetMessagesRange_ThrowsArgumentException_WhenEndIdLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetMessagesRangeAsync("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, -1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetMessagesRange_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetMessagesRangeAsync("12ffb712-aca7-416f-b899-8f9aaac6770f", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetSurroundingMessages_ThrowsArgumentException_WhenCountLessThanZero()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetSurroundingMessages("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 1, -1);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetSurroundingMessages_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetSurroundingMessages("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", 404, 1);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task GetSurroundingMessages_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetSurroundingMessages("12ffb712-aca7-416f-b899-8f9aaac6770f", 1, 1);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetAllRepliesToUserAfterDateAsync_ReturnsAllReplies()
    {
        // Arrange
        var expected = new List<Message>
        {
            (await _dbContext.Messages
                .Include(nameof(Message.Author))
                .Include(nameof(Message.Reactions))
                .Include(nameof(Message.Attachments))
                .FirstOrDefaultAsync(message => message.Id == 2))!
        };

        // Act
        var result = await _messageRepository.GetAllRepliesToUserAfterDateAsync("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e",
            1, DateTime.Now - new TimeSpan(1, 0, 0));

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetAllRepliesToUserAfterDateAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetAllRepliesToUserAfterDateAsync("00000000-0000-0000-0000-000000000000", 1,
                DateTime.Now);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }


    [Test]
    public async Task GetAllRepliesToUserAfterDateAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Act
        AsyncTestDelegate act = async () =>
            await _messageRepository.GetAllRepliesToUserAfterDateAsync("12ffb712-aca7-416f-b899-8f9aaac6770f", 1,
                DateTime.Now);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsMessage_WhenFound()
    {
        // Arrange
        Message expected = await _dbContext.Messages
            .Include(nameof(Message.Author))
            .Include(nameof(Message.Reactions))
            .Include(nameof(Message.Attachments))
            .FirstAsync(m => m.Id == 1);

        // Act
        var result = await _messageRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new MessageEqualityComparer()));
        Assert.That(result.Author, Is.EqualTo(expected.Author).Using(new UserEqualityComparer()));
        Assert.That(result.Reactions, Is.EqualTo(expected.Reactions).Using(new ReactionEqualityComparer()));
        Assert.That(result.Attachments, Is.EqualTo(expected.Attachments).Using(new AttachmentEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.GetByIdAsync(100);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewMessage()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 4,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = 1, // "Room #1"
            AuthorId = 1, // "First User"
        };

        // Act
        await _messageRepository.AddAsync(messageToAdd);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 4);
    }

    [Test]
    public async Task AddAsync_AppliesAttachmentsToMessage()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 4,
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
                    Path = "./root/Test.exe",
                    SizeInBytes = 203
                }
            }
        };

        // Act
        await _messageRepository.AddAsync(messageToAdd);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 4);
        Assert.That(await _dbContext.Attachments.CountAsync() == 2);
    }

    [Test]
    public async Task AddAsync_AppliesReactionsToMessage()
    {
        // Arrange
        var messageToAdd = new Message()
        {
            Id = 4,
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
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 4);
        Assert.That(await _dbContext.Reactions.CountAsync() == 2);
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
    public async Task DeleteAsync_DeletesMessage()
    {
        // Arrange
        var messageToDelete = await _dbContext.Messages.AsNoTracking().FirstAsync(m => m.Id == 1);

        // Act
        await _messageRepository.DeleteAsync(messageToDelete);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteAsync_DeletesAllRelatedReactions()
    {
        // Arrange
        var messageToDelete = await _dbContext.Messages.AsNoTracking().FirstAsync(m => m.Id == 1);

        // Act
        await _messageRepository.DeleteAsync(messageToDelete);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteAsync_DeletesAllRelatedAttachments()
    {
        // Arrange
        var messageToDelete = await _dbContext.Messages.FirstAsync(m => m.Id == 1);

        // Act
        await _messageRepository.DeleteAsync(messageToDelete);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Attachments.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteAsync_NotDeletesAnyOtherMessages()
    {
        // Arrange
        var messageToDelete = await _dbContext.Messages.FirstAsync(m => m.Id == 1);

        // Act
        await _messageRepository.DeleteAsync(messageToDelete);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.AnyAsync(m => m.Id == 2), Is.True);
    }

    [Test]
    public async Task DeleteAsync_ThrowsMessageNotFoundException_WhenMessageDoesNotExist()
    {
        // Arrange
        var messageToDelete = new Message { Id = 404 };

        // Act
        AsyncTestDelegate act = async () => await _messageRepository.DeleteAsync(messageToDelete);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesMessage()
    {
        // Act
        await _messageRepository.DeleteByIdAsync(1);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        // Act
        await _messageRepository.DeleteByIdAsync(1);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        // Act
        await _messageRepository.DeleteByIdAsync(1);
        await _messageRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Attachments.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _messageRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesMessage()
    {
        // Arrange
        var messageToUpdate = await _dbContext.Messages.AsNoTracking().FirstAsync(m => m.Id == 1);
        messageToUpdate.Content = "Updated content";

        // Act
        _messageRepository.Update(messageToUpdate);
        await _messageRepository.SaveChangesAsync();

        // Assert
        var updatedMessage = await _dbContext.Messages.FirstAsync(m => m.Id == 1);
        Assert.That(updatedMessage.Content, Is.EqualTo("Updated content"));
    }

    [Test]
    public async Task Update_ThrowsMessageNotFoundException_WhenMessageDoesNotExist()
    {
        // Arrange
        var messageToUpdate = new Message { Id = 404, Content = "Updated content" };

        // Act
        TestDelegate act = () => _messageRepository.Update(messageToUpdate);

        // Assert
        Assert.Throws<MessageNotFoundException>(act);
    }

    [Test]
    public async Task Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var messageToUpdate = await _dbContext.Messages.FirstAsync(m => m.RoomId == 2);
        messageToUpdate.Content = "Updated content";

        // Act
        TestDelegate act = () => _messageRepository.Update(messageToUpdate);

        // Assert
        Assert.Throws<RoomExpiredException>(act);
    }
}