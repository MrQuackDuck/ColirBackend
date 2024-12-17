using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Extensions;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly ColirDbContext _dbContext;

    public UserSettingsRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all users' settings
    /// </summary>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    public async Task<IEnumerable<UserSettings>> GetAllAsync(string[]? overriddenIncludes = default)
    {
        return await _dbContext.UserSettings
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(UserSettings.User)])
            .ToListAsync();
    }

    /// <summary>
    /// Gets user settings by their id
    /// </summary>
    /// <param name="id">Id of user settings</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="NotFoundException">Thrown when not found by id</exception>
    public async Task<UserSettings> GetByIdAsync(long id, string[]? overriddenIncludes = default)
    {
        return await _dbContext.UserSettings
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(UserSettings.User)])
            .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException();
    }

    /// <summary>
    /// Gets user settings by user's hex id
    /// </summary>
    /// <param name="hexId">Hex Id of the user</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="ArgumentException">Thrown when an invalid Hex Id is provided</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    /// <exception cref="NotFoundException">Thrown when user settings weren't found</exception>
    public async Task<UserSettings> GetByUserHexIdAsync(int hexId, string[]? overriddenIncludes = default)
    {
        if (hexId < 0 || hexId > 16_777_216)
        {
            throw new ArgumentException("Invalid Hex ID provided!");
        }

        if (!await _dbContext.Users.AnyAsync(u => u.HexId == hexId))
        {
            throw new UserNotFoundException();
        }

        return await _dbContext.UserSettings
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(UserSettings.User)])
            .FirstOrDefaultAsync(s => s.User.HexId == hexId) ?? throw new NotFoundException();
    }

    /// <summary>
    /// Adds user settings to the DB
    /// </summary>
    /// <param name="settings">User settings to add</param>
    /// <exception cref="ArgumentException">Thrown when settings for this user already exist</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    public async Task AddAsync(UserSettings settings)
    {
        if (await _dbContext.UserSettings.AnyAsync(s => s.UserId == settings.UserId))
        {
            throw new ArgumentException("Settings for this user already exist!");
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
    public async Task DeleteAsync(UserSettings settings)
    {
        var target = await _dbContext.UserSettings.FindAsync(settings.Id) ?? throw new NotFoundException();

        _dbContext.UserSettings.Remove(target);
    }

    /// <summary>
    /// Deletes user settings by id
    /// </summary>
    /// <param name="id">Id of user settings to delete</param>
    /// <exception cref="NotFoundException">Thrown when user settings weren't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.UserSettings.FindAsync(id) ?? throw new NotFoundException();

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

        // Check if another UserId is provided
        if (originalEntity.UserId != settings.UserId)
        {
            throw new ArgumentException("You can't update settings with a different user id!");
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(settings).State = EntityState.Modified;
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