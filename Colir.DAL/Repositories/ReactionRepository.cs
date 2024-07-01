using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class ReactionRepository : IReactionRepository
{
    private ColirDbContext _dbContext;
    
    public ReactionRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Reaction>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Reaction> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Reaction entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Reaction entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(Reaction entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public async Task<List<Reaction>> GetReactionsOnMessage(long messageId)
    {
        throw new NotImplementedException();
    }
}