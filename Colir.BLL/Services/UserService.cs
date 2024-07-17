using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<DetailedUserModel> AuthorizeWithGitHubAsync(RequestToAuthorizeWithGitHub request)
    {
        throw new NotImplementedException();
    }

    public async Task<DetailedUserModel> AuthorizeAsAnnoymousAsync(RequestToAuthorizeAsAnnoymous request)
    {
        throw new NotImplementedException();
    }

    public async Task<DetailedUserModel> ChangeUsernameAsync(RequestToChangeUsername request)
    {
        throw new NotImplementedException();
    }

    public async Task ChangeSettingsAsync(RequestToChangeSettings request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAccount(RequestToDeleteAccount request)
    {
        throw new NotImplementedException();
    }
}