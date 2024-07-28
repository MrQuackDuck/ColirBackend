namespace Colir.Interfaces.ApiRelatedServices;

/// <summary>
/// On this website, when a user is redirected from the OAuth2 page, he/she is not instantly registered and authenticated
/// Instead, the user is given a new randomly generated code and placed in such queue
/// Later, in order to register, the user needs to exhange the code to be able to connect to the SignalR hub and start the registration process
/// 
/// This all was made to implement the dynamic registration process where a user chooses his HexId (aka. Colir Id)
/// from a given randomly generated list instead of choosing the custom one
/// </summary>
public interface IOAuth2RegistrationQueueService
{
    /// <summary>
    /// Adds a user to the registration queue
    /// </summary>
    /// <param name="oAuth2UserId">User's Id from OAuth service (like from Google, GitHub, etc..)</param>
    /// <returns>Queue token</returns>
    string AddToQueue(string oAuth2UserId);

    /// <summary>
    /// Returns the "oAuth2UserId" and deletes the user from the queue
    /// </summary>
    /// <param name="queueToken">Queue token given by <see cref="AddToQueue"/> method</param>
    /// <returns>User's Id from OAuth service (Google, GitHub, etc..)</returns>
    string ExchangeToken(string queueToken);
}