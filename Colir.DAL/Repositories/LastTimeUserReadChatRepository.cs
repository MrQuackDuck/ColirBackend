using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class LastTimeUserReadChatRepository : ILastTimeUserReadChatRepository
{
    private ColirDbContext _dbContext;
    
    public LastTimeUserReadChatRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<LastTimeUserReadChat>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<LastTimeUserReadChat> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(LastTimeUserReadChat entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(LastTimeUserReadChat entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(LastTimeUserReadChat entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public async Task<LastTimeUserReadChat> GetAsync(long userId, long roomId)
    {
        throw new NotImplementedException();
    }
}