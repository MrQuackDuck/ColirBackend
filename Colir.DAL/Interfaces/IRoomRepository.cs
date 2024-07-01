using DAL.Entities;

namespace DAL.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task DeleteAllExpiredAsync();

    // Delete method here will delete all messages etc.
}