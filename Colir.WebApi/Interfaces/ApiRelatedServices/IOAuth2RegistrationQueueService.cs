using Colir.Communication.Models;
using Colir.Exceptions.NotFound;
using Colir.Hubs;

namespace Colir.Interfaces.ApiRelatedServices;

/// <summary>
/// On this website, when a user is redirected from the OAuth2 page, he/she is not instantly registered and authenticated
/// Instead, if the user is not registered yet, he/she receives a new randomly generated queue code and is placed in such queue
/// Later, in order to register, the user needs to exchange the code to be able to connect to the SignalR hub and start the registration process
/// <see cref="RegistrationHub"/>
///
/// IMPORTANT: If the user is already registered, he/she will receive a JWT authentication token instead of getting a queue token
///
/// This all was made to implement the dynamic registration process where a user chooses his HexId (aka Colir Id)
/// from a given randomly generated list instead of choosing a custom one
/// </summary>
public interface IOAuth2RegistrationQueueService
{
    /// <summary>
    /// Adds a user to the registration queue
    /// </summary>
    /// <param name="userData">The data about the user needed to proceed to the registration queue</param>
    /// <returns>Queue token</returns>
    string AddToQueue(RegistrationUserData userData);

    /// <summary>
    /// Exchanges the queue token for user data + deletes the user from the queue
    /// </summary>
    /// <param name="queueToken">Queue token given by the <see cref="AddToQueue"/> method</param>
    /// <returns>Returns the data about the user and deletes the user from the queue</returns>
    /// <exception cref="NotFoundException">Thrown when the queueToken is not valid</exception>
    RegistrationUserData ExchangeToken(string queueToken);
}