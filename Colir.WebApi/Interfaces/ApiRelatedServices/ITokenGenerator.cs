using DAL.Enums;

namespace Colir.Interfaces.ApiRelatedServices;

public interface ITokenGenerator
{
    /// <summary>
    /// Generates a JWT token for users to authenticate in this application
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="userHexId">Hex Id of the user</param>
    /// <param name="authType">Auth type of the user</param>
    /// <returns></returns>
    public string GenerateJwtToken(long userId, long userHexId, UserAuthType authType);
}