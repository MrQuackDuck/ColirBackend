using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Colir.Communication.Models;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Misc.Utils;
using DAL.Encrpyion;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Colir.ApiRelatedServices;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public TokenService(IConfiguration config, IUnitOfWork unitOfWork)
    {
        _config = config;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc cref="ITokenService.GenerateJwtToken"/>
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
        var encrpyionKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Authentication:JwtKey").Value!));
        var jwtToken = new JwtSecurityToken(claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
            signingCredentials: new SigningCredentials(encrpyionKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    /// <inheritdoc cref="ITokenService.GenerateRefreshToken"/>
    public string GenerateRefreshToken(string accessToken)
    {
        var refreshTokenKey = _config["Authentication:RefreshTokenKey"]!;
        var encryptor = new StringEncryptor(HashUtil.ToSha256Truncated(refreshTokenKey, 16),
            HashUtil.ToSha256Truncated(accessToken, 16));

        var refreshToken = new RefreshToken()
        {
            Content = accessToken,
            ExpiryDate = DateTime.Now.Add(TimeSpan.FromDays(7))
        };

        return encryptor.Encrypt(JsonSerializer.Serialize(refreshToken));
    }

    /// <inheritdoc cref="ITokenService.ValidateRefreshToken"/>
    public async Task<bool> ValidateRefreshToken(string expiredAccessToken, string refreshToken)
    {
        try
        {
            var refreshTokenKey = _config["Authentication:RefreshTokenKey"]!;
            var encryptor = new StringEncryptor(HashUtil.ToSha256Truncated(refreshTokenKey, 16),
                HashUtil.ToSha256Truncated(expiredAccessToken, 16));

            var decryptedRefreshToken = JsonSerializer.Deserialize<RefreshToken>(encryptor.Decrypt(refreshToken))!;

            // If the refresh token is expired
            if (DateTime.Now > decryptedRefreshToken.ExpiryDate) return false;
            var claims = GetClaimsFromExpiredToken(decryptedRefreshToken.Content);
            var userHexId = int.Parse(claims.First(c => c.Type == "HexId").Value);

            // Check if user exists
            var userExists = await _unitOfWork.UserRepository.ExistsAsync(userHexId);
            if (!userExists) return false;

            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
    }

    /// <inheritdoc cref="ITokenService.GetClaimsFromExpiredToken"/>
    public List<Claim> GetClaimsFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:JwtKey"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal.Claims.ToList();
    }
}