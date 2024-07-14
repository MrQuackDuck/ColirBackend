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
    
    public async Task<IEnumerable<UserSettings>> GetAllAsync()
    {
        return await _dbContext.UserSettings
            .Include(nameof(UserSettings.User))
            .ToListAsync();
    }

    public async Task<UserSettings> GetByIdAsync(long id)
    {
        try
        {
            return await _dbContext.UserSettings
                .Include(nameof(UserSettings.User))
                .FirstAsync(s => s.Id == id);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }
    
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
                .Include(nameof(UserSettings.User))
                .FirstAsync(s => s.User.HexId == hexId);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

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

    public void Delete(UserSettings settings)
    {
        if (!_dbContext.UserSettings.Any(s => s.Id == settings.Id))
        {
            throw new NotFoundException();
        }

        _dbContext.UserSettings.Remove(settings);
    }

    public async Task DeleteByIdAsync(long id)
    {
        try
        {
            var target = _dbContext.UserSettings.First(s => s.Id == id);
            _dbContext.UserSettings.Remove(target);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

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
            throw new ArgumentException("Settings with the same User ID exist already!");
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(settings).State = EntityState.Modified;
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