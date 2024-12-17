using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Extensions;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class LastTimeUserReadChatRepository : ILastTimeUserReadChatRepository
{
    private readonly ColirDbContext _dbContext;

    public LastTimeUserReadChatRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all times users read any chats recently
    /// </summary>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    public async Task<IEnumerable<LastTimeUserReadChat>> GetAllAsync(string[]? overriddenIncludes = default)
    {
        return await _dbContext.LastTimeUserReadChats
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(LastTimeUserReadChat.Room), nameof(LastTimeUserReadChat.User)])
            .ToListAsync();
    }

    /// <summary>
    /// Gets last time user read the chat in a certain room by userId and roomId
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="roomId">Id of the room</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    /// <exception cref="NotFoundException">Thrown when the record wasn't found</exception>
    public async Task<LastTimeUserReadChat> GetAsync(long userId, long roomId, string[]? overriddenIncludes = default)
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
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(LastTimeUserReadChat.Room), nameof(LastTimeUserReadChat.User)])
            .FirstOrDefaultAsync(l => l.RoomId == roomId && l.UserId == userId) ?? throw new NotFoundException();
    }

    /// <summary>
    /// Gets last time user read the chat in a certain room by id
    /// </summary>
    /// <param name="id">Id of the entry</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="NotFoundException">Thrown when the entity wasn't found</exception>
    public async Task<LastTimeUserReadChat> GetByIdAsync(long id, string[]? overriddenIncludes = default)
    {
        return await _dbContext.LastTimeUserReadChats
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(LastTimeUserReadChat.Room), nameof(LastTimeUserReadChat.User)])
            .FirstOrDefaultAsync(l => l.Id == id) ?? throw new NotFoundException();
    }

    /// <summary>
    /// Adds last time user read chat in a certain room
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    /// <exception cref="InvalidActionException">Thrown when an entry with the same userId and roomId already exists</exception>
    public async Task AddAsync(LastTimeUserReadChat entity)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == entity.RoomId);
        if (room is null)
        {
            throw new RoomNotFoundException();
        }

        if (room.IsExpired())
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
    /// Deletes the last time user read chat in a certain room
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    /// <exception cref="NotFoundException">Thrown when the entity wasn't found</exception>
    public async Task DeleteAsync(LastTimeUserReadChat entity)
    {
        var target = await _dbContext.LastTimeUserReadChats.FindAsync(entity.Id) ?? throw new NotFoundException();
        _dbContext.LastTimeUserReadChats.Remove(target);
    }

    /// <summary>
    /// Deletes the last time user read chat in a certain room by id
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <exception cref="NotFoundException">Thrown when the entity wasn't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.LastTimeUserReadChats.FindAsync(id) ?? throw new NotFoundException();
        _dbContext.LastTimeUserReadChats.Remove(target);
    }

    /// <summary>
    /// Updates last time user read chat
    /// </summary>
    /// <param name="entity">Last time user read chat to update</param>
    /// <exception cref="NotFoundException">Thrown when last time user read chat wasn't found</exception>
    /// <exception cref="ArgumentException">Thrown when another room id is provided</exception>
    /// <exception cref="ArgumentException">Thrown when another user id is provided</exception>
    /// <exception cref="RoomExpiredException">Thrown when an expired room's id is provided</exception>
    public void Update(LastTimeUserReadChat entity)
    {
        var originalEntity = _dbContext.LastTimeUserReadChats
            .Include(nameof(LastTimeUserReadChat.Room))
            .FirstOrDefault(l => l.Id == entity.Id) ?? throw new NotFoundException();

        // Check if another UserId is provided
        if (originalEntity.UserId != entity.UserId)
        {
            throw new ArgumentException("You can't update last time user read chat with a different user id!");
        }

        // Check if another RoomId is provided
        if (originalEntity.RoomId != entity.RoomId)
        {
            throw new ArgumentException("You can't update last time user read chat with a different room id!");
        }

        if (originalEntity.Room!.IsExpired())
        {
            throw new RoomExpiredException();
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(entity).State = EntityState.Modified;
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
}