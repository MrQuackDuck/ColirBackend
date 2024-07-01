using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class UserStatisticsRepository : IUserStatisticsRepository
{
    private ColirDbContext _dbContext;
    
    public UserStatisticsRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserStatistics>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UserStatistics> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(UserStatistics entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(UserStatistics entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(UserStatistics entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public async Task<UserStatistics> GetByUserHexIdAsync(string hexId)
    {
        throw new NotImplementedException();
    }
}