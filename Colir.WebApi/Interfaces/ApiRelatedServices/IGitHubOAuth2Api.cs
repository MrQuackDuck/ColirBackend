namespace Colir.Interfaces.ApiRelatedServices;

public interface IGitHubOAuth2Api
{
    /// <summary>
    /// Gets an access token to get info about the GitHub profile
    /// </summary>
    /// <param name="githubClientId">GitHub OAuth2 client id</param>
    /// <param name="githubAuthSecret">GitHub OAuth2 secret</param>
    /// <param name="code">Code from GitHub OAuth2 consent screen</param>
    /// <returns>GitHub access token</returns>
    Task<string> GetUserGitHubTokenAsync(string githubClientId, string githubAuthSecret, string code);

    /// <summary>
    /// Gets user id from the Google profile
    /// </summary>
    /// <param name="githubAccessToken">Google</param>
    /// <returns>GitHub id of the user</returns>
    Task<string> GetUserGitHubIdAsync(string githubAccessToken);
}