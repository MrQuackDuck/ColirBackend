using Colir.Interfaces.ApiRelatedServices;
using Colir.Misc.ExtensionMethods;

namespace Colir.ApiRelatedServices;

/// <inheritdoc cref="IOAuth2RegistrationQueueService"/>
public class OAuth2RegistrationQueueService : IOAuth2RegistrationQueueService
{
    private Dictionary<string, string> _queue = new Dictionary<string, string>();
    
    /// <inheritdoc cref="IOAuth2RegistrationQueueService.AddToQueue"/>
    public string AddToQueue(string oAuth2UserId)
    {
        // If user's already in the queue, remove him from there
        if (_queue.ContainsValue(oAuth2UserId)) 
            _queue.RemoveByValue(oAuth2UserId);
        
        // Generating a new queue token
        var queueToken = Guid.NewGuid().ToString();
        _queue.Add(queueToken, oAuth2UserId);

        return queueToken;
    }

    /// <inheritdoc cref="IOAuth2RegistrationQueueService.ExchangeToken"/>
    public string ExchangeToken(string queueToken)
    {
        var oAuth2UserId = _queue[queueToken];
        _queue.Remove(queueToken);
        
        return oAuth2UserId;
    }
}