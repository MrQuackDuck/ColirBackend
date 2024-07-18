using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class MessageRepository : IMessageRepository
{
    private ColirDbContext _dbContext;
    
    public MessageRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all messages
    /// </summary>
    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await _dbContext.Messages
            .AsNoTracking()
            .Include(nameof(Message.Room))
            .Include(nameof(Message.Author))
            .Include(nameof(Message.RepliedTo))
            .Include(nameof(Message.Attachments))
            .Include(nameof(Message.Reactions))
            .ToListAsync();
    }

    /// <summary>
    /// Gets the message by id
    /// </summary>
    /// <param name="id">Id of message to get</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found by provided id</exception>
    public async Task<Message> GetByIdAsync(long id)
    {
        return await _dbContext.Messages
            .AsNoTracking()
            .Include(nameof(Message.Room))
            .Include(nameof(Message.Author))
            .Include(nameof(Message.RepliedTo))
            .Include(nameof(Message.Attachments))
            .Include(nameof(Message.Reactions))
            .FirstOrDefaultAsync(m => m.Id == id) ?? throw new MessageNotFoundException();
    }
    
    /// <summary>
    /// Gets last sent messages in certain room
    /// </summary>
    /// <param name="roomGuid">Room GUID to get messages in</param>
    /// <param name="count">Count of messages to take</param>
    /// <param name="skip">Count of messages to skip</param>
    /// <exception cref="ArgumentException">Thrown when either count or skip is less than zero</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by provided GUID</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public async Task<List<Message>> GetLastMessages(string roomGuid, int count, int skip)
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

        if (room.ExpiryDate < DateTime.Now)
        {
            throw new RoomExpiredException();
        }
        
        return await _dbContext.Messages
            .AsNoTracking()
            .Include(nameof(Message.Room))
            .Include(nameof(Message.Author))
            .Include(nameof(Message.RepliedTo))
            .Include(nameof(Message.Attachments))
            .Include(nameof(Message.Reactions))
            .Where(m => m.Room.Guid == roomGuid)
            .OrderByDescending(m => m.PostDate)
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Adds the message to DB
    /// </summary>
    /// <param name="message">Message to add</param>
    /// <exception cref="MessageNotFoundException">Thrown when replied message wasn't found</exception>
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
        if (room.ExpiryDate < DateTime.Now) throw new RoomExpiredException();
        
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
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    public void Delete(Message message)
    {
        var target = _dbContext.Messages.FirstOrDefault(m => m.Id == message.Id) ?? throw new MessageNotFoundException();
        
        var room = _dbContext.Rooms.First(r => r.Id == target.RoomId) ?? throw new RoomNotFoundException();
        if (room.ExpiryDate < DateTime.Now) throw new RoomExpiredException();
        
        _dbContext.Messages.Remove(target);
    }

    /// <summary>
    /// Deletes the message by id
    /// </summary>
    /// <param name="id">Id of the message to delete</param>
    /// <exception cref="NotFoundException">Thrown when the message wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Messages
            .Include(nameof(Message.Room))
            .FirstOrDefaultAsync(m => m.Id == id) ?? throw new MessageNotFoundException();
        
        if (target.Room.ExpiryDate < DateTime.Now) throw new RoomExpiredException();
        
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
        
        if (originalEntity.Room.ExpiryDate < DateTime.Now) throw new RoomExpiredException();
        
        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(message).State = EntityState.Modified;
    }

    /// <summary>
    /// Saves the changes to DB
    /// </summary>
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Saves the changes to DB asynchronously
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}