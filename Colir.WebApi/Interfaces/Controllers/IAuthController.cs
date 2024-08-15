using Colir.BLL.Models;
using Colir.Communication.RequestModels.Auth;
using Colir.Interfaces.ApiRelatedServices;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IAuthController
{
    /// <summary>
    /// If authenticated, returns <see cref="OkResult"/>
    /// If not, returns <see cref="UnauthorizedResult"/>
    /// </summary>
    ActionResult IsAuthenticated();

    /// <summary>
    /// Redirects the user to the GitHub authentication page
    /// </summary>
    ActionResult GitHubLogin();

    /// <summary>
    /// Redirects the user to the Google authentication page
    /// </summary>
    ActionResult GoogleLogin();

    /// <summary>
    /// Exchanges the GitHub OAuth2 code for a registration queue token
    /// Details: <see cref="IOAuth2RegistrationQueueService"/>
    /// IMPORTANT: If the user was already registered, a JWT & refresh tokens are generated and returned
    /// </summary>
    Task<ActionResult> ExchangeGitHubCode([FromQuery] string code, [FromQuery] string state);

    /// <summary>
    /// Exchanges the Google OAuth2 code for a registration queue token
    /// Details: <see cref="IOAuth2RegistrationQueueService"/>
    /// IMPORTANT: If the user was already registered, a JWT & refresh tokens are generated and returned
    /// </summary>
    Task<ActionResult> ExchangeGoogleCode([FromQuery] string code, [FromQuery] string state);

    /// <summary>
    /// Anonymously logs the user in
    /// Returns JWT & refresh tokens instantly
    /// </summary>
    /// <param name="name">Desired name</param>
    Task<ActionResult<DetailedUserModel>> AnonymousLogin(string name);

    /// <summary>
    /// Refreshes the JWT token and returns new refresh token
    /// </summary>
    Task<IActionResult> RefreshToken(RefreshTokenRequestModel model);

    /// <summary>
    /// Logs user out of his account
    /// </summary>
    Task<ActionResult> Logout();
}