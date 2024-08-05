﻿using Colir.BLL.Interfaces;
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
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);
        VoiceChatUsers.Remove(user);

        return Task.CompletedTask;
    }

    public async Task<SignalRHubResult> Leave()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserLeft", user.HexId);
        VoiceChatUsers.Remove(user);

        return Success();
    }

    public async Task<SignalRHubResult> MuteSelf()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserMuted", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> UnmuteSelf()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserUnmuted", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> DefeanSelf()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserDefeaned", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> UndefeanSelf()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserUndefeaned", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> SendVoiceSignal(string audioData)
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuerHexId = this.GetIssuerHexId();

        // Exclude sending data for those who are currently defeaned
        var idsToSendTo = VoiceChatUsers
            .Where(u => u.RoomGuid == roomGuid && !u.IsDefeaned)
            .Select(u => u.ConnectionId);

        await Clients.Clients(idsToSendTo).SendAsync(roomGuid, new Signal(issuerHexId, audioData));
        return Success();
    }

    public async Task<SignalRHubResult> EnableVideo()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserEnabledVideo", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> DisableVideo()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserDisabledVideo", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> SendVideoSignal(string videoData)
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuerHexId = this.GetIssuerHexId();

        var idsToSendTo = VoiceChatUsers
            .Select(u => u.ConnectionId);

        await Clients.Clients(idsToSendTo).SendAsync(roomGuid, new Signal(issuerHexId, videoData));
        return Success();
    }

    public async Task<SignalRHubResult> EnableStream()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserEnabledStream", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> DisableStream()
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var user = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        await Clients.Group(roomGuid).SendAsync("UserDisabledStream", user.HexId);

        return Success();
    }

    public async Task<SignalRHubResult> SendStreamSignal(string pictureData)
    {
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuerHexId = this.GetIssuerHexId();

        var idsToSendTo = VoiceChatUsers
            .Where(u => u.WatchedStreams.Contains(issuerHexId))
            .Select(u => u.ConnectionId);

        await Clients.Clients(idsToSendTo).SendAsync(roomGuid, new Signal(issuerHexId, pictureData));
        return Success();
    }

    public SignalRHubResult WatchStream(long userHexId)
    {
        var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        issuer.WatchedStreams.Add(userHexId);
        return Success();
    }

    public SignalRHubResult UnwatchStream(long userHexId)
    {
        var issuer = VoiceChatUsers.First(u => u.ConnectionId == Context.ConnectionId);

        issuer.WatchedStreams.Remove(userHexId);
        return Success();
    }
}