using DAL.Entities;

namespace DAL.Interfaces;

public interface IReactionRepository : IRepository<Reaction>
{
    Task<List<Reaction>> GetReactionsOnMessage(long messageId, string[]? overriddenIncludes = default);
}