using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class RoomRepository : IRoomRepository
{
    private ColirDbContext _dbContext;
    
    public RoomRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Room> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Room entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Room entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(Room entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAllExpiredAsync()
    {
        throw new NotImplementedException();
    }
}