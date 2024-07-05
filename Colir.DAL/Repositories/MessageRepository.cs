using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class MessageRepository : IMessageRepository
{
    private ColirDbContext _dbContext;
    
    public MessageRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Message> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Message entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Message entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(Message entity)
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

    public async Task<List<Message>> GetLastMessages(string roomGuid, int count, int skip)
    {
        throw new NotImplementedException();
    }
}