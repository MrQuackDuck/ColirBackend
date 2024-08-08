using Colir.BLL.RequestModels.Room;
using Colir.BLL.Services;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using Colir.Hubs.Abstract;
using Colir.Interfaces.Hubs;
using Colir.Misc.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

[Authorize]
[SignalRHub]
public class ClearRoomHub : ColirHub, IClearRoomHub
{
    private readonly RoomService _roomService;

    private static readonly Dictionary<string, string> ConnectionsToGroupsMapping = new();

    public ClearRoomHub(RoomService roomService)
    {
        _roomService = roomService;
    }

    public override async Task OnConnectedAsync()
    {
        // Require a room GUID to connect
        var roomGuid = Context.GetHttpContext()?.Request.Query["roomGuid"].ToString();
        if (roomGuid is null || roomGuid.Length == 0)
        {
            Context.Abort();
        }

        try
        {
            // Trying to get the room. If not found, an exception will occur
            var room = await _roomService.GetRoomInfoAsync(new RequestToGetRoomInfo
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = roomGuid!
            });

            // If an issuer is not the owner of the room, abort the connection
            if (this.GetIssuerHexId() != room.Owner.HexId) Context.Abort();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomGuid!);
            ConnectionsToGroupsMapping[Context.ConnectionId] = roomGuid!;
        }
        catch (RoomExpiredException)
        {
            Context.Abort();
            await _roomService.DeleteAllExpiredAsync();
        }
        catch (RoomNotFoundException)
        {
            Context.Abort();
        }
        catch (IssuerNotInRoomException)
        {
            Context.Abort();
        }
    }

    /// <inheritdoc cref="IClearRoomHub.Clear"/>
    public async Task<SignalRHubResult> Clear()
    {
        var request = new RequestToClearRoom
        {
            IssuerId = this.GetIssuerId(),
            RoomGuid = ConnectionsToGroupsMapping[Context.ConnectionId]
        };

        var roomCleaner = await _roomService.ClearRoomAsync(request);

        var issuerConnectionId = Context.ConnectionId;

        roomCleaner.FileDeleted += async () =>
        {
            await Clients.Client(issuerConnectionId).SendAsync("FileDeleted");
        };

        roomCleaner.Finished += async () =>
        {
            await Clients.Client(issuerConnectionId).SendAsync("CleaningFinished");
            Context.Abort();
        };

        return Success(roomCleaner.FilesToDeleteCount);
    }
}