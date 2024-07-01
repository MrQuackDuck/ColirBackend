using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.UserStatistics;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class UserStatisticsService : IUserStatisticsService
{
    private IUnitOfWork _unitOfWork;
    
    public UserStatisticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public Task<UserStatisticsModel> GetStatisticsAsync(RequestToGetStatistics request)
    {
        throw new NotImplementedException();
    }
}