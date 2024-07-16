using Colir.Exceptions;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private ColirDbContext _dbContext;
    
    public UserSettingsRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Gets all users' settings
    /// </summary>
    public async Task<IEnumerable<UserSettings>> GetAllAsync()
    {
        return await _dbContext.UserSettings
            .AsNoTracking()
            .Include(nameof(UserSettings.User))
            .ToListAsync();
    }

    /// <summary>
    /// Gets user setinngs by its id
    /// </summary>
    /// <param name="id">Id of user settings</param>
    /// <exception cref="NotFoundException">Thrown when not found by id</exception>
    public async Task<UserSettings> GetByIdAsync(long id)
    {
        try
        {
            return await _dbContext.UserSettings
                .AsNoTracking()
                .Include(nameof(UserSettings.User))
                .FirstAsync(s => s.Id == id);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }
    
    /// <summary>
    /// Gets user settings by user's hex id
    /// </summary>
    /// <param name="hexId">Hex Id of the user</param>
    /// <exception cref="ArgumentException">Thrown when invalid Hex Id provided</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    /// <exception cref="NotFoundException">Thrown when user settings weren't found</exception>
    public async Task<UserSettings> GetByUserHexIdAsync(long hexId)
    {
        if (hexId < 0 || hexId > 16_777_216)
        {
            throw new ArgumentException("Invalid Hex ID provided!");
        }
        
        if (!await _dbContext.Users.AnyAsync(u => u.HexId == hexId))
        {
            throw new UserNotFoundException();
        }
        
        try
        {
            return await _dbContext.UserSettings
                .AsNoTracking()
                .Include(nameof(UserSettings.User))
                .FirstAsync(s => s.User.HexId == hexId);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

    /// <summary>
    /// Adds user settings to DB
    /// </summary>
    /// <param name="settings">User settings to add</param>
    /// <exception cref="ArgumentException">Thrown when settings for this user already exist</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    public async Task AddAsync(UserSettings settings)
    {
        if (await _dbContext.UserSettings.AnyAsync(s => s.UserId == settings.UserId))
        {
            throw new ArgumentException();
        }

        if (!await _dbContext.Users.AnyAsync(u => u.Id == settings.UserId))
        {
            throw new UserNotFoundException();
        }
        
        await _dbContext.AddAsync(settings);
    }

    /// <summary>
    /// Deletes user settings
    /// </summary>
    /// <param name="settings">User settings to delete</param>
    /// <exception cref="NotFoundException">Thrown when user settings weren't found</exception>
    public void Delete(UserSettings settings)
    {
        var target = _dbContext.UserSettings.FirstOrDefault(s => s.Id == settings.Id) ?? throw new NotFoundException();
        
        _dbContext.UserSettings.Remove(target);
    }

    /// <summary>
    /// Deletes user settings by id
    /// </summary>
    /// <param name="id">Id of user settings to delete</param>
    /// <exception cref="NotFoundException">Thrown when user settings weren't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException();
        
        _dbContext.UserSettings.Remove(target);
    }

    /// <summary>
    /// Updates user settings
    /// </summary>
    /// <param name="settings">User settings to update</param>
    /// <exception cref="NotFoundException">Thrown when user settings weren't found</exception>
    /// <exception cref="ArgumentException">Thrown when a client tries to update user settings with another user id</exception>
    public void Update(UserSettings settings)
    {
        var originalEntity = _dbContext.UserSettings.FirstOrDefault(s => s.Id == settings.Id);
        
        if (originalEntity == null)
        {
            throw new NotFoundException();
        }
        
        // Check if another UserId provided
        if (originalEntity.UserId != settings.UserId)
        {
            throw new ArgumentException("You can't update settings with different user id!");
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(settings).State = EntityState.Modified;
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