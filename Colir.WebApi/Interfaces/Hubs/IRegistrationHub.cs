using Colir.Communication.Enums;
using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

/// <summary>
/// SignalR Hub to provide full registration process with choosing the Hex Id from a random list and the username
/// Intended to be used by users who register their accounts via OAuth2 services such as Google, GitHub etc..
/// </summary>
public interface IRegistrationHub
{
    /// <summary>
    /// Regenerates the list of Hexs to choose from
    /// </summary>
    Task<SignalRHubResult> RegenerateHexs();

    /// <summary>
    /// Chooses the Hex Id for the user
    /// An error with <see cref="ErrorCode.InvalidActionException"/> code returned when the chosen hex is not present in the offered list
    /// </summary>
    /// <param name="hex">The hex id from previously given list</param>
    SignalRHubResult ChooseHex(int hex);

    /// <summary>
    /// Sets a username
    /// </summary>
    /// <param name="username">Username to set</param>
    SignalRHubResult ChooseUsername(string username);

    /// <summary>
    /// Finishes the registration and sends JWT token to the user
    /// An error with <see cref="ErrorCode.InvalidActionException"/> code returned either when the hex or the username wasn't chosen yet (or an unhandled exception occurred)
    /// </summary>
    Task<SignalRHubResult> FinishRegistration();
}