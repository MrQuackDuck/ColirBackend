using DAL.Entities;

namespace DAL.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room> GetByGuidAsync(string guid);
    Task DeleteAllExpiredAsync();
}