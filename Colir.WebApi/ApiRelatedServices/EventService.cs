using Colir.Interfaces.ApiRelatedServices;

namespace Colir.ApiRelatedServices;

/// <inheritdoc cref="IEventService"/>
public class EventService : IEventService
{
    public event Action<(int, string)> UserKicked = default!;
    public event Action<(int, string)> UserLeftRoom = default!;
    public event Action<int> UserDeletedAccount = default!;
    public event Action<int> UserLoggedOut = default!;

    public void OnUserKicked(int hexId, string roomGuid)
    {
        UserKicked((hexId, roomGuid));
    }

    public void OnUserLeftRoom(int hexId, string roomGuid)
    {
        UserLeftRoom((hexId, roomGuid));
    }

    public void OnUserLoggedOut(int hexId)
    {
        UserLoggedOut(hexId);
    }

    public void OnUserDeletedAccount(int hexId)
    {
        UserDeletedAccount(hexId);
    }
}