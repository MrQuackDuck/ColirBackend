using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Communication;
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
using Newtonsoft.Json.Linq;

namespace Colir.Controllers;

[ApiController]
[Route("API/[controller]/[action]")]
public class AuthController : ControllerBase, IAuthController
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    private readonly IOAuth2RegistrationQueueService _registrationQueueService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IUserService userService, IConfiguration config, IOAuth2RegistrationQueueService registrationQueueService, IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _config = config;
        _registrationQueueService = registrationQueueService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Exchanges the GitHub OAuth2 code on registration queue token
    /// Details: <see cref="IOAuth2RegistrationQueueService"/>
    /// IMPORTANT: If the user was already registered, JWT authentication token is generated and returned
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> ExchangeGitHubCode([FromQuery] string code)
    {
        try
        {
            // Getting credentials from the configuration
            var githubClientId = _config["Authentication:GitHubClientId"]!;
            var githubAuthSecret = _config["Authentication:GitHubSecret"]!;
            
            using var httpClient = new HttpClient();

            // Getting the token
            var gitHubToken = await GetUserGitHubToken(httpClient, githubClientId, githubAuthSecret, code);
            
            // Using the token to obtain the id of the user from GitHub
            var userGitHubId = await GetUserGitHubId(httpClient, gitHubToken);
            
            try
            {
                // Check if the user already registered
                await _unitOfWork.UserRepository.GetByGithudIdAsync(userGitHubId);
                
                // If an exception not occurred, the user was already registered. So, authenticate him/her
                var request = new RequestToAuthorizeViaGitHub()
                {
                    GitHubId = userGitHubId
                };

                var userModel = await _userService.AuthorizeViaGitHubAsync(request);

                // Creating claims for a token
                var claims = new List<Claim>
                {
                    new Claim("Id", userModel.Id.ToString()),
                    new Claim("AuthType", userModel.AuthType.ToString())
                };

                var jwtToken = GenerateJwtToken(claims);

                // Applying the jwt token to response cookies
                Response.ApplyJwtToken(jwtToken);

                return Ok(new { jwtToken });
            }
            catch (UserNotFoundException)
            {
                // "UserNotFoundException" exception occured, which means the user's not registered yet, so give him a queue token
                // The token can be later exchanged in "RegistrationHub" to start registration process
                var queueToken = _registrationQueueService.AddToQueue(userGitHubId, UserAuthType.Github);
                return Ok(new { queueToken });   
            }
        }
        catch (HttpRequestException)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<DetailedUserModel>> AnonnymousLogin(string name)
    {
        try
        {
            var request = new RequestToAuthorizeAsAnnoymous
            {
                DesiredUsername = name
            };

            var userModel = await _userService.AuthorizeAsAnnoymousAsync(request);

            // Creating claims for a token
            var claims = new List<Claim>
            {
                new Claim("Id", userModel.Id.ToString()),
                new Claim("AuthType", userModel.AuthType.ToString())
            };

            var jwtToken = GenerateJwtToken(claims);

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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // Delete the account if the request was issued by an user with anonymous auth type
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
    private async Task<string> GetUserGitHubToken(HttpClient httpClient, string githubClientId, string githubAuthSecret, string code)
    {
        var requestToGetToken = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
        requestToGetToken.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "client_id", githubClientId },
            { "client_secret", githubAuthSecret },
            { "code", code }
        });

        // Sending the request to get the token from GitHub
        var responseWithToken = await (await httpClient.SendAsync(requestToGetToken)).Content.ReadAsStringAsync();
        return responseWithToken[(responseWithToken.IndexOf('=') + 1)..responseWithToken.IndexOf('&')];
    }

    [NonAction]
    private async Task<string> GetUserGitHubId(HttpClient httpClient, string githubToken)
    {
        var requestToGetUserData = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        requestToGetUserData.Headers.Add("Accept", "application/vnd.github+json");
        requestToGetUserData.Headers.Add("Authorization", $"Bearer {githubToken}");
        requestToGetUserData.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        requestToGetUserData.Headers.Add("User-Agent", "ASP.NET");

        var responseWithUserData = await httpClient.SendAsync(requestToGetUserData);
        responseWithUserData.EnsureSuccessStatusCode();
        dynamic userData = JObject.Parse(await responseWithUserData.Content.ReadAsStringAsync());
        return (string)userData.id.ToString();
    }
    
    [NonAction]
    private string GenerateJwtToken(List<Claim> claims)
    {
        // Getting the key and generating a token
        var encrpyionKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:JwtKey").Value!));
        var jwtToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
            signingCredentials: new SigningCredentials(encrpyionKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}