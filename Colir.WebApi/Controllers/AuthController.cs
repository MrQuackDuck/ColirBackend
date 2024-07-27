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

    public AuthController(IUserService userService, IConfiguration config, IOAuth2RegistrationQueueService registrationQueueService)
    {
        _userService = userService;
        _config = config;
        _registrationQueueService = registrationQueueService;
    }

    /// <summary>
    /// Exchanges the GitHub OAuth2 code on registration queue token
    /// Details: <see cref="IOAuth2RegistrationQueueService"/> 
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<string>> ExchangeGitHubCode([FromQuery] string code)
    {
        try
        {
            var githubClientId = _config["Authentication:GitHubClientId"]!;
            var githubAuthSecret = _config["Authentication:GitHubSecret"]!;

            using var httpClient = new HttpClient();
            var requestToGetToken = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            requestToGetToken.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", githubClientId },
                { "client_secret", githubAuthSecret },
                { "code", code }
            });

            // Getting the token from GitHub
            var responseWithToken = await (await httpClient.SendAsync(requestToGetToken)).Content.ReadAsStringAsync();
            var token = responseWithToken[(responseWithToken.IndexOf('=') + 1)..responseWithToken.IndexOf('&')];

            // Using the token to obtain a unique id of the user
            var requestToGetUserData = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            requestToGetUserData.Headers.Add("Accept", "application/vnd.github+json");
            requestToGetUserData.Headers.Add("Authorization", $"Bearer {token}");
            requestToGetUserData.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            requestToGetUserData.Headers.Add("User-Agent", "ASP.NET");

            var responseWithUserData = await httpClient.SendAsync(requestToGetUserData);
            responseWithUserData.EnsureSuccessStatusCode();
            dynamic userData = JObject.Parse(await responseWithUserData.Content.ReadAsStringAsync());
            var userGitHubId = (string)userData.id.ToString();

            var queueToken = _registrationQueueService.AddToQueue(userGitHubId);

            return Ok(new { queueToken });
        }
        catch (ArgumentException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserAlreadyInRegistrationQueue));   
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

            // Getting the key and generating a token
            var encrpyionKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:JwtKey").Value!));
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
                signingCredentials: new SigningCredentials(encrpyionKey, SecurityAlgorithms.HmacSha256));

            var jwtTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // Appending the token to cookies
            Response.Cookies.Append("jwt", jwtTokenString, new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Ok(new { jwtTokenString });
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
}