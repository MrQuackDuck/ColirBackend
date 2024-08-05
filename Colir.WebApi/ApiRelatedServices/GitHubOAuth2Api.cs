using Colir.Interfaces.ApiRelatedServices;
using Newtonsoft.Json.Linq;

namespace Colir.ApiRelatedServices;

public class GitHubOAuth2Api : IGitHubOAuth2Api
{
    /// <inheritdoc cref="IGitHubOAuth2Api.GetUserGitHubTokenAsync"/>
    public async Task<string> GetUserGitHubTokenAsync(string githubClientId, string githubAuthSecret, string code)
    {
        using var httpClient = new HttpClient();

        var requestToGetToken = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
        requestToGetToken.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "client_id", githubClientId },
            { "client_secret", githubAuthSecret },
            { "code", code }
        });

        // Sending the request to get the token from GitHub
        var response = await httpClient.SendAsync(requestToGetToken);
        response.EnsureSuccessStatusCode();
        var responseWithToken = await (response).Content.ReadAsStringAsync();
        if (responseWithToken.ToLowerInvariant().Contains("error=bad_verification_code")) throw new HttpRequestException();
        return responseWithToken[(responseWithToken.IndexOf('=') + 1)..responseWithToken.IndexOf('&')];
    }

    /// <inheritdoc cref="IGitHubOAuth2Api.GetUserGitHubIdAsync"/>
    public async Task<string> GetUserGitHubIdAsync(string githubAccessToken)
    {
        using var httpClient = new HttpClient();

        var requestToGetUserData = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        requestToGetUserData.Headers.Add("Accept", "application/vnd.github+json");
        requestToGetUserData.Headers.Add("Authorization", $"Bearer {githubAccessToken}");
        requestToGetUserData.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        requestToGetUserData.Headers.Add("User-Agent", "ASP.NET");

        var responseWithUserData = await httpClient.SendAsync(requestToGetUserData);
        responseWithUserData.EnsureSuccessStatusCode();
        dynamic userData = JObject.Parse(await responseWithUserData.Content.ReadAsStringAsync());
        return (string)userData.id.ToString();
    }
}