﻿using Colir.Exceptions;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class RoomRepository : IRoomRepository
{
    private ColirDbContext _dbContext;
    private IConfiguration _config;
    
    public RoomRepository(ColirDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    /// <summary>
    /// Gets all rooms
    /// </summary>
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _dbContext.Rooms
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .ToListAsync();
    }

    /// <summary>
    /// Gets a room by its id
    /// </summary>
    /// <param name="id">Id of</param>
    /// <exception cref="NotFoundException">Thrown when room wasn't found</exception>
    public async Task<Room> GetByIdAsync(long id)
    {
        try
        {
            return await _dbContext.Rooms
                .Include(nameof(Room.Owner))
                .Include(nameof(Room.JoinedUsers))
                .FirstAsync(r => r.Id == id);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

    /// <summary>
    /// Adds a room to DB
    /// </summary>
    /// <param name="room">The room to add</param>
    /// <exception cref="RoomExpiredException">Thrown when room's expiry date is earlier than now</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room it too short</exception>
    /// <exception cref="StringTooLongException"Thrown when the name for the room it too long></exception>
    /// <exception cref="UserNotFoundException">Thrown when provided owner wasn't found by id</exception>
    public async Task AddAsync(Room room)
    {
        // Check for provided date
        if (room.ExpiryDate < DateTime.Now)
        {
            throw new RoomExpiredException();
        }

        // Check for min name length
        var minRoomNameLength = int.Parse(_config["MinRoomNameLength"]!);

        if (room.Name.Length < minRoomNameLength)
        {
            throw new StringTooShortException();
        }
        
        // Check for max name length
        var maxRoomNameLength = int.Parse(_config["MaxRoomNameLength"]!);

        if (room.Name.Length > maxRoomNameLength)
        {
            throw new StringTooLongException();
        }

        // Check if provided owner exists
        var owner = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == room.OwnerId);
        if (owner is null)
        {
            throw new UserNotFoundException();
        }
        
        await _dbContext.Rooms.AddAsync(room);
    }

    /// <summary>
    /// Deletes the room
    /// </summary>
    /// <param name="room">The room to delete</param>
    /// <exception cref="NotFoundException">Thrown when the room wasn't found in DB</exception>
    public void Delete(Room room)
    {
        // Check if room exists
        if (_dbContext.Rooms.FirstOrDefault(r => r.Id == room.Id) is null)
        {
            throw new NotFoundException();
        }
        
        _dbContext.Rooms.Remove(room);
        _dbContext.Messages.RemoveRange(_dbContext.Messages.Where(m => m.RoomId == room.Id));
    }

    /// <summary>
    /// Deletes the room by id
    /// </summary>
    /// <param name="id">The id of the room to delete</param>
    /// <exception cref="NotFoundException">Thrown when the room wasn't found by provided id in DB</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await GetByIdAsync(id);
        _dbContext.Rooms.Remove(target);
    }

    /// <summary>
    /// Deletes all expired rooms
    /// </summary>
    /// <exception cref="NotFoundException">Thrown when no expired rooms found</exception>
    public async Task DeleteAllExpiredAsync()
    {
        var expiredRooms = _dbContext.Rooms.Where(r => r.ExpiryDate < DateTime.Now);

        if (expiredRooms.Count() == 0)
        {
            throw new NotFoundException();
        }
        
        _dbContext.RemoveRange(expiredRooms);
    }

    /// <summary>
    /// Updates the room
    /// </summary>
    /// <param name="room">The room to update</param>
    /// <exception cref="RoomExpiredException">Thrown when room's expiry date is earlier than now</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room it too short</exception>
    /// <exception cref="StringTooLongException">Thrown when the name for the room it too long</exception>
    /// <exception cref="NotFoundException">Thrown when the room wasn't found by its id</exception>
    public void Update(Room room)
    {
        // Check for provided date
        if (room.ExpiryDate < DateTime.Now)
        {
            throw new RoomExpiredException();
        }

        // Check for min name length
        var minRoomNameLength = int.Parse(_config["MinRoomNameLength"]!);

        if (room.Name.Length < minRoomNameLength)
        {
            throw new StringTooShortException();
        }
        
        // Check for max name length
        var maxRoomNameLength = int.Parse(_config["MaxRoomNameLength"]!);

        if (room.Name.Length > maxRoomNameLength)
        {
            throw new StringTooLongException();
        }

        var originalEntity = _dbContext.Rooms.FirstOrDefault(r => r.Id == room.Id);
        if (originalEntity != null)
        {
            _dbContext.Entry(originalEntity).State = EntityState.Detached;
        }
        else
        {
            throw new NotFoundException();
        }

        _dbContext.Entry(room).State = EntityState.Modified;
    }

    /// <summary>
    /// Saves the changes
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