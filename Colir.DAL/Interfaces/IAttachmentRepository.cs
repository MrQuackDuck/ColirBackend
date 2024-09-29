using DAL.Entities;

namespace DAL.Interfaces;

public interface IAttachmentRepository : IRepository<Attachment>
{
    public Task DeleteAttachmentByPathAsync(string fileName);
}