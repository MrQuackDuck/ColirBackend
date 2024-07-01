using DAL.Entities;

namespace DAL.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetLastMessages(string roomGuid, int count, int skip);
}