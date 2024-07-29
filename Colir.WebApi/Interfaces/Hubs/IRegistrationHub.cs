using Colir.BLL.Models;
using Colir.Interfaces.ApiRelatedServices;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Interfaces.Hubs;

/// <summary>
/// SignalR Hub for full registration process with choosing the Hex Id from a random list
/// Intended to be used by users who register their accounts via OAuth2 services such as Google, GitHub etc..
/// </summary>
public interface IRegistrationHub
{
    /// <summary>
    /// Regenerates the list of Hexs to choose from
    /// </summary>
    Task RegenerateHexs();
    
    /// <summary>
    /// Chooses the Hex Id for the user
    /// </summary>
    /// <param name="hex">The hex id from previously given list</param>
    /// <exception cref="HubException">Thrown when the hex is not present in list of hexs to choose from</exception>
    void ChooseHex(int hex);
    
    /// <summary>
    /// Sets a username
    /// </summary>
    /// <param name="username">Username to set</param>
    void ChooseUsername(string username);
    
    /// <summary>
    /// Finishes the registration
    /// </summary>
    Task<DetailedUserModel> FinishRegistration();
}