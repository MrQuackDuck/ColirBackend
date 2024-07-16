using Colir.Exceptions;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class LastTimeUserReadChatRepository : ILastTimeUserReadChatRepository
{
    private ColirDbContext _dbContext;
    
    public LastTimeUserReadChatRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Gets all times users read any chats recently
    /// </summary>
    public async Task<IEnumerable<LastTimeUserReadChat>> GetAllAsync()
    {
        return await _dbContext.LastTimeUserReadChats
            .Include(nameof(LastTimeUserReadChat.Room))
            .Include(nameof(LastTimeUserReadChat.User))
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets last time user read the chat in certain room by userId and roomId
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="roomId">Id of the room</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    public async Task<LastTimeUserReadChat> GetAsync(long userId, long roomId)
    {
        if (!await _dbContext.Rooms.AnyAsync(r => r.Id == roomId))
        {
            throw new RoomNotFoundException();
        }
        
        if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
        {
            throw new UserNotFoundException();
        }
        
        return await _dbContext.LastTimeUserReadChats
            .Include(nameof(LastTimeUserReadChat.Room))
            .Include(nameof(LastTimeUserReadChat.User))
            .FirstAsync(l => l.RoomId == roomId && l.UserId == userId);
    }

    /// <summary>
    /// Gets last time user read the chat in certain room by id
    /// </summary>
    /// <param name="id">Id of the entry</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<LastTimeUserReadChat> GetByIdAsync(long id)
    {
        try
        {
            return await _dbContext.LastTimeUserReadChats
                .Include(nameof(LastTimeUserReadChat.Room))
                .Include(nameof(LastTimeUserReadChat.User))
                .FirstAsync(l => l.Id == id);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

    /// <summary>
    /// Add last time user read chat in certain room
    /// </summary>
    /// <param name="entity"></param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    /// <exception cref="InvalidActionException">Thrown when the entry with same userId and roomId already exists</exception>
    public async Task AddAsync(LastTimeUserReadChat entity)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == entity.RoomId);
        if (room is null)
        {
            throw new RoomNotFoundException();
        }

        if (room.ExpiryDate < DateTime.Now)
        {
            throw new RoomExpiredException();
        }
        
        if (!await _dbContext.Users.AnyAsync(u => u.Id == entity.UserId))
        {
            throw new UserNotFoundException();
        }
        
        if (await _dbContext.LastTimeUserReadChats.AnyAsync(l => l.UserId == entity.UserId && l.RoomId == entity.RoomId))
        {
            throw new InvalidActionException();
        }
        
        await _dbContext.LastTimeUserReadChats.AddAsync(entity);
    }

    /// <summary>
    /// Deletes the last time user read chat in certain room
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    /// <exception cref="NotFoundException">Thrown when the entity wasn't found</exception>
    public void Delete(LastTimeUserReadChat entity)
    {
        if (!_dbContext.LastTimeUserReadChats.Any(l => l.Id == entity.Id))
        {
            throw new NotFoundException();
        }
        
        _dbContext.LastTimeUserReadChats.Remove(entity);
    }

    /// <summary>
    /// Deletes the last time user read chat in certain room by id
    /// </summary>
    /// <param name="id">Id of entity to delete</param>
    /// <exception cref="NotFoundException">Thrown when the entity wasn't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await GetByIdAsync(id);
        _dbContext.LastTimeUserReadChats.Remove(target);
    }

    /// <summary>
    /// Updates last time user read chat
    /// </summary>
    /// <param name="entity">Last time user read chat to update</param>
    /// <exception cref="NotFoundException">Thrown when last time user read chat wasn't found</exception>
    /// <exception cref="ArgumentException">Thrown when another room id provided</exception>
    /// <exception cref="ArgumentException">Thrown when another user id provided</exception>
    /// <exception cref="RoomExpiredException">Thrown when expired room's id provided</exception>
    public void Update(LastTimeUserReadChat entity)
    {
        var originalEntity = _dbContext.LastTimeUserReadChats
            .Include(nameof(LastTimeUserReadChat.Room)).FirstOrDefault(l => l.Id == entity.Id);
        
        if (originalEntity == null)
        {
            throw new NotFoundException();
        }
        
        // Check if another UserId provided
        if (originalEntity.UserId != entity.UserId)
        {
            throw new ArgumentException("You can't update last time user read chat with different user id!");
        }
        
        // Check if another RoomId provided
        if (originalEntity.RoomId != entity.RoomId)
        {
            throw new ArgumentException("You can't update last time user read chat with different room id!");
        }

        if (originalEntity.Room.ExpiryDate < DateTime.Now)
        {
            throw new RoomExpiredException();
        }
        
        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(entity).State = EntityState.Modified;
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