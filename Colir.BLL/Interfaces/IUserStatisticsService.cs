using Colir.BLL.Models;
using Colir.BLL.RequestModels.UserStatistics;
using Colir.Exceptions.NotFound;

namespace Colir.BLL.Interfaces;

public interface IUserStatisticsService
{
    /// <summary>
    /// Gets user statistics
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<UserStatisticsModel> GetStatisticsAsync(RequestToGetStatistics request);
}