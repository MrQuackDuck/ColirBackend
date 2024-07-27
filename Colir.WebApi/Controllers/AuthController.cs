using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Communication;
using Colir.Exceptions;
using Colir.Interfaces.Controllers;
using DAL.Enums;
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
    private readonly ILogger _logger;

    public AuthController(IUserService userService, IConfiguration config, ILogger<AuthController> logger)
    {
        _userService = userService;
        _config = config;
        _logger = logger;
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

            // Claims for a token
            var claims = new List<Claim>
            {
                new Claim("Id", userModel.Id.ToString()),
                new Claim("AuthType", userModel.AuthType.ToString()),
            };

            // Generating a token
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
        catch (Exception e)
        {
            _logger.LogError(e, "An unhandled exception occurred!");
            return BadRequest();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // Delete the account if the request was issued by an user with anonymous auth type
            var userId = long.Parse(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
            var authType = HttpContext.User.Claims.First(c => c.Type == "AuthType").Value;
            if (authType == UserAuthType.Anonymous.ToString())
            {
                await _userService.DeleteAccount(new() { IssuerId = userId });
            }

            Response.Cookies.Delete("jwt");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unhandled exception occurred!");
            return BadRequest();
        }
    }
}