using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Extensions;
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
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    public async Task<IEnumerable<User>> GetAllAsync(string[]? overriddenIncludes = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .ToListAsync();
    }

    /// <summary>
    /// Gets the user by their id
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found by the provided id</exception>
    public async Task<User> GetByIdAsync(long id, string[]? overriddenIncludes = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException();
    }

    /// <summary>
    /// Gets the user by their hex id
    /// </summary>
    /// <param name="hexId">Hex Id of the user</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="ArgumentException">Thrown when an invalid hex id is provided</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found by the provided hex id</exception>
    public async Task<User> GetByHexIdAsync(int hexId, string[]? overriddenIncludes = default)
    {
        if (hexId < 0 || hexId > 16_777_216)
        {
            throw new ArgumentException("Invalid Hex ID provided!");
        }

        return await _dbContext.Users
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.HexId == hexId) ?? throw new UserNotFoundException();
    }

    /// <summary>
    /// Gets the user by their GitHub Id
    /// </summary>
    /// <param name="githubId">GitHub Id of the user</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found by the provided GitHub id</exception>
    public async Task<User> GetByGithudIdAsync(string githubId, string[]? overriddenIncludes = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.GitHubId == githubId) ?? throw new UserNotFoundException();
    }

    /// <summary>
    /// Gets the user by their Google Id
    /// </summary>
    /// <param name="googleId">Google Id of the user</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found by the provided Google id</exception>
    public async Task<User> GetByGoogleIdAsync(string googleId, string[]? overriddenIncludes = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? GetDefaultIncludes())
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.GoogleId == googleId) ?? throw new UserNotFoundException();
    }

    /// <summary>
    /// Determines if a user with the provided Hex Id exists already
    /// </summary>
    public async Task<bool> ExistsAsync(int hexId)
    {
        return await _dbContext.Users.AnyAsync(u => u.HexId == hexId);
    }

    /// <summary>
    /// Adds a user to the DB
    /// </summary>
    /// <param name="user">The user to add</param>
    /// <exception cref="ArgumentException">Thrown when an invalid hex id is provided</exception>
    /// <exception cref="ArgumentException">Thrown when a user with the same hex id already exists</exception>
    /// <exception cref="StringTooShortException">Thrown when the username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when the username is too long</exception>
    /// <exception cref="RoomExpiredException">Thrown when one of the JoinedRooms is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when one of the JoinedRooms wasn't found</exception>
    public async Task AddAsync(User user)
    {
        if (user.HexId < 0 || user.HexId > 16_777_216)
        {
            throw new ArgumentException("Invalid Hex ID provided!");
        }

        // Check if a user with the same Hex ID exists
        if (await _dbContext.Users.CountAsync(u => u.HexId == user.HexId) > 0)
        {
            throw new ArgumentException("A user with the same Hex ID already exists!");
        }

        var minUsernameLength = int.Parse(_config["AppSettings:MinUsernameLength"]!);

        if (user.Username.Length < minUsernameLength)
        {
            throw new StringTooShortException();
        }

        var maxUsernameLength = int.Parse(_config["AppSettings:MaxUsernameLength"]!);

        if (user.Username.Length > maxUsernameLength)
        {
            throw new StringTooLongException();
        }

        foreach (var room in user.JoinedRooms)
        {
            if (room.IsExpired()) throw new RoomExpiredException();
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
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found in the DB</exception>
    public async Task DeleteAsync(User user)
    {
        var target = await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .FirstOrDefaultAsync(u => u.Id == user.Id) ?? throw new UserNotFoundException();

        _dbContext.UsersToRooms.RemoveRange(_dbContext.UsersToRooms.Where(userToRoom => userToRoom.UserId == user.Id));
        _dbContext.Users.Remove(target);
        _dbContext.UserSettings.Remove(target.UserSettings);
        _dbContext.UserStatistics.Remove(target.UserStatistics);
    }

    /// <summary>
    /// Deletes the user by id
    /// </summary>
    /// <param name="id">The id of the user to delete</param>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found by the provided id in the DB</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException();

        _dbContext.UsersToRooms.RemoveRange(_dbContext.UsersToRooms.Where(userToRoom => userToRoom.UserId == id));
        _dbContext.Users.Remove(target);
        _dbContext.UserSettings.Remove(target.UserSettings);
        _dbContext.UserStatistics.Remove(target.UserStatistics);
    }

    /// <summary>
    /// Updates the user
    /// </summary>
    /// <param name="user">The user to update</param>
    /// <exception cref="StringTooShortException">Thrown when the username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when the username is too long</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found by their id in the DB</exception>
    /// <exception cref="ArgumentException">Thrown when a user with the same hex id already exists</exception>
    public void Update(User user)
    {
        var minUsernameLength = int.Parse(_config["AppSettings:MinUsernameLength"]!);

        if (user.Username.Length < minUsernameLength)
        {
            throw new StringTooShortException();
        }

        var maxUsernameLength = int.Parse(_config["AppSettings:MaxUsernameLength"]!);

        if (user.Username.Length > maxUsernameLength)
        {
            throw new StringTooLongException();
        }

        var originalEntity = _dbContext.Users
            .Include(nameof(User.JoinedRooms))
            .AsSplitQuery()
            .FirstOrDefault(u => u.Id == user.Id) ?? throw new UserNotFoundException();

        // Check if a user with the same Hex ID exists
        if (originalEntity.HexId != user.HexId && _dbContext.Users.Count(u => u.HexId == user.HexId) >= 1)
        {
            throw new ArgumentException("A user with the same Hex ID already exists!");
        }

        // Check if joined rooms list has changed to delete rooms where the user is not present
        if ((List<Room>?)originalEntity.JoinedRooms != null)
        {
            for (int i = originalEntity.JoinedRooms.Count - 1; i >= 0; i--)
            {
                var room = originalEntity.JoinedRooms[i];

                if (!user.JoinedRooms.Any(r => r.Id == room.Id))
                {
                    originalEntity.JoinedRooms.Remove(room);
                }
            }
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(user).State = EntityState.Modified;
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

    private static string[] GetDefaultIncludes()
    {
        return new[]
        {
            nameof(User.UserStatistics),
            nameof(User.UserSettings),
            nameof(User.JoinedRooms)
        };
    }
}