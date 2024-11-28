using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

#nullable enable

public class RoomRepository : IRoomRepository
{
    public IRoomFileManager RoomFileManager { get; }

    private readonly ColirDbContext _dbContext;
    private readonly IConfiguration _config;

    public RoomRepository(ColirDbContext dbContext, IConfiguration config, IRoomFileManager roomFileManager)
    {
        _dbContext = dbContext;
        _config = config;
        RoomFileManager = roomFileManager;
    }

    /// <summary>
    /// Gets all rooms
    /// </summary>
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .ToListAsync();
    }

    /// <summary>
    /// Gets a room by its id
    /// </summary>
    /// <param name="id">Id of a room</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    public async Task<Room> GetByIdAsync(long id)
    {
        return await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .FirstOrDefaultAsync(r => r.Id == id) ?? throw new RoomNotFoundException();
    }

    /// <summary>
    /// Gets a room by its GUID
    /// </summary>
    /// <param name="guid">GUID of a room</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found</exception>
    public async Task<Room> GetByGuidAsync(string guid)
    {
        return await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .FirstOrDefaultAsync(r => r.Guid == guid) ?? throw new RoomNotFoundException();
    }

    /// <summary>
    /// Adds a room to the DB
    /// </summary>
    /// <param name="room">The room to add</param>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when the name for the room is too long</exception>
    /// <exception cref="UserNotFoundException">Thrown when the provided owner wasn't found by id</exception>
    public async Task AddAsync(Room room)
    {
        // Check for provided date
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // Check for min name length
        var minRoomNameLength = int.Parse(_config["AppSettings:MinRoomNameLength"]!);

        if (room.Name.Length < minRoomNameLength)
        {
            throw new StringTooShortException();
        }

        // Check for max name length
        var maxRoomNameLength = int.Parse(_config["AppSettings:MaxRoomNameLength"]!);

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
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found in the DB</exception>
    public void Delete(Room room)
    {
        var target = _dbContext.Rooms.FirstOrDefault(r => r.Id == room.Id) ?? throw new RoomNotFoundException();

        _dbContext.UsersToRooms.RemoveRange(_dbContext.UsersToRooms.Where(userToRoom => userToRoom.RoomId == room.Id));
        _dbContext.Rooms.Remove(target);
        var messagesToDelete = _dbContext.Messages.Where(m => m.RoomId == room.Id);
        _dbContext.Messages.RemoveRange(messagesToDelete);
        var attachmentsToDelete = _dbContext.Attachments.Where(a => messagesToDelete.Any(m => m.Id == a.MessageId));
        _dbContext.Attachments.RemoveRange(attachmentsToDelete);

        RoomFileManager.DeleteAllFiles(room.Guid);
    }

    /// <summary>
    /// Deletes the room by id
    /// </summary>
    /// <param name="id">The id of the room to delete</param>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by the provided id in the DB</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id) ?? throw new RoomNotFoundException();

        _dbContext.UsersToRooms.RemoveRange(_dbContext.UsersToRooms.Where(userToRoom => userToRoom.RoomId == id));
        _dbContext.Rooms.Remove(target);
        var messagesToDelete = _dbContext.Messages.Where(m => m.RoomId == id);
        _dbContext.Messages.RemoveRange(messagesToDelete);
        var attachmentsToDelete = _dbContext.Attachments.Where(a => messagesToDelete.Any(m => m.Id == a.MessageId));
        _dbContext.Attachments.RemoveRange(attachmentsToDelete);

        RoomFileManager.DeleteAllFiles(target.Guid);
    }

    /// <summary>
    /// Deletes all expired rooms
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when no expired rooms are found</exception>
    public void DeleteAllExpired()
    {
        var expiredRooms = _dbContext.Rooms.Where(r => r.ExpiryDate < DateTime.Now);

        if (expiredRooms.Count() == 0)
        {
            throw new RoomNotFoundException();
        }

        foreach (var room in expiredRooms)
        {
            Delete(room);
        }
    }

    /// <summary>
    /// Updates the room
    /// </summary>
    /// <param name="room">The room to update</param>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when the name for the room is too long</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room wasn't found by its id</exception>
    public void Update(Room room)
    {
        // Check for provided date
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // Check for min name length
        var minRoomNameLength = int.Parse(_config["AppSettings:MinRoomNameLength"]!);

        if (room.Name.Length < minRoomNameLength)
        {
            throw new StringTooShortException();
        }

        // Check for max name length
        var maxRoomNameLength = int.Parse(_config["AppSettings:MaxRoomNameLength"]!);

        if (room.Name.Length > maxRoomNameLength)
        {
            throw new StringTooLongException();
        }

        var originalEntity = _dbContext.Rooms
            .Include(nameof(Room.JoinedUsers))
            .FirstOrDefault(r => r.Id == room.Id) ?? throw new RoomNotFoundException();

        // Check if joined users list has changed to delete users who are no longer in the room
        if ((List<User>?)room.JoinedUsers != null)
        {
            for (int i = originalEntity.JoinedUsers.Count - 1; i >= 0; i--)
            {
                var user = originalEntity.JoinedUsers[i];

                if (!room.JoinedUsers.Any(u => u.Id == user.Id))
                {
                    originalEntity.JoinedUsers.Remove(user);
                }
            }
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(room).State = EntityState.Modified;
    }

    /// <summary>
    /// Saves changes
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