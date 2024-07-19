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
    
    /// <summary>
    /// Authorizes the user (i.e: returns account data for user)
    ///
    /// If the user was found by GitHub Id, its data will be returned
    /// Otherwise, a new user with provided HexId and Username will be created
    /// </summary>
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

    /// <summary>
    /// Creates a new user with provided Username and returns its data instantly
    /// </summary>
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

    /// <summary>
    /// Changes the username for an user
    /// </summary>
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

    /// <summary>
    /// Changes the settings for the user
    /// </summary>
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

    /// <summary>
    /// Deletes the account of the user
    /// </summary>
    public async Task DeleteAccount(RequestToDeleteAccount request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        await _unitOfWork.UserRepository.DeleteByIdAsync(request.IssuerId);
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}