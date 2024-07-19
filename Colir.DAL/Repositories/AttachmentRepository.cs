using Colir.Exceptions.NotFound;
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
    
    /// <summary>
    /// Gets all attachments
    /// </summary>
    public async Task<IEnumerable<Attachment>> GetAllAsync()
    {
        return await _dbContext.Attachments
            .AsNoTracking()
            .Include(nameof(Attachment.Message))
            .ToListAsync();
    }

    /// <summary>
    /// Gets the attachment by id
    /// </summary>
    /// <param name="id">Id of attachment to get</param>
    /// <exception cref="AttachmentNotFoundException">Thrown when the attachment wasn't found</exception>
    public async Task<Attachment> GetByIdAsync(long id)
    {
        return await _dbContext.Attachments
            .AsNoTracking()
            .Include(nameof(Attachment.Message))
            .FirstOrDefaultAsync(a => a.Id == id) ?? throw new AttachmentNotFoundException();
    }

    /// <summary>
    /// Adds the attachment to DB
    /// </summary>
    /// <param name="attachment">An attachment to add</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    public async Task AddAsync(Attachment attachment)
    {
        if (!await _dbContext.Messages.AnyAsync(m => m.Id == attachment.MessageId))
        {
            throw new MessageNotFoundException();
        }
        
        await _dbContext.Attachments.AddAsync(attachment);
    }

    /// <summary>
    /// Deletes the attachment
    /// </summary>
    /// <param name="attachment">An attachment to delete</param>
    /// <exception cref="NotFoundException">Thrown when the attachment wasn't found</exception>
    public void Delete(Attachment attachment)
    {
        var target = _dbContext.Attachments.FirstOrDefault(a => a.Id == attachment.Id) ?? throw new AttachmentNotFoundException();
        _dbContext.Attachments.Remove(target);
    }

    /// <summary>
    /// Deletes the attachment by id
    /// </summary>
    /// <param name="id">Id of the attachment to delete</param>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Attachments.FirstOrDefaultAsync(a => a.Id == id) ?? throw new AttachmentNotFoundException();
        _dbContext.Attachments.Remove(target);
    }

    /// <summary>
    /// Updates the attachment
    /// </summary>
    /// <param name="attachment">An attachment to update</param>
    /// <exception cref="AttachmentNotFoundException">Thrown when non-existing attachment provided</exception>
    public void Update(Attachment attachment)
    {
        var originalEntity = _dbContext.Attachments
                                 .Include(nameof(Attachment.Message))
                                 .FirstOrDefault(a => a.Id == attachment.Id)
                             ?? throw new AttachmentNotFoundException();

        _dbContext.Entry(originalEntity).State = EntityState.Detached;
        _dbContext.Entry(attachment).State = EntityState.Modified;
    }

    /// <summary>
    /// Saves the changes to DB
    /// </summary>
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Saves the changes to DB asynchronously
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}