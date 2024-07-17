using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.UserStatistics;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class UserStatisticsService : IUserStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserStatisticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<UserStatisticsModel> GetStatisticsAsync(RequestToGetStatistics request)
    {
        throw new NotImplementedException();
    }
}