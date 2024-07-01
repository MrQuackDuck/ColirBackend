using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;

namespace Colir.BLL.Interfaces;

public interface IUserService
{
    Task<DetailedUserModel> AuthorizeWithGitHubAsync(RequestToAuthorizeWithGitHub request);
    Task<DetailedUserModel> AuthorizeAsAnnoymousAsync(RequestToAuthorizeAsAnnoymous request);
    Task<DetailedUserModel> ChangeUsernameAsync(RequestToChangeUsername request);
    Task ChangeSettingsAsync(RequestToChangeSettings request);
    Task DeleteAccount(RequestToDeleteAccount request);
}