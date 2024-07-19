using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByHexIdAsync(int hexId);
    
    Task<User> GetByGithudIdAsync(string githubId);

    Task<bool> ExistsAsync(int hexId);
}