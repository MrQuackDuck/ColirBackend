using Colir.Exceptions;
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

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _dbContext.Rooms
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .ToListAsync();
    }

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

    public void Delete(Room room)
    {
        // Check if room exists
        if (_dbContext.Rooms.FirstOrDefault(r => r.Id == room.Id) is null)
        {
            throw new NotFoundException();
        }
        
        _dbContext.Rooms.Remove(room);
    }

    public async Task DeleteByIdAsync(long id)
    {
        try
        {
            var target = await _dbContext.Rooms.FirstAsync(r => r.Id == id);
            _dbContext.Rooms.Remove(target);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

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

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllExpiredAsync()
    {
        var expiredRooms = _dbContext.Rooms.Where(r => r.ExpiryDate < DateTime.Now);

        if (expiredRooms.Count() == 0)
        {
            throw new NotFoundException();
        }
        
        _dbContext.RemoveRange(expiredRooms);
    }
}