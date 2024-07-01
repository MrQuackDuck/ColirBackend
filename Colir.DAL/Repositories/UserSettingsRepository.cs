using DAL.Entities;
using DAL.Interfaces;

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
        throw new NotImplementedException();
    }

    public async Task<UserSettings> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(UserSettings entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(UserSettings entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(UserSettings entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public async Task<UserSettings> GetByUserHexIdAsync(string hexId)
    {
        throw new NotImplementedException();
    }
}