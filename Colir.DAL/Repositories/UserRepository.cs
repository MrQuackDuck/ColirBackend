using Colir.Exceptions;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ColirDbContext _dbContext;
    private readonly IConfiguration _config;
    
    public UserRepository(ColirDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .ToListAsync();
    }

    /// <summary>
    /// Gets the user by its id
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <exception cref="NotFoundException">Thrown when the user wasn't found by provided id</exception>
    public async Task<User> GetByIdAsync(long id)
    {
        return await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException();
    }

    /// <summary>
    /// Gets the user by its hex id
    /// </summary>
    /// <param name="hexId">Hex id of the user</param>
    /// <exception cref="ArgumentException">Thrown when invalid hex id provided</exception>
    /// <exception cref="NotFoundException">Thrown when the user wasn't found by provided hex id</exception>
    public async Task<User> GetByHexIdAsync(long hexId)
    {
        if (hexId < 0 || hexId > 16_777_216)
        {
            throw new ArgumentException("Invalid Hex ID provided!");
        }
        
        return await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .FirstOrDefaultAsync(u => u.HexId == hexId) ?? throw new NotFoundException();
    }

    /// <summary>
    /// Adds a user to DB
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Thrown when invalid hex id provided</exception>
    /// <exception cref="ArgumentException">Thrown when the user with the same hex id exists already</exception>
    /// <exception cref="StringTooShortException">Thrown when username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when username is too long</exception>
    /// <exception cref="RoomExpiredException">Thrown when one of JoinedRooms is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when one of JoinedRooms wasn't found></exception>
    public async Task AddAsync(User user)
    {
        if (user.HexId < 0 || user.HexId > 16_777_216)
        {
            throw new ArgumentException("Invalid Hex ID provided!");
        }
        
        // Check if a user with the same Hex ID exists
        if (await _dbContext.Users.CountAsync(u => u.HexId == user.HexId) > 0)
        {
            throw new ArgumentException("User with the same Hex ID exists already!");
        }

        var minUsernameLength = int.Parse(_config["MinUsernameLength"]!);

        if (user.Username.Length < minUsernameLength)
        {
            throw new StringTooShortException();
        }
        
        var maxUsernameLength = int.Parse(_config["MaxUsernameLength"]!);
        
        if (user.Username.Length > maxUsernameLength)
        {
            throw new StringTooLongException();
        }

        foreach (var room in user.JoinedRooms)
        {
            if (room.ExpiryDate < DateTime.Now) throw new RoomExpiredException();
            if (!await _dbContext.Rooms.AnyAsync(r => room.Id == r.Id)) throw new RoomNotFoundException();
        }
        
        var userStats = new UserStatistics
        {
            User = user
        };

        var userSettings = new UserSettings
        {
            User = user,
            StatisticsEnabled = true
        };
        
        await _dbContext.Users.AddAsync(user);
        await _dbContext.UserStatistics.AddAsync(userStats);
        await _dbContext.UserSettings.AddAsync(userSettings);
    }

    /// <summary>
    /// Deletes the user
    /// </summary>
    /// <param name="user">The user to delete</param>
    /// <exception cref="NotFoundException">Thrown when the user wasn't found in DB</exception>
    public void Delete(User user)
    {
        if (!_dbContext.Users.Any(u => u.Id == user.Id)) throw new NotFoundException();
        
        _dbContext.Users.Remove(user);
        _dbContext.UserSettings.Remove(user.UserSettings);
        _dbContext.UserStatistics.Remove(user.UserStatistics);
    }
    
    /// <summary>
    /// Deletes the user by id
    /// </summary>
    /// <param name="id">The id of the user to delete</param>
    /// <exception cref="NotFoundException">Thrown when the user wasn't found by provided id in DB</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await GetByIdAsync(id);
        _dbContext.Users.Remove(target);
        _dbContext.UserSettings.Remove(target.UserSettings);
        _dbContext.UserStatistics.Remove(target.UserStatistics);
    }

    /// <summary>
    /// Updates the user
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="StringTooShortException">Thrown when username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when username is too long</exception>
    /// <exception cref="NotFoundException">Thrown when the user wasn't found by its id in DB</exception>
    /// <exception cref="ArgumentException">Thrown when the user with the same hex id exists already</exception>
    public void Update(User user)
    {
        var minUsernameLength = int.Parse(_config["MinUsernameLength"]!);

        if (user.Username.Length < minUsernameLength)
        {
            throw new StringTooShortException();
        }
        
        var maxUsernameLength = int.Parse(_config["MaxUsernameLength"]!);
        
        if (user.Username.Length > maxUsernameLength)
        {
            throw new StringTooLongException();
        }
        
        var originalEntity = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
        
        if (originalEntity == null)
        {
            throw new NotFoundException();
        }
        
        // Check if a user with the same Hex ID exists
        if (originalEntity.HexId != user.HexId && _dbContext.Users.Count(u => u.HexId == user.HexId) >= 1)
        {
            throw new ArgumentException("User with the same Hex ID exists already!");
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(user).State = EntityState.Modified;
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