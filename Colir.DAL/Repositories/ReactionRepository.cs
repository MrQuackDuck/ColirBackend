﻿using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class ReactionRepository : IReactionRepository
{
    private ColirDbContext _dbContext;

    public ReactionRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all reactions
    /// </summary>
    public async Task<IEnumerable<Reaction>> GetAllAsync()
    {
        return await _dbContext.Reactions
            .AsNoTracking()
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .ToListAsync();
    }

    /// <summary>
    /// Gets the reaction by id
    /// </summary>
    /// <param name="id">Id of reaction to get</param>
    /// <exception cref="ReactionNotFoundException">Thrown when the reaction wasn't found by provided id</exception>
    public async Task<Reaction> GetByIdAsync(long id)
    {
        return await _dbContext.Reactions
            .AsNoTracking()
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .FirstOrDefaultAsync(r => r.Id == id) ?? throw new ReactionNotFoundException();
    }

    /// <summary>
    /// Gets all reactions on certain message
    /// </summary>
    /// <param name="messageId">Id of message to find reactions on</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    public async Task<List<Reaction>> GetReactionsOnMessage(long messageId)
    {
        if (!await _dbContext.Messages.AnyAsync(m => m.Id == messageId))
        {
            throw new MessageNotFoundException();
        }

        return await _dbContext.Reactions
            .AsNoTracking()
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .Where(r => r.MessageId == messageId)
            .ToListAsync();
    }

    /// <summary>
    /// Adds the reaction to the DB
    /// </summary>
    /// <param name="reaction">Reaction to add</param>
    /// <exception cref="UserNotFoundException">Thrown when the author wasn't found by id</exception>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found by id</exception>
    public async Task AddAsync(Reaction reaction)
    {
        if (!await _dbContext.Users.AnyAsync(u => u.Id == reaction.AuthorId))
        {
            throw new UserNotFoundException();
        }

        if (!await _dbContext.Messages.AnyAsync(m => m.Id == reaction.MessageId))
        {
            throw new MessageNotFoundException();
        }

        await _dbContext.AddAsync(reaction);
    }

    /// <summary>
    /// Deletes the reaction
    /// </summary>
    /// <param name="reaction">The reaction to delete</param>
    /// <exception cref="ReactionNotFoundException">Thrown when the reaction wasn't found</exception>
    public void Delete(Reaction reaction)
    {
        var target = _dbContext.Reactions.FirstOrDefault(r => r.Id == reaction.Id) ?? throw new ReactionNotFoundException();
        _dbContext.Reactions.Remove(target);
    }

    /// <summary>
    /// Deletes the reaction by id
    /// </summary>
    /// <param name="id">Id of the reaction to delete</param>
    /// <exception cref="ReactionNotFoundException">Thrown when the reaction wasn't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Reactions.FirstOrDefaultAsync(r => r.Id == id) ?? throw new ReactionNotFoundException();
        _dbContext.Reactions.Remove(target);
    }

    /// <summary>
    /// Updates the raection
    /// </summary>
    /// <param name="reaction">The reaction to update</param>
    /// <exception cref="ReactionNotFoundException">Thrown when the reaction wasn't found</exception>
    public void Update(Reaction reaction)
    {
        var originalEntity = _dbContext.Reactions.FirstOrDefault(r => r.Id == reaction.Id);

        if (originalEntity == null)
        {
            throw new ReactionNotFoundException();
        }

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(reaction).State = EntityState.Modified;
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