using Colir.ApiRelatedServices.Models;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Misc.ExtensionMethods;

namespace Colir.ApiRelatedServices;

/// <inheritdoc cref="IOAuth2RegistrationQueueService"/>
public class OAuth2RegistrationQueueService : IOAuth2RegistrationQueueService
{
    private Dictionary<string, RegistrationUserData> _queue = new();
    
    /// <inheritdoc cref="IOAuth2RegistrationQueueService.AddToQueue"/>
    public string AddToQueue(RegistrationUserData userData)
    {
        // If user's already in the queue, remove him from there
        if (_queue.ContainsValue(userData)) 
            _queue.RemoveByValue(userData);
        
        // Generating a new queue token
        var queueToken = Guid.NewGuid().ToString();
        _queue.Add(queueToken, userData);

        return queueToken;
    }

    /// <inheritdoc cref="IOAuth2RegistrationQueueService.ExchangeToken"/>
    public RegistrationUserData ExchangeToken(string queueToken)
    {
        var data = _queue.GetValueOrDefault(queueToken);
        _queue.Remove(queueToken);
        
        return data;
    }
}