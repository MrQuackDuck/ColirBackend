using DAL.Entities;

namespace DAL.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetLastMessagesAsync(string roomGuid, int count, int skip);
    Task<List<Message>> GetMessagesRangeAsync(string roomGuid, long startId, long endId);
    Task<List<Message>> GetSurroundingMessages(string roomGuid, long messageId, int count);
    Task<List<Message>> GetAllRepliesToUserAfterDateAsync(string roomGuid, long userId, DateTime date);
}