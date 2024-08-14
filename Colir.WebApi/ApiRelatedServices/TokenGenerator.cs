using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Colir.Interfaces.ApiRelatedServices;
using DAL.Enums;
using Microsoft.IdentityModel.Tokens;

namespace Colir.ApiRelatedServices;

public class TokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _config;

    public TokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    /// <inheritdoc cref="ITokenGenerator.GenerateJwtToken"/>
    public string GenerateJwtToken(long userId, long userHexId, UserAuthType authType)
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