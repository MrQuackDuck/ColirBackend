using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByHexIdAsync(long hexId);
    
    Task<User> GetByGithudIdAsync(string githubId);

    Task<bool> Exists(long hexId);
}