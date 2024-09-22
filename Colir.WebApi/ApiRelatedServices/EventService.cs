using Colir.Interfaces.ApiRelatedServices;

namespace Colir.ApiRelatedServices;

/// <inheritdoc cref="IEventService"/>
public class EventService : IEventService
{
    public event Action<(int, string)> UserKicked = default!;

    public void KickUser(int hexId, string roomGuid)
    {
        UserKicked((hexId, roomGuid));
    }
}