using Colir.Exceptions.NotFound;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Misc.ExtensionMethods;
using DAL.Enums;

namespace Colir.ApiRelatedServices;

/// <inheritdoc cref="IOAuth2RegistrationQueueService"/>
public class OAuth2RegistrationQueueService : IOAuth2RegistrationQueueService
{
    private Dictionary<string, (string, UserAuthType)> _queue = new();
    
    /// <inheritdoc cref="IOAuth2RegistrationQueueService.AddToQueue"/>
    public string AddToQueue(string oAuth2UserId, UserAuthType authType)
    {
        // If user's already in the queue, remove him from there
        if (_queue.ContainsValue((oAuth2UserId, authType))) 
            _queue.RemoveByValue(oAuth2UserId);
        
        // Generating a new queue token
        var queueToken = Guid.NewGuid().ToString();
        _queue.Add(queueToken, (oAuth2UserId, authType));

        return queueToken;
    }

    /// <inheritdoc cref="IOAuth2RegistrationQueueService.ExchangeToken"/>
    public (string, UserAuthType) ExchangeToken(string queueToken)
    {
        var oAuth2UserId = _queue.GetValueOrDefault(queueToken);
        _queue.Remove(queueToken);
        
        return oAuth2UserId;
    }
}