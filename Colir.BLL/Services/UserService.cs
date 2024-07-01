using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class UserService : IUserService
{
    private IUnitOfWork _unitOfWork;
    
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public Task<DetailedUserModel> AuthorizeWithGitHubAsync(RequestToAuthorizeWithGitHub request)
    {
        throw new NotImplementedException();
    }

    public Task<DetailedUserModel> AuthorizeAsAnnoymousAsync(RequestToAuthorizeAsAnnoymous request)
    {
        throw new NotImplementedException();
    }

    public Task<DetailedUserModel> ChangeUsernameAsync(RequestToChangeUsername request)
    {
        throw new NotImplementedException();
    }

    public Task ChangeSettingsAsync(RequestToChangeSettings request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAccount(RequestToDeleteAccount request)
    {
        throw new NotImplementedException();
    }
}