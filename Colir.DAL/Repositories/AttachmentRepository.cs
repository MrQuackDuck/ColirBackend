using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Extensions;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly ColirDbContext _dbContext;

    public AttachmentRepository(ColirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all attachments
    /// </summary>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    public async Task<IEnumerable<Attachment>> GetAllAsync(string[]? overriddenIncludes = default)
    {
        return await _dbContext.Attachments
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(Attachment.Message)])
            .ToListAsync();
    }

    /// <summary>
    /// Gets the attachment by id
    /// </summary>
    /// <param name="id">Id of the attachment to get</param>
    /// <param name="overriddenIncludes">Overridden options for eager loading</param>
    /// <exception cref="AttachmentNotFoundException">Thrown when the attachment wasn't found</exception>
    public async Task<Attachment> GetByIdAsync(long id, string[]? overriddenIncludes = default)
    {
        return await _dbContext.Attachments
            .AsNoTracking()
            .IncludeMultiple(overriddenIncludes ?? [nameof(Attachment.Message)])
            .FirstOrDefaultAsync(a => a.Id == id) ?? throw new AttachmentNotFoundException();
    }

    /// <summary>
    /// Adds the attachment to the DB
    /// </summary>
    /// <param name="attachment">An attachment to add</param>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    public async Task AddAsync(Attachment attachment)
    {
        await _dbContext.Attachments.AddAsync(attachment);
    }

    /// <summary>
    /// Deletes the attachment (not from the storage)
    /// </summary>
    /// <param name="attachment">An attachment to delete</param>
    /// <exception cref="AttachmentNotFoundException">Thrown when the attachment wasn't found</exception>
    public async Task DeleteAsync(Attachment attachment)
    {
        var target = await _dbContext.Attachments.FindAsync(attachment.Id) ?? throw new AttachmentNotFoundException();
        _dbContext.Attachments.Remove(target);
    }

    /// <summary>
    /// Deletes the attachment from DB by filename (not from the storage)
    /// </summary>
    /// <param name="fileName">Filename of the attachment to delete</param>
    /// <exception cref="AttachmentNotFoundException">Thrown when the attachment wasn't found</exception>
    public async Task DeleteAttachmentByPathAsync(string fileName)
    {
        var target = await _dbContext.Attachments.FirstOrDefaultAsync(a => a.Path.ToLower() == fileName.ToLower()) ?? throw new AttachmentNotFoundException();
        _dbContext.Attachments.Remove(target);
    }

    /// <summary>
    /// Deletes the attachment by id (not from the storage)
    /// </summary>
    /// <param name="id">Id of the attachment to delete</param>
    public async Task DeleteByIdAsync(long id)
    {
        var target = await _dbContext.Attachments.FindAsync(id) ?? throw new AttachmentNotFoundException();
        _dbContext.Attachments.Remove(target);
    }

    /// <summary>
    /// Updates the attachment
    /// </summary>
    /// <param name="attachment">An attachment to update</param>
    /// <exception cref="AttachmentNotFoundException">Thrown when a non-existing attachment is provided</exception>
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
    /// Saves changes to the DB
    /// </summary>
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Saves changes to the DB asynchronously
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}