using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByHexIdAsync(long hexId);
}