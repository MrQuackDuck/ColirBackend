using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByHexIdAsync(int hexId, string[]? overriddenIncludes = default);

    Task<User> GetByGithudIdAsync(string githubId, string[]? overriddenIncludes = default);

    Task<User> GetByGoogleIdAsync(string googleId, string[]? overriddenIncludes = default);

    Task<bool> ExistsAsync(int hexId);
}