using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.UserStatistics;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class UserStatisticsService : IUserStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc cref="IUserStatisticsService.GetStatisticsAsync"/>
    public async Task<UserStatisticsModel> GetStatisticsAsync(RequestToGetStatistics request)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var statistics = await _unitOfWork.UserStatisticsRepository.GetByUserHexIdAsync(user.HexId);
        return _mapper.Map<UserStatisticsModel>(statistics);
    }
}