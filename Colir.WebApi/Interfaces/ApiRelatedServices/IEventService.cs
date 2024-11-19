namespace Colir.Interfaces.ApiRelatedServices;

/// <summary>
/// Singleton service for passing events from controllers to SignalR hubs
/// </summary>
public interface IEventService
{
    event Action<(int, string)> UserKicked;
    event Action<(int, string)> UserLeftRoom;
    event Action<int> UserDeletedAccount;

    void OnUserKicked(int hexId, string roomGuid);

    void OnUserLeftRoom(int hexId, string roomGuid);

    void OnUserDeletedAccount(int hexId);
}