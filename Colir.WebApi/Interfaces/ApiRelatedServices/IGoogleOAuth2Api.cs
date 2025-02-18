﻿namespace Colir.Interfaces.ApiRelatedServices;

public interface IGoogleOAuth2Api
{
    /// <summary>
    /// Gets an access token that can be exchanged to get info about the Google user
    /// </summary>
    /// <param name="googleClientId">Google OAuth2 client id</param>
    /// <param name="googleAuthSecret">Google OAuth2 secret</param>
    /// <param name="code">Code from Google OAuth2 consent screen</param>
    /// <returns>Google access token</returns>
    /// <exception cref="HttpRequestException">Thrown when <param name="code"/> is either invalid or expired</exception>
    Task<string> GetUserGoogleAccessTokenAsync(string googleClientId, string googleAuthSecret, string code);

    /// <summary>
    /// Gets user id from the Google profile
    /// </summary>
    /// <param name="googleAccessToken">Google access token</param>
    /// <returns>Google Id of the user</returns>
    Task<string> GetUserGoogleIdAsync(string googleAccessToken);
}