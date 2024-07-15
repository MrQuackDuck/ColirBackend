using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserStatisticsRepository : IRepository<UserStatistics>
{
    Task<UserStatistics> GetByUserHexIdAsync(long hexId);
}