using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private ColirDbContext _dbContext;
    
    public AttachmentRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Attachment>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Attachment> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Attachment entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Attachment entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Update(Attachment entity)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}