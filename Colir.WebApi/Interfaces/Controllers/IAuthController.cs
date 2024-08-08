using Colir.BLL.Models;
using Colir.Interfaces.ApiRelatedServices;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IAuthController
{
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
    /// IMPORTANT: If the user was already registered, a JWT authentication token is generated and returned
    /// </summary>
    Task<ActionResult> ExchangeGitHubCode([FromQuery] string code, [FromQuery] string state);

    /// <summary>
    /// Exchanges the Google OAuth2 code for a registration queue token
    /// Details: <see cref="IOAuth2RegistrationQueueService"/>
    /// IMPORTANT: If the user was already registered, a JWT authentication token is generated and returned
    /// </summary>
    Task<ActionResult> ExchangeGoogleCode([FromQuery] string code, [FromQuery] string state);

    /// <summary>
    /// Anonymously logs the user in
    /// Returns JWT token instantly
    /// </summary>
    /// <param name="name">Desired name</param>
    Task<ActionResult<DetailedUserModel>> AnonnymousLogin(string name);

    /// <summary>
    /// Logs user out of his account
    /// </summary>
    Task<ActionResult> Logout();
}