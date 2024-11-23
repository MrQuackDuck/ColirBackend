using System.Collections.Concurrent;
using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Room;
using Colir.Communication.Enums;
using Colir.Communication.Models;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using Colir.Hubs.Abstract;
using Colir.Interfaces.ApiRelatedServices;
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

    private static readonly ConcurrentDictionary<string, string> ConnectionsToGroupsMapping = new();
    private static readonly ConcurrentDictionary<string, VoiceChatUser> VoiceChatUsers = new();

    // TODO: Fix in the future. This is a temporary solution to keep track of all connected users (not only those who are in the voice chat)
    private static readonly ConcurrentBag<ChatUser> ConnectedUsers = new();

    public VoiceChatHub(IRoomService roomService, IEventService eventService)
    {
        _roomService = roomService;
        eventService.UserKicked += OnUserKickedOrLeft;
        eventService.UserLeftRoom += OnUserKickedOrLeft;
        eventService.RoomDeleted += OnRoomDeleted;
        eventService.UserDeletedAccount += OnUserDeletedAccountOrLoggedOut;
        eventService.UserLoggedOut += OnUserDeletedAccountOrLoggedOut;
    }

    public override async Task OnConnectedAsync()
    {
        var roomGuid = Context.GetHttpContext()?.Request.Query["roomGuid"].ToString();
        if (string.IsNullOrEmpty(roomGuid))
        {
            Context.Abort();
            return;
        }

        try
        {
            await _roomService.GetRoomInfoAsync(new RequestToGetRoomInfo
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = roomGuid
            });

            await Groups.AddToGroupAsync(Context.ConnectionId, roomGuid);

            // Adding the user to the list of connected users
            ConnectedUsers.Add(new ChatUser
            {
                ConnectionId = Context.ConnectionId,
                HexId = this.GetIssuerHexId(),
                RoomGuid = roomGuid
            });

            ConnectionsToGroupsMapping[Context.ConnectionId] = roomGuid;
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

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Notify other users that the user has left
        if (VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            Clients.Group(user.RoomGuid).SendAsync("UserLeft", user.HexId);
        }

        ConnectionsToGroupsMapping.TryRemove(Context.ConnectionId, out _);
        VoiceChatUsers.TryRemove(Context.ConnectionId, out _);
        ConnectedUsers.RemoveWhere(x => x.ConnectionId == Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    /// <inheritdoc cref="IVoiceChatHub.GetVoiceChatUsers"/>
    public SignalRHubResult GetVoiceChatUsers()
    {
        if (!ConnectionsToGroupsMapping.TryGetValue(Context.ConnectionId, out var issuerRoomGuid))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
        return Success(VoiceChatUsers.Values.Where(u => u.RoomGuid == issuerRoomGuid).ToList());
    }

    /// <inheritdoc cref="IVoiceChatHub.Join"/>
    public async Task<SignalRHubResult> Join(bool isMuted, bool isDeafened)
    {
        if (!ConnectionsToGroupsMapping.TryGetValue(Context.ConnectionId, out var roomGuid))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        var issuerHexId = this.GetIssuerHexId();

        var user = new VoiceChatUser
        {
            HexId = issuerHexId,
            ConnectionId = Context.ConnectionId,
            RoomGuid = roomGuid,
            IsMuted = isMuted,
            IsDeafened = isDeafened,
            IsStreamEnabled = false,
            IsVideoEnabled = false
        };

        if (VoiceChatUsers.TryAdd(Context.ConnectionId, user))
        {
            await Clients.Group(roomGuid).SendAsync("UserJoined", user);
            return Success();
        }
        else
        {
            return Error(new(ErrorCode.YouAreAlreadyConnectedToVoiceChannel));
        }
    }

    /// <inheritdoc cref="IVoiceChatHub.Leave"/>
    public async Task<SignalRHubResult> Leave()
    {
        if (!ConnectionsToGroupsMapping.TryGetValue(Context.ConnectionId, out var roomGuid))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        if (VoiceChatUsers.TryRemove(Context.ConnectionId, out var user))
        {
            await Clients.Group(roomGuid).SendAsync("UserLeft", user.HexId);
            return Success();
        }
        else
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
    }

    /// <inheritdoc cref="IVoiceChatHub.MuteSelf"/>
    public async Task<SignalRHubResult> MuteSelf()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsMuted = true;
        await Clients.Group(user.RoomGuid).SendAsync("UserMuted", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.UnmuteSelf"/>
    public async Task<SignalRHubResult> UnmuteSelf()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsMuted = false;
        await Clients.Group(user.RoomGuid).SendAsync("UserUnmuted", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.DeafenSelf"/>
    public async Task<SignalRHubResult> DeafenSelf()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsDeafened = true;
        await Clients.Group(user.RoomGuid).SendAsync("UserDeafened", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.UndeafenSelf"/>
    public async Task<SignalRHubResult> UndeafenSelf()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsDeafened = false;
        await Clients.Group(user.RoomGuid).SendAsync("UserUndeafened", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.SendVoiceSignal"/>
    public async Task<SignalRHubResult> SendVoiceSignal(string audioData)
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        var idsToSendTo = VoiceChatUsers.Values
            .Where(u => u.RoomGuid == user.RoomGuid && !u.IsDeafened)
            .Select(u => u.ConnectionId);

        await Clients.Clients(idsToSendTo).SendAsync("ReceiveVoiceSignal", new Signal(user.HexId, audioData));
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.EnableVideo"/>
    public async Task<SignalRHubResult> EnableVideo()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsVideoEnabled = true;
        await Clients.Group(user.RoomGuid).SendAsync("UserEnabledVideo", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.DisableVideo"/>
    public async Task<SignalRHubResult> DisableVideo()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsVideoEnabled = false;
        await Clients.Group(user.RoomGuid).SendAsync("UserDisabledVideo", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.SendVideoSignal"/>
    public async Task<SignalRHubResult> SendVideoSignal(string videoData)
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        var idsToSendTo = VoiceChatUsers.Values
            .Where(u => u.RoomGuid == user.RoomGuid)
            .Select(u => u.ConnectionId);

        await Clients.Clients(idsToSendTo).SendAsync("ReceiveVideoSignal", new Signal(user.HexId, videoData));
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.EnableStream"/>
    public async Task<SignalRHubResult> EnableStream()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsStreamEnabled = true;
        await Clients.Group(user.RoomGuid).SendAsync("UserEnabledStream", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.DisableStream"/>
    public async Task<SignalRHubResult> DisableStream()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.IsStreamEnabled = false;
        await Clients.Group(user.RoomGuid).SendAsync("UserDisabledStream", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.SendStreamSignal"/>
    public async Task<SignalRHubResult> SendStreamSignal(string pictureData)
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        var idsToSendTo = VoiceChatUsers.Values
            .Where(u => u.RoomGuid == user.RoomGuid && u.WatchedStreams.Contains(user.HexId))
            .Select(u => u.ConnectionId);

        await Clients.Clients(idsToSendTo).SendAsync("ReceiveStreamData", new Signal(user.HexId, pictureData));
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.WatchStream"/>
    public SignalRHubResult WatchStream(long userHexId)
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.WatchedStreams.Add(userHexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.UnwatchStream"/>
    public SignalRHubResult UnwatchStream(long userHexId)
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        user.WatchedStreams.Remove(userHexId);
        return Success();
    }

    private static void OnUserKickedOrLeft((int hexId, string roomGuid) data)
    {
        var connectionIds = ConnectedUsers.Where(x => x.HexId == data.hexId && x.RoomGuid == data.roomGuid).Select(c => c.ConnectionId);

        foreach (var connectionId in connectionIds)
        {
            Disconnect(connectionId);
        }
    }

    private static void OnRoomDeleted(string roomGuid)
    {
        var connectionIds = ConnectedUsers.Where(x => x.RoomGuid == roomGuid).Select(c => c.ConnectionId);

        foreach (var connectionId in connectionIds)
        {
            Disconnect(connectionId);
        }
    }

    private static void OnUserDeletedAccountOrLoggedOut(int hexId)
    {
        var connectionIds = ConnectedUsers.Where(x => x.HexId == hexId).Select(c => c.ConnectionId);

        foreach (var connectionId in connectionIds)
        {
            Disconnect(connectionId);
        }
    }
}