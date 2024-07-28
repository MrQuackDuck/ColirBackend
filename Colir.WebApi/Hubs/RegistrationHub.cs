using Colir.BLL.Models;
using Colir.Interfaces.Hubs;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

/// <inheritdoc cref="IRegistrationHub"/>
[SignalRHub]
public class RegistrationHub : IRegistrationHub
{
    /// <inheritdoc cref="IRegistrationHub.Connect"/>
    public void Connect(string queueToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IRegistrationHub.RegenerateHexs"/>
    public void RegenerateHexs()
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseHex"/>
    public void ChooseHex(int orderOfItem)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseUsername"/>
    public void ChooseUsername(string username)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc cref="IRegistrationHub.FinishRegistration"/>
    public DetailedUserModel FinishRegistration()
    {
        throw new NotImplementedException();
    }
}