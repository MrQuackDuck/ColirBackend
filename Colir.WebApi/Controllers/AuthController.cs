using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Communication;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using Colir.Extensions;
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

    public AuthController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
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