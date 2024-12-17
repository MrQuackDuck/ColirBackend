using DAL.Entities;

namespace DAL.Interfaces;

public interface ILastTimeUserReadChatRepository : IRepository<LastTimeUserReadChat>
{
    Task<LastTimeUserReadChat> GetAsync(long userId, long roomId, string[]? overriddenIncludes = default);
}