using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Room;
using Colir.Communication.Enums;
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
    private static readonly List<VoiceChatUser> VoiceChatUsers = new();

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

    public SignalRHubResult GetVoiceChatUsers()
    {
        var issuerRoomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        return Success(VoiceChatUsers.Where(u => u.RoomGuid == issuerRoomGuid).ToList());
    }

    public async Task<SignalRHubResult> Join()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];

        // Removing the old user if connected already
        var oldUser = VoiceChatUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
        if (oldUser != null) VoiceChatUsers.Remove(oldUser);

        var user = new VoiceChatUser
        {
            HexId = this.GetIssuerHexId(),
            ConnectionId = Context.ConnectionId,
            RoomGuid = roomGuid,
            IsDefeaned = false,
            IsMuted = true,
            IsStreamEnabled = false,
            IsVideoEnabled = false
        };

        VoiceChatUsers.Add(user);

        await Clients.Group(roomGuid).SendAsync("UserJoined", user);
        return Success();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = VoiceChatUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId)!;
        VoiceChatUsers.Remove(user);

        return Task.CompletedTask;
    }

    public async Task<SignalRHubResult> Leave()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserLeft", issuer.HexId);
        VoiceChatUsers.Remove(issuer);

        return Success();
    }

    public async Task<SignalRHubResult> MuteSelf()
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            await Clients.Group(roomGuid).SendAsync("UserMuted", issuer.HexId);

            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> UnmuteSelf()
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            await Clients.Group(roomGuid).SendAsync("UserUnmuted", issuer.HexId);

            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> DefeanSelf()
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            await Clients.Group(roomGuid).SendAsync("UserDefeaned", issuer.HexId);

            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> UndefeanSelf()
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            await Clients.Group(roomGuid).SendAsync("UserUndefeaned", issuer.HexId);

            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> SendVoiceSignal(string audioData)
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            // Exclude sending data for those who are currently defeaned
            var idsToSendTo = VoiceChatUsers
                .Where(u => u.RoomGuid == roomGuid && !u.IsDefeaned)
                .Select(u => u.ConnectionId);

            await Clients.Clients(idsToSendTo).SendAsync(roomGuid, new Signal(issuer.HexId, audioData));
            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> EnableVideo()
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            await Clients.Group(roomGuid).SendAsync("UserEnabledVideo", issuer.HexId);

            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> DisableVideo()
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            await Clients.Group(roomGuid).SendAsync("UserDisabledVideo", issuer.HexId);

            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> SendVideoSignal(string videoData)
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            var idsToSendTo = VoiceChatUsers
                .Select(u => u.ConnectionId);

            await Clients.Clients(idsToSendTo).SendAsync(roomGuid, new Signal(issuer.HexId, videoData));
            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public async Task<SignalRHubResult> EnableStream()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserEnabledStream", issuer.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> DisableStream()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserDisabledStream", issuer.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> SendStreamSignal(string pictureData)
    {
        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            var idsToSendTo = VoiceChatUsers
                .Where(u => u.WatchedStreams.Contains(issuer.HexId))
                .Select(u => u.ConnectionId);

            await Clients.Clients(idsToSendTo).SendAsync(roomGuid, new Signal(issuer.HexId, pictureData));
            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public SignalRHubResult WatchStream(long userHexId)
    {
        try
        {
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            issuer.WatchedStreams.Add(userHexId);
            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    public SignalRHubResult UnwatchStream(long userHexId)
    {
        try
        {
            var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

            issuer.WatchedStreams.Remove(userHexId);
            return Success();
        }
        catch (InvalidOperationException)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }
}