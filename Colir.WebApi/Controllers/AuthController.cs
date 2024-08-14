using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Communication.Enums;
using Colir.Communication.Models;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Interfaces.Controllers;
using Colir.Misc.ExtensionMethods;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Colir.Controllers;

[ApiController]
[Route("API/[controller]/[action]")]
public class AuthController : ControllerBase, IAuthController
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    private readonly IOAuth2RegistrationQueueService _registrationQueueService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGitHubOAuth2Api _gitHubOAuth2Api;
    private readonly IGoogleOAuth2Api _googleOAuth2Api;

    public AuthController(IUserService userService, IConfiguration config, IOAuth2RegistrationQueueService registrationQueueService,
        IUnitOfWork unitOfWork, IGitHubOAuth2Api gitHubOAuth2Api, IGoogleOAuth2Api googleOAuth2Api)
    {
        _userService = userService;
        _config = config;
        _registrationQueueService = registrationQueueService;
        _unitOfWork = unitOfWork;
        _gitHubOAuth2Api = gitHubOAuth2Api;
        _googleOAuth2Api = googleOAuth2Api;
    }

    /// <inheritdoc cref="IAuthController.GitHubLogin"/>
    [HttpGet]
    public ActionResult GitHubLogin()
    {
        var githubClientId = _config["Authentication:GitHubClientId"]!;
        var state = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("state", state);
        var link = $"https://github.com/login/oauth/authorize?client_id={githubClientId}&state={state}";
        return Ok(link);
    }

    /// <inheritdoc cref="IAuthController.GoogleLogin"/>
    [HttpGet]
    public ActionResult GoogleLogin()
    {
        var googleClientId = _config["Authentication:GoogleClientId"]!;
        var redirectLink = _config["Authentication:GoogleRedirectLink"]!.Replace(":", "%3A");
        var state = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("state", state);
        var link = $"https://accounts.google.com/o/oauth2/v2/auth?scope=https://www.googleapis.com/auth/userinfo.email&response_type=code&redirect_uri={redirectLink}&client_id={googleClientId}&state={state}";
        return Ok(link);
    }

    /// <inheritdoc cref="IAuthController.ExchangeGitHubCode"/>
    [HttpGet]
    public async Task<ActionResult> ExchangeGitHubCode([FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            // Verifying the state (against XSRF attacks)
            if (HttpContext.Session.GetString("state") != state) return BadRequest();
            HttpContext.Session.Remove("state");

            // Getting credentials from the configuration
            var githubClientId = _config["Authentication:GitHubClientId"]!;
            var githubAuthSecret = _config["Authentication:GitHubSecret"]!;

            // Getting an access token
            var gitHubToken = await _gitHubOAuth2Api.GetUserGitHubTokenAsync(githubClientId, githubAuthSecret, code);

            // Using the token to obtain user's id from GitHub
            var userGitHubId = await _gitHubOAuth2Api.GetUserGitHubIdAsync(gitHubToken);

            try
            {
                // Checking if the user already registered. Otherwise, the "UserNotFoundException" will be thrown
                await _unitOfWork.UserRepository.GetByGithudIdAsync(userGitHubId);

                // If an exception not occurred, the user was already registered. So, authenticate him/her
                var request = new RequestToAuthorizeViaGitHub { GitHubId = userGitHubId };

                var userModel = await _userService.AuthorizeViaGitHubAsync(request);

                var jwtToken = GenerateJwtToken(userModel.Id, userModel.HexId, userModel.AuthType);

                // Applying the jwt token to response's cookies
                Response.ApplyJwtToken(jwtToken);

                return Ok(new { jwtToken });
            }
            catch (UserNotFoundException)
            {
                // "UserNotFoundException" exception occured, which means the user's not registered yet, so give him a queue token
                // The token can be later exchanged in "RegistrationHub" to start a registration process
                var queueToken = _registrationQueueService.AddToQueue(new RegistrationUserData(userGitHubId, UserAuthType.Github));
                return Ok(new { queueToken });
            }
        }
        catch (HttpRequestException)
        {
            return BadRequest();
        }
    }

    /// <inheritdoc cref="IAuthController.ExchangeGoogleCode"/>
    [HttpGet]
    public async Task<ActionResult> ExchangeGoogleCode([FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            // Verifying the state (against XSRF attacks)
            if (HttpContext.Session.GetString("state") != state) return BadRequest();
            HttpContext.Session.Remove("state");

            // Getting credentials from the configuration
            var googleClientId = _config["Authentication:GoogleClientId"]!;
            var googleAuthSecret = _config["Authentication:GoogleClientSecret"]!;

            using var httpClient = new HttpClient();

            // Getting an access token
            var token = await _googleOAuth2Api.GetUserGoogleAccessTokenAsync(googleClientId, googleAuthSecret, code);

            // Using the token to obtain user's id from Google
            var userGoogleid = await _googleOAuth2Api.GetUserGoogleIdAsync(token);

            try
            {
                // Checking if the user already registered. Otherwise, the "UserNotFoundException" will be thrown
                await _unitOfWork.UserRepository.GetByGoogleIdAsync(userGoogleid);

                // If an exception not occurred, the user was already registered. So, authenticate him/her
                var request = new RequestToAuthorizeViaGoogle { GoogleId = userGoogleid };

                var userModel = await _userService.AuthorizeViaGoogleAsync(request);

                var jwtToken = GenerateJwtToken(userModel.Id, userModel.HexId, userModel.AuthType);

                // Applying the jwt token to response's cookies
                Response.ApplyJwtToken(jwtToken);

                return Ok(new { jwtToken });
            }
            catch (UserNotFoundException)
            {
                // "UserNotFoundException" exception occured, which means the user's not registered yet, so give him a queue token
                // The token can be later exchanged in "RegistrationHub" to start a registration process
                var queueToken = _registrationQueueService.AddToQueue(new RegistrationUserData(userGoogleid, UserAuthType.Google));
                return Ok(new { queueToken });
            }
        }
        catch (HttpRequestException)
        {
            return BadRequest();
        }
    }

    /// <inheritdoc cref="IAuthController.AnonymousLogin"/>
    [HttpPost]
    public async Task<ActionResult<DetailedUserModel>> AnonymousLogin(string name)
    {
        try
        {
            var request = new RequestToAuthorizeAsAnnoymous { DesiredUsername = name };

            var userModel = await _userService.AuthorizeAsAnnoymousAsync(request);

            var jwtToken = GenerateJwtToken(userModel.Id, userModel.HexId, userModel.AuthType);

            // Applying the jwt token
            Response.ApplyJwtToken(jwtToken);

            return Ok(new { jwtToken });
        }
        catch (StringTooShortException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooShort));
        }
        catch (StringTooLongException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooLong));
        }
    }

    /// <inheritdoc cref="IAuthController.Logout"/>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // Delete the account if the request was issued by a user with anonymous auth type
            var userId = this.GetIssuerId();
            var authType = HttpContext.User.Claims.First(c => c.Type == "AuthType").Value;
            if (authType == UserAuthType.Anonymous.ToString())
            {
                await _userService.DeleteAccount(new() { IssuerId = userId });
            }

            Response.Cookies.Delete("jwt");
            return Ok();
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }

    [NonAction]
    private string GenerateJwtToken(long userId, long userHexId, UserAuthType authType)
    {
        // Creating claims for a token
        var claims = new List<Claim>
        {
            new Claim("Id", userId.ToString()),
            new Claim("HexId", userHexId.ToString()),
            new Claim("AuthType", authType.ToString())
        };

        // Getting the key and generating a token
        var encrpyionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:JwtKey").Value!));
        var jwtToken = new JwtSecurityToken(claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
            signingCredentials: new SigningCredentials(encrpyionKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}