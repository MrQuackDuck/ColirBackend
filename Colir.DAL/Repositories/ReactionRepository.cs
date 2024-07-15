using Colir.Exceptions;
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
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets the reaction by id
    /// </summary>
    /// <param name="id">Id of reaction to get</param>
    /// <exception cref="NotFoundException">Thrown when the reaction wasn't found by provided id</exception>
    public async Task<Reaction> GetByIdAsync(long id)
    {
        try
        {
            return await _dbContext.Reactions
                .Include(nameof(Reaction.Author))
                .Include(nameof(Reaction.Message))
                .FirstAsync(r => r.Id == id);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException();
        }
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
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .Where(r => r.MessageId == messageId)
            .ToListAsync();
    }

    /// <summary>
    /// Adds the reaction to DB
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
    /// <exception cref="NotFoundException">Thrown when the reaction wasn't found</exception>
    public void Delete(Reaction reaction)
    {
        if (!_dbContext.Reactions.Any(r => r.Id == reaction.Id))
        {
            throw new NotFoundException();
        }

        _dbContext.Reactions.Remove(reaction);
    }

    /// <summary>
    /// Deletes the reaction by id
    /// </summary>
    /// <param name="id">Id of the reaction to delete</param>
    /// <exception cref="NotFoundException">Thrown when the reaction wasn't found</exception>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await GetByIdAsync(id);
        _dbContext.Reactions.Remove(target);
    }

    /// <summary>
    /// Updates the raection
    /// </summary>
    /// <param name="reaction">The reaction to update</param>
    /// <exception cref="NotFoundException">Thrown when the reaction wasn't found</exception>
    public void Update(Reaction reaction)
    {
        var originalEntity = _dbContext.Reactions.FirstOrDefault(r => r.Id == reaction.Id);
        
        if (originalEntity == null)
        {
            throw new NotFoundException();
        }
        
        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(reaction).State = EntityState.Modified;
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