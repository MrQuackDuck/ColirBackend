namespace Colir.Interfaces.ApiRelatedServices;

public interface IGitHubOAuth2Api
{
    /// <summary>
    /// Gets an access token that can be exchanged to get info about the GitHub user
    /// </summary>
    /// <param name="githubClientId">GitHub OAuth2 client id</param>
    /// <param name="githubAuthSecret">GitHub OAuth2 secret</param>
    /// <param name="code">Code from GitHub OAuth2 consent screen</param>
    /// <returns>GitHub access token</returns>
    /// <exception cref="HttpRequestException">Thrown when <param name="code"/> is either invalid or expired</exception>
    Task<string> GetUserGitHubTokenAsync(string githubClientId, string githubAuthSecret, string code);

    /// <summary>
    /// Gets user id from the GitHub profile
    /// </summary>
    /// <param name="githubAccessToken">GitHub access token</param>
    /// <returns>GitHub id of the user</returns>
    Task<string> GetUserGitHubIdAsync(string githubAccessToken);
}