using Colir.BLL.Models;
using Colir.BLL.RequestModels.UserStatistics;

namespace Colir.BLL.Interfaces;

public interface IUserStatisticsService
{
    Task<UserStatisticsModel> GetStatisticsAsync(RequestToGetStatistics request);
}