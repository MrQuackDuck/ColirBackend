using Colir.Interfaces.ApiRelatedServices;

namespace Colir.ApiRelatedServices;

/// <inheritdoc cref="IOAuth2RegistrationQueueService"/>
/// TODO: Implement this service
public class OAuth2RegistrationQueueService : IOAuth2RegistrationQueueService
{
    /// <inheritdoc cref="IOAuth2RegistrationQueueService.AddToQueue"/>
    public string AddToQueue(string oAuth2UserId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IOAuth2RegistrationQueueService.ExchangeToken"/>
    public string ExchangeToken(string queueToken)
    {
        throw new NotImplementedException();
    }
}