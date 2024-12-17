using DAL.Entities;

namespace DAL.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    IRoomFileManager RoomFileManager { get; }
    Task<Room> GetByGuidAsync(string guid, string[]? overriddenIncludes = default);
    Task DeleteAllExpiredAsync();
}