using Colir.BLL.Models;
using Colir.Interfaces.ApiRelatedServices;

namespace Colir.Interfaces.Hubs;

/// <summary>
/// SignalR Hub for full registration process with choosing the Hex Id from a random list
/// Intended to be used by users who register their accounts with OAuth2 services such as Google, GitHub etc..
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
    /// <param name="orderOfItem">The order of chosen hex id from given list</param>
    void ChooseHex(int orderOfItem);
    
    /// <summary>
    /// Sets a username
    /// </summary>
    /// <param name="username">Username to set</param>
    void ChooseUsername(string username);
    
    /// <summary>
    /// Finishes the registration
    /// </summary>
    DetailedUserModel FinishRegistration();
}