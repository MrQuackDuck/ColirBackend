using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHexColorGenerator _hexGenerator;
    
    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IHexColorGenerator hexGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hexGenerator = hexGenerator;
    }
    
    /// <inheritdoc cref="IUserService.GetAccountInfo"/>
    public async Task<DetailedUserModel> GetAccountInfo(RequestToGetAccountInfo request)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        return _mapper.Map<DetailedUserModel>(user);
    }

    /// <inheritdoc cref="IUserService.AuthorizeWithGitHubAsync"/>
    public async Task<DetailedUserModel> AuthorizeWithGitHubAsync(RequestToAuthorizeWithGitHub request)
    {
        try
        {
            // Returns user data if found
            var user = await _unitOfWork.UserRepository.GetByGithudIdAsync(request.GitHubId);
            return _mapper.Map<DetailedUserModel>(user);
        }
        catch (UserNotFoundException)
        {
            // Create a user if wasn't found
            var user = new User
            {
                Username = request.Username,
                HexId = request.HexId,
                GitHubId = request.GitHubId,
                AuthType = UserAuthType.Github
            };
            
            // Check if an user with the same HexId already exists
            if (await _unitOfWork.UserRepository.ExistsAsync(request.HexId))
            {
                throw new ArgumentException("Hex Id is not unique!");
            }

            var transaction = _unitOfWork.BeginTransaction();

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return _mapper.Map<DetailedUserModel>(user);
        }
    }

    /// <inheritdoc cref="IUserService.AuthorizeAsAnnoymousAsync"/>
    public async Task<DetailedUserModel> AuthorizeAsAnnoymousAsync(RequestToAuthorizeAsAnnoymous request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        var user = new User
        {
            Username = request.DesiredUsername,
            HexId = await _hexGenerator.GetUniqueHexColor(),
            AuthType = UserAuthType.Anonymous
        };

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return _mapper.Map<DetailedUserModel>(user);
    }

    /// <inheritdoc cref="IUserService.ChangeUsernameAsync"/>
    public async Task<DetailedUserModel> ChangeUsernameAsync(RequestToChangeUsername request)
    {
        var transaction = _unitOfWork.BeginTransaction();

        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        user.Username = request.DesiredUsername;
        
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return _mapper.Map<DetailedUserModel>(user);
    }

    /// <inheritdoc cref="IUserService.ChangeSettingsAsync"/>
    public async Task ChangeSettingsAsync(RequestToChangeSettings request)
    {
        var transaction = _unitOfWork.BeginTransaction();

        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var settingsToUpdate = _mapper.Map<UserSettings>(request.NewSettings);
        settingsToUpdate.Id = user.UserSettings.Id;
        settingsToUpdate.UserId = user.Id;
        
        _unitOfWork.UserSettingsRepository.Update(settingsToUpdate);
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <inheritdoc cref="IUserService.DeleteAccount"/>
    public async Task DeleteAccount(RequestToDeleteAccount request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        await _unitOfWork.UserRepository.DeleteByIdAsync(request.IssuerId);
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}