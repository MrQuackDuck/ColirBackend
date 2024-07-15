﻿using Colir.Exceptions;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserStatisticsRepository : IUserStatisticsRepository
{
    private ColirDbContext _dbContext;
    
    public UserStatisticsRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all users' statistics
    /// </summary>
    public async Task<IEnumerable<UserStatistics>> GetAllAsync()
    {
        return await _dbContext.UserStatistics
            .Include(nameof(UserStatistics.User))
            .ToListAsync();
    }

    /// <summary>
    /// Gets user statistics by its id
    /// </summary>
    /// <param name="id">Id of user statistics</param>
    /// <exception cref="NotFoundException">Thrown when not found by id</exception>
    public async Task<UserStatistics> GetByIdAsync(long id)
    {
        try
        {
            return await _dbContext.UserStatistics
                .Include(nameof(UserStatistics.User))
                .FirstAsync(s => s.Id == id);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }
    
    /// <summary>
    /// Gets user statistics by user's hex id
    /// </summary>
    /// <param name="hexId">Hex Id of the user</param>
    /// <exception cref="ArgumentException">Thrown when invalid Hex Id provided</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    /// <exception cref="NotFoundException">Thrown when user statistics wasn't found</exception>
    public async Task<UserStatistics> GetByUserHexIdAsync(long hexId)
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
            return await _dbContext.UserStatistics
                .Include(nameof(UserSettings.User))
                .FirstAsync(s => s.User.HexId == hexId);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
    }

    /// <summary>
    /// Adds user statistics to DB
    /// </summary>
    /// <param name="statistics">User statistics to add</param>
    /// <exception cref="ArgumentException">Thrown when statistics for this user already exist</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    public async Task AddAsync(UserStatistics statistics)
    {
        if (await _dbContext.UserSettings.AnyAsync(s => s.UserId == statistics.UserId))
        {
            throw new ArgumentException();
        }

        if (!await _dbContext.Users.AnyAsync(u => u.Id == statistics.UserId))
        {
            throw new UserNotFoundException();
        }
        
        await _dbContext.AddAsync(statistics);
    }

    /// <summary>
    /// Deletes user statistics
    /// </summary>
    /// <param name="statistics">User statistics to delete</param>
    /// <exception cref="NotFoundException">Thrown when user statistics wasn't found</exception>
    public void Delete(UserStatistics statistics)
    {
        if (!_dbContext.UserStatistics.Any(s => s.Id == statistics.Id))
        {
            throw new NotFoundException();
        }

        _dbContext.UserStatistics.Remove(statistics);
    }

    /// <summary>
    /// Deletes user statistics by id
    /// </summary>
    /// <param name="id">Id of user statistics to delete</param>
    /// <exception cref="NotFoundException">Thrown when user statistics weren't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await GetByIdAsync(id);
        _dbContext.UserStatistics.Remove(target);
    }

    /// <summary>
    /// Updates user statistics
    /// </summary>
    /// <param name="statistics">User statistics to update</param>
    /// <exception cref="NotFoundException">Thrown when user statistics weren't found</exception>
    /// <exception cref="ArgumentException">Thrown when a client tries to update user statistics with another user id</exception>
    public void Update(UserStatistics statistics)
    {
        var originalEntity = _dbContext.UserStatistics.FirstOrDefault(s => s.Id == statistics.Id);
        
        if (originalEntity == null)
        {
            throw new NotFoundException();
        }
        
        // Check if another UserId provided
        if (originalEntity.UserId != statistics.UserId)
        {
            throw new ArgumentException("You can't update statistics with different user id!");
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(statistics).State = EntityState.Modified;
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