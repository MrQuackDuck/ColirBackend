using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class UserRepository : IUserRepository
{
    private ColirDbContext _dbContext;
    
    public UserRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(User entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetByHexIdAsync(string hexId)
    {
        throw new NotImplementedException();
    }
}