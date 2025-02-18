﻿using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Extensions;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ColirDbContext _dbContext;

    public MessageRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all messages
    /// </summary>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    public async Task<IEnumerable<Message>> GetAllAsync(string[]? overriddenIncludes = default)
    {
        return await _dbContext.Messages
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .ToListAsync();
    }

    /// <summary>
    /// Gets the message by id
    /// </summary>
    /// <param name="id">Id of the message to get</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found by provided id</exception>
    public async Task<Message> GetByIdAsync(long id, string[]? overriddenIncludes = default)
    {
        return await _dbContext.Messages
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id) ?? throw new MessageNotFoundException();
    }

    /// <summary>
    /// Gets last sent messages from a certain room
    /// </summary>
    /// <param name="roomGuid">Room GUID to get messages in</param>
    /// <param name="count">Count of messages to take</param>
    /// <param name="skip">Count of messages to skip</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="ArgumentException">Thrown when either count or skip is less than zero</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by provided GUID</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public async Task<List<Message>> GetLastMessagesAsync(string roomGuid, int count, int skip,
        string[]? overriddenIncludes = default)
    {
        if (count < 0)
        {
            throw new ArgumentException("Count to take can't be less than zero!");
        }

        if (skip < 0)
        {
            throw new ArgumentException("Skip count can't be less than zero!");
        }

        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Guid == roomGuid) ?? throw new RoomNotFoundException();

        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        return await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.RoomId == room.Id)
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .OrderByDescending(m => m.Id)
            .Skip(skip)
            .Take(count)
            .AsSplitQuery()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves messages in the range
    /// </summary>
    /// <param name="roomGuid">Room GUID to get messages in</param>
    /// <param name="startId">Id of the first message</param>
    /// <param name="endId">Id of the last message</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by provided GUID</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public Task<List<Message>> GetMessagesRangeAsync(string roomGuid, long startId, long endId,
        string[]? overriddenIncludes = default)
    {
        if (startId < 0 || endId < 0)
        {
            throw new ArgumentException("Message IDs can't be less than zero!");
        }

        var room = _dbContext.Rooms.FirstOrDefault(r => r.Guid == roomGuid) ?? throw new RoomNotFoundException();

        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        if (startId > endId)
        {
            var temp = startId;
            startId = endId;
            endId = temp;
        }

        return _dbContext.Messages
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .Where(m => m.Room.Guid == roomGuid && m.Id >= startId && m.Id <= endId)
            .OrderBy(m => m.Id)
            .AsSplitQuery()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves messages around the message
    /// </summary>
    /// <param name="roomGuid">Room GUID to get messages in</param>
    /// <param name="messageId">Id of the message to get surrounding messages of</param>
    /// <param name="count">Count of messages to take</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="ArgumentException">Thrown when count is less than zero</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by provided GUID</exception>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found by the provided id</exception>
    public async Task<List<Message>> GetSurroundingMessages(string roomGuid, long messageId, int count,
        string[]? overriddenIncludes = default)
    {
        if (count < 0)
        {
            throw new ArgumentException("Count to take can't be less than zero!");
        }

        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Guid == roomGuid) ?? throw new RoomNotFoundException();

        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        var message = await _dbContext.Messages
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == messageId) ?? throw new MessageNotFoundException();

        var messagesBefore = await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.RoomId == room.Id && m.Id < message.Id)
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .OrderByDescending(m => m.Id)
            .Take(count / 2)
            .AsSplitQuery()
            .ToListAsync();

        var messagesAfter = await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.RoomId == room.Id && m.Id > message.Id)
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .OrderBy(m => m.Id)
            .Take(count / 2)
            .AsSplitQuery()
            .ToListAsync();

        var result = messagesBefore.Concat(new[] { message }).Concat(messagesAfter).ToList();

        return result;
    }

    /// <summary>
    /// Gets all unread messages that have a reply to messages of the provided user
    /// </summary>
    /// <param name="roomGuid">Room GUID to get messages in</param>
    /// <param name="userId">Id of the user to get replies to</param>
    /// <param name="date">Date to get messages after</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by provided GUID</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public async Task<List<Message>> GetAllRepliesToUserAfterDateAsync(string roomGuid, long userId, DateTime date,
        string[]? overriddenIncludes = default)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Guid == roomGuid) ?? throw new RoomNotFoundException();

        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        return await _dbContext.Messages
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .Where(m => m.RoomId == room.Id && m.RepliedMessageId != null && m.RepliedTo!.AuthorId == userId && m.PostDate > date)
            .OrderBy(m => m.Id)
            .AsSplitQuery()
            .ToListAsync();
    }

    /// <summary>
    /// Adds the message to the DB
    /// </summary>
    /// <param name="message">Message to add</param>
    /// <exception cref="MessageNotFoundException">Thrown when the replied message wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the author wasn't found</exception>
    public async Task AddAsync(Message message)
    {
        if (message.RepliedMessageId != null && !await _dbContext.Messages.AnyAsync(m => m.Id == message.RepliedMessageId))
        {
            throw new MessageNotFoundException();
        }

        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == message.RoomId) ?? throw new RoomNotFoundException();
        if (room.IsExpired()) throw new RoomExpiredException();

        if (!await _dbContext.Users.AnyAsync(u => u.Id == message.AuthorId))
        {
            throw new UserNotFoundException();
        }

        await _dbContext.Messages.AddAsync(message);
    }

    /// <summary>
    /// Deletes the message
    /// </summary>
    /// <param name="message">Message to delete</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    public async Task DeleteAsync(Message message)
    {
        var target = await _dbContext.Messages.FindAsync(message.Id) ?? throw new MessageNotFoundException();

        var reliedMessages = _dbContext.Messages.Where(m => m.RepliedMessageId == target.Id);
        foreach (var reliedMessage in reliedMessages)
            reliedMessage.RepliedTo = null;

        _dbContext.Messages.Remove(target);
        _dbContext.Attachments.RemoveRange(_dbContext.Attachments.Where(a => a.MessageId == target.Id));
    }

    /// <summary>
    /// Deletes the message by id
    /// </summary>
    /// <param name="id">Id of the message to delete</param>
    /// <exception cref="NotFoundException">Thrown when the message wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Messages.FindAsync(id) ?? throw new MessageNotFoundException();

        _dbContext.Messages.Remove(target);
        _dbContext.Attachments.RemoveRange(_dbContext.Attachments.Where(a => a.MessageId == id));
        _dbContext.Reactions.RemoveRange(_dbContext.Reactions.Where(r => r.MessageId == id));
    }

    /// <summary>
    /// Updates the message
    /// </summary>
    /// <param name="message">The message to update</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public void Update(Message message)
    {
        var originalEntity = _dbContext.Messages.Include(nameof(Message.Room)).FirstOrDefault(m => m.Id == message.Id);

        if (originalEntity == null)
        {
            throw new MessageNotFoundException();
        }

        if (originalEntity.Room.IsExpired()) throw new RoomExpiredException();

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(message).State = EntityState.Modified;
    }

    /// <summary>
    /// Saves changes to the DB
    /// </summary>
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Saves changes to the DB asynchronously
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private static string[] GetDefaultIncludes() => new[]
    {
        nameof(Message.Author),
        nameof(Message.Attachments),
        nameof(Message.RepliedTo),
        nameof(Message.RepliedTo) + "." + nameof(Message.Author),
        nameof(Message.RepliedTo) + "." + nameof(Message.Attachments),
        nameof(Message.Reactions),
        nameof(Message.Reactions) + "." + nameof(Reaction.Author)
    };
}