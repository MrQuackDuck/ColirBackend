using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IClearRoomHub
{
    Task<SignalRHubResult> Clear();
}