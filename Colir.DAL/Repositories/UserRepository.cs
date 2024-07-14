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

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .ToListAsync();
    }

    public async Task<User> GetByIdAsync(long id)
    {
        return await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException();
    }
    
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

    public void Delete(User user)
    {
        if (!_dbContext.Users.Any(u => u.Id == user.Id)) throw new NotFoundException();
        
        _dbContext.Users.Remove(user);
    }

    public async Task DeleteByIdAsync(long id)
    {
        try
        {
            var target = await _dbContext.Users.FirstAsync(u => u.Id == id);
            _dbContext.Users.Remove(target);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

    public void Update(User user)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}