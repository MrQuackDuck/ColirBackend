using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class RoomRepository : IRoomRepository
{
    private ColirDbContext _dbContext;
    private IConfiguration _config;
    
    public RoomRepository(ColirDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return _dbContext.Rooms.ToList();
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

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAllExpiredAsync()
    {
        throw new NotImplementedException();
    }
}