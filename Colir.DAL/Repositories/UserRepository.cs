using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ColirDbContext _dbContext;
    private readonly IConfiguration _configuration;
    
    public UserRepository(ColirDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
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

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetByHexIdAsync(string hexId)
    {
        throw new NotImplementedException();
    }
}