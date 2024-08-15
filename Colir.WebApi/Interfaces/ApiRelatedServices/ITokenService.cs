using System.Security.Claims;
using DAL.Enums;
using Microsoft.IdentityModel.Tokens;

namespace Colir.Interfaces.ApiRelatedServices;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for users to authenticate in this application
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="userHexId">Hex Id of the user</param>
    /// <param name="authType">Auth type of the user</param>
    /// <returns></returns>
    public string GenerateJwtToken(long userId, long userHexId, UserAuthType authType);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <param name="accessToken">The JWT token that will be used as a key for encryption</param>
    public string GenerateRefreshToken(string accessToken);

    /// <summary>
    /// Validates the refresh token
    /// </summary>
    /// <param name="expiredAccessToken">The expired JWT token</param>
    /// <param name="refreshToken">The refresh token</param>
    /// <returns></returns>
    public Task<bool> ValidateRefreshToken(string expiredAccessToken, string refreshToken);

    /// <summary>
    /// Gets claims from the token
    /// </summary>
    /// <exception cref="SecurityTokenException">Thrown when the token is invalid</exception>
    public List<Claim> GetClaimsFromExpiredToken(string token);
}