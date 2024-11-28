using DAL.Enums;

namespace Colir.Communication.Models;

public class RegistrationUserData
{
    /// <summary>
    /// User's Id from OAuth service (like from Google, GitHub, etc..)
    /// </summary>
    public string OAuth2UserId { get; }

    /// <summary>
    /// The type of user's authentication
    /// </summary>
    public UserAuthType AuthType { get; }

    public RegistrationUserData(string oAuth2UserId, UserAuthType authType)
    {
        OAuth2UserId = oAuth2UserId;
        AuthType = authType;
    }

    protected bool Equals(RegistrationUserData other)
    {
        return OAuth2UserId == other.OAuth2UserId && AuthType == other.AuthType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OAuth2UserId, (int)AuthType);
    }
}