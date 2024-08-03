using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Room;
using Colir.Communication.Models;
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
public class VoiceChatHub : ColirHub, IVoiceChatHub
{
    private readonly IRoomService _roomService;
    
    private static readonly Dictionary<string, string> ConnectionsToGroupsMapping = new();
    private static readonly Dictionary<string, VoiceChatUser> ConnectionsToVoiceChatUsersMapping = new();
    
    public VoiceChatHub(IRoomService roomService)
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
            await _roomService.GetRoomInfoAsync(new RequestToGetRoomInfo
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = roomGuid!
            });

            await Groups.AddToGroupAsync(Context.ConnectionId, roomGuid!);
            ConnectionsToGroupsMapping[Context.ConnectionId] = roomGuid!;
        }
        catch (RoomExpiredException)
        {
            Context.Abort();
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

    public async Task<SignalRHubResult> Join()
    {
        ConnectionsToVoiceChatUsersMapping[Context.ConnectionId] = new VoiceChatUser
        {
            UserId = this.GetIssuerId(),
            RoomGuid = ConnectionsToGroupsMapping[Context.ConnectionId],
            IsDefeaned = false,
            IsMuted = true,
            IsStreamEnabled = false,
            IsVideoEnabled = false
        };

        return Success();
    }

    public async Task<SignalRHubResult> Leave()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> MuteSelf()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> UnmuteSelf()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> DefeanSelf()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> UndefeanSelf()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> SendVoiceSignal(string audioData)
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> EnableVideo()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> DisableVideo()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> SendVideo(string videoData)
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> EnableStream()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> DisableStream()
    {
        throw new NotImplementedException();
    }

    public async Task<SignalRHubResult> SendStreamPicture(string pictureData)
    {
        throw new NotImplementedException();
    }
}