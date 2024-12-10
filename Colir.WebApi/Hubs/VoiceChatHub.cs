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
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

[Authorize]
[SignalRHub]
public class VoiceChatHub : ColirHub, IVoiceChatHub
{
    private readonly IRoomService _roomService;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly ConcurrentDictionary<string, string> ConnectionsToGroupsMapping = new();

    /// <summary>
    /// Dictionary to store users' data needed for voice chat
    /// The connection id is a key and the value is user's data
    /// The user is added to the dictionary when they connect to the Hub
    /// The user considered to be conncected to the voice chat if the 'IsConnectedToVoice' property is set to true
    /// </summary>
    private static readonly ConcurrentDictionary<string, VoiceChatUser> VoiceChatUsers = new();

    public VoiceChatHub(IRoomService roomService, IUnitOfWork unitOfWork, IEventService eventService)
    {
        _roomService = roomService;
        _unitOfWork = unitOfWork;
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

            // Create and add the user to the voice chat
            var user = new VoiceChatUser
            {
                ConnectionId = Context.ConnectionId,
                HexId = this.GetIssuerHexId(),
                RoomGuid = roomGuid,
            };

            VoiceChatUsers[Context.ConnectionId] = user;
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

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        if (VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user) && user.IsConnectedToVoice)
        {
            // Notify other users that the user has left
            await Clients.Group(user.RoomGuid).SendAsync("UserLeft", user.HexId);

            if (user.IsMuted)
            {
                await UpdateUserStatisticsAsync(user);
            }

            // Mark user as disconnected and remove
            user.IsConnectedToVoice = false;
            VoiceChatUsers.TryRemove(Context.ConnectionId, out _);
        }

        ConnectionsToGroupsMapping.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public SignalRHubResult GetVoiceChatUsers()
    {
        if (!ConnectionsToGroupsMapping.TryGetValue(Context.ConnectionId, out var issuerRoomGuid))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }
        var usersInRoom = VoiceChatUsers.Values.Where(u => u.RoomGuid == issuerRoomGuid && u.IsConnectedToVoice).ToList();
        return Success(usersInRoom);
    }

    public async Task<SignalRHubResult> Join(bool isMuted, bool isDeafened)
    {
        if (!ConnectionsToGroupsMapping.TryGetValue(Context.ConnectionId, out var roomGuid))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        var user = VoiceChatUsers[Context.ConnectionId];

        if (user.IsConnectedToVoice)
        {
            return Error(new(ErrorCode.YouAreAlreadyConnectedToVoiceChannel));
        }

        // Check if user has connections from multiple devices/clients
        if (VoiceChatUsers.Values.Any(u => u.HexId == user.HexId && u.RoomGuid == roomGuid && u.IsConnectedToVoice))
        {
            return Error(new(ErrorCode.YouAreAlreadyConnectedToVoiceChannel));
        }

        user.IsMuted = isMuted;
        user.IsDeafened = isDeafened;
        user.IsConnectedToVoice = true;
        await Clients.Group(roomGuid).SendAsync("UserJoined", user);
        return Success();
    }

    public async Task<SignalRHubResult> Leave()
    {
        if (!ConnectionsToGroupsMapping.TryGetValue(Context.ConnectionId, out var roomGuid))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        var user = VoiceChatUsers[Context.ConnectionId];

        if (!user.IsConnectedToVoice)
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        if (!user.IsMuted)
        {
            await UpdateUserStatisticsAsync(user);
        }

        user.IsConnectedToVoice = false;
        await Clients.Group(roomGuid).SendAsync("UserLeft", user.HexId);
        return Success();
    }

    /// <inheritdoc cref="IVoiceChatHub.MuteSelf"/>
    public async Task<SignalRHubResult> MuteSelf()
    {
        if (!VoiceChatUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            return Error(new(ErrorCode.YouAreNotConnectedToVoiceChannel));
        }

        if (!user.IsMuted)
        {
            await UpdateUserStatisticsAsync(user);
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
        user.LastTimeUnmuted = DateTime.UtcNow;
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
            .Where(u => u.RoomGuid == user.RoomGuid && !u.IsDeafened && u.IsConnectedToVoice)
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
            .Where(u => u.RoomGuid == user.RoomGuid && u.IsConnectedToVoice)
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
            .Where(u => u.RoomGuid == user.RoomGuid && u.WatchedStreams.Contains(user.HexId) && u.IsConnectedToVoice)
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

    private async Task UpdateUserStatisticsAsync(VoiceChatUser user)
    {
        var userSettings = await _unitOfWork.UserSettingsRepository.GetByUserHexIdAsync(user.HexId);
        if (!userSettings.StatisticsEnabled)
        {
            return;
        }

        var userStatistics = await _unitOfWork.UserStatisticsRepository.GetByUserHexIdAsync(user.HexId);
        var newSeconds = (int)(DateTime.UtcNow - user.LastTimeUnmuted).TotalSeconds;
        if (newSeconds <= 0)
        {
            return;
        }

        userStatistics.SecondsSpentInVoice += newSeconds;
        _unitOfWork.UserStatisticsRepository.Update(userStatistics);
        await _unitOfWork.SaveChangesAsync();
    }

    private static void OnUserKickedOrLeft((int hexId, string roomGuid) data)
    {
        var connectionIds = VoiceChatUsers.Values
            .Where(x => x.HexId == data.hexId && x.RoomGuid == data.roomGuid)
            .Select(c => c.ConnectionId);

        foreach (var connectionId in connectionIds)
        {
            Disconnect(connectionId);
        }
    }

    private static void OnRoomDeleted(string roomGuid)
    {
        var connectionIds = VoiceChatUsers.Values
            .Where(x => x.RoomGuid == roomGuid)
            .Select(c => c.ConnectionId);

        foreach (var connectionId in connectionIds)
        {
            Disconnect(connectionId);
        }
    }

    private static void OnUserDeletedAccountOrLoggedOut(int hexId)
    {
        var connectionIds = VoiceChatUsers.Values
            .Where(x => x.HexId == hexId)
            .Select(c => c.ConnectionId);

        foreach (var connectionId in connectionIds)
        {
            Disconnect(connectionId);
        }
    }
}