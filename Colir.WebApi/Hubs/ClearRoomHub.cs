using Colir.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Hubs;

[Authorize]
public class ClearRoomHub : Hub<IClearRoomHub>
{
    public async Task Connect(string roomGuid)
    {
        throw new NotImplementedException();
    }

    public async Task Clear()
    {
        throw new NotImplementedException();
    }
}