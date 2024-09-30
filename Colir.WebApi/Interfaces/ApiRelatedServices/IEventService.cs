namespace Colir.Interfaces.ApiRelatedServices;

/// <summary>
/// Singleton service for passing events from controllers to SignalR hubs
/// </summary>
public interface IEventService
{
    event Action<(int, string)> UserKicked;

    void OnUserKicked(int hexId, string roomGuid);
}