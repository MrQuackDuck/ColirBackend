using Colir.Interfaces.ApiRelatedServices;
using Newtonsoft.Json.Linq;

namespace Colir.ApiRelatedServices;

public class GoogleOAuth2Api : IGoogleOAuth2Api
{
    private readonly IConfiguration _config;
    
    public GoogleOAuth2Api(IConfiguration config)
    {
        _config = config;
    }

    /// <inheritdoc cref="IGoogleOAuth2Api.GetUserGoogleAccessTokenAsync"/>
    public async Task<string> GetUserGoogleAccessTokenAsync(string googleClientId, string googleAuthSecret, string code)
    {
        using var httpClient = new HttpClient();
        
        var requestToGetToken = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
        requestToGetToken.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "client_id", googleClientId },
            { "client_secret", googleAuthSecret },
            { "code", code },
            { "redirect_uri", _config["Authentication:GoogleRedirectLink"]! },
            { "grant_type", "authorization_code" }
        });
        
        // Sending the request to get the token from Google
        dynamic responseWithToken = JObject.Parse(await (await httpClient.SendAsync(requestToGetToken)).Content.ReadAsStringAsync());
        return responseWithToken.access_token.ToString();
    }

    /// <inheritdoc cref="IGoogleOAuth2Api.GetUserGoogleIdAsync"/>
    public async Task<string> GetUserGoogleIdAsync(string googleAccessToken)
    {
        using var httpClient = new HttpClient();
        
        var requestToGetUserData = new HttpRequestMessage(HttpMethod.Get, $"https://www.googleapis.com/oauth2/v1/userinfo?access_token={googleAccessToken}");
        requestToGetUserData.Headers.Add("Accept", "application/json");
        requestToGetUserData.Headers.Add("User-Agent", "ASP.NET");

        var responseWithUserData = await httpClient.SendAsync(requestToGetUserData);
        responseWithUserData.EnsureSuccessStatusCode();
        dynamic userData = JObject.Parse(await responseWithUserData.Content.ReadAsStringAsync());
        return (string)userData.id.ToString();
    }
}