using System.Collections.Concurrent;
using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Attachment;
using Colir.BLL.RequestModels.Message;
using Colir.BLL.RequestModels.Room;
using Colir.Communication.Enums;
using Colir.Communication.RequestModels.Chat;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using Colir.Hubs.Abstract;
using Colir.Interfaces.Hubs;
using Colir.Misc.ExtensionMethods;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

[Authorize]
[SignalRHub]
[Route("API/ChatHub")]
public class ChatHub : ColirHub, IChatHub
{
    private readonly IRoomService _roomService;
    private readonly IMessageService _messageService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly ConcurrentDictionary<string, string> ConnectionsToGroupsMapping = new();

    public ChatHub(IRoomService roomService, IMessageService messageService, IAttachmentService attachmentService,
        IUnitOfWork unitOfWork)
    {
        _roomService = roomService;
        _messageService = messageService;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
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

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectionsToGroupsMapping.Remove(Context.ConnectionId, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IChatHub.GetMessages"/>
    public async Task<SignalRHubResult> GetMessages(GetLastMessagesModel model)
    {
        if (!IsModelValid(model)) return Error(new (ErrorCode.ModelNotValid));

        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];

        var request = new RequestToGetLastMessages
        {
            IssuerId = this.GetIssuerId(),
            Count = model.Count,
            SkipCount = model.SkipCount,
            RoomGuid = roomGuid
        };

        try
        {
            var requestToUpdateLastTimeUserReadChat = new RequestToUpdateLastTimeUserReadChat
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = roomGuid
            };

            await _roomService.UpdateLastTimeUserReadChatAsync(requestToUpdateLastTimeUserReadChat);

            return Success(await _messageService.GetLastMessagesAsync(request));
        }
        catch (IssuerNotInRoomException)
        {
            return Error(new(ErrorCode.IssuerNotInTheRoom), true);
        }
        catch (RoomExpiredException)
        {
            try { return Error(new(ErrorCode.RoomExpired), true); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
    }

    /// <inheritdoc cref="IChatHub.SendMessage"/>
    public async Task<SignalRHubResult> SendMessage(SendMessageModel model)
    {
        if (!IsModelValid(model)) return Error(new (ErrorCode.ModelNotValid));

        // Uploading attachments
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuerId = this.GetIssuerId();

        var allFilesSize = model.Attachments.Sum(a => a.Length);
        var roomFreeStorage = _unitOfWork.RoomRepository.RoomFileManager.GetFreeStorageSize(roomGuid);

        if (allFilesSize > roomFreeStorage)
        {
            return Error(new(ErrorCode.NotEnoughSpace));
        }

        if (model.Content.Length == 0)
        {
            return Error(new(ErrorCode.NotEnoughSpace));
        }

        try
        {
            // Uploading attachments and retrieving their ids
            var attachmentIds = (await Task.WhenAll(
                model.Attachments.Select(async file =>
                {
                    var request = new RequestToUploadAttachment
                    {
                        IssuerId = issuerId,
                        RoomGuid = roomGuid,
                        File = file
                    };

                    return (await _attachmentService.UploadAttachmentAsync(request)).Id;
                })
            )).ToList();

            var request = new RequestToSendMessage
            {
                IssuerId = issuerId,
                Content = model.Content,
                RoomGuid = roomGuid,
                AttachmentsIds = attachmentIds,
                ReplyMessageId = model.ReplyMessageId
            };

            // Sending the message
            var messageModel = await _messageService.SendAsync(request);

            // Notifying others
            await Clients.Group(roomGuid).SendAsync("ReceiveMessage", messageModel);
            return Success();
        }
        catch (RoomExpiredException)
        {
            try { return Error(new(ErrorCode.RoomExpired), true); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
        catch (IssuerNotInRoomException)
        {
            return Error(new(ErrorCode.IssuerNotInTheRoom), true);
        }
    }

    /// <inheritdoc cref="IChatHub.EditMessage"/>
    public async Task<SignalRHubResult> EditMessage(EditMessageModel model)
    {
        if (!IsModelValid(model)) return Error(new (ErrorCode.ModelNotValid));

        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];

            var request = new RequestToEditMessage
            {
                IssuerId = this.GetIssuerId(),
                MessageId = model.MessageId,
                NewContent = model.NewContent
            };

            var editedMessage = await _messageService.EditAsync(request);

            // Notifying others
            await Clients.Group(roomGuid).SendAsync("MessageEdited", editedMessage);
            return Success();
        }
        catch (ArgumentException)
        {
            return Error(new (ErrorCode.EmptyMessage));
        }
        catch (RoomExpiredException)
        {
            try { return Error(new(ErrorCode.RoomExpired), true); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
        catch (IssuerNotInRoomException)
        {
            return Error(new(ErrorCode.IssuerNotInTheRoom), true);
        }
        catch (NotEnoughPermissionsException)
        {
            return Error(new(ErrorCode.YouAreNotAuthorOfMessage));
        }
    }

    /// <inheritdoc cref="IChatHub.DeleteMessage"/>
    public async Task<SignalRHubResult> DeleteMessage(DeleteMessageModel model)
    {
        if (!IsModelValid(model)) return Error(new (ErrorCode.ModelNotValid));

        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];

            var request = new RequestToDeleteMessage { IssuerId = this.GetIssuerId(), MessageId = model.MessageId };

            await _messageService.DeleteAsync(request);

            // Notifying others
            await Clients.Group(roomGuid).SendAsync("MessageDeleted", model.MessageId);
            return Success();
        }
        catch (RoomExpiredException)
        {
            try { return Error(new(ErrorCode.RoomExpired), true); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
        catch (IssuerNotInRoomException)
        {
            return Error(new(ErrorCode.IssuerNotInTheRoom), true);
        }
        catch (NotEnoughPermissionsException)
        {
            return Error(new(ErrorCode.YouAreNotAuthorOfMessage));
        }
    }

    /// <inheritdoc cref="IChatHub.AddReactionOnMessage"/>
    public async Task<SignalRHubResult> AddReactionOnMessage(AddReactionOnMessageModel model)
    {
        if (!IsModelValid(model)) return Error(new (ErrorCode.ModelNotValid));

        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];

            var request = new RequestToAddReactionOnMessage
            {
                IssuerId = this.GetIssuerId(),
                MessageId = model.MessageId,
                Reaction = model.Reaction
            };

            var updatedMessage = await _messageService.AddReaction(request);

            // Notifying others
            await Clients.Group(roomGuid).SendAsync("MessageGotReaction", updatedMessage);
            return Success();
        }
        catch (IssuerNotInRoomException)
        {
            return Error(new(ErrorCode.IssuerNotInTheRoom), true);
        }
        catch (RoomExpiredException)
        {
            try { return Error(new(ErrorCode.RoomExpired), true); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
    }

    /// <inheritdoc cref="IChatHub.RemoveReactionFromMessage"/>
    public async Task<SignalRHubResult> RemoveReactionFromMessage(RemoveReactionFromMessageModel model)
    {
        if (!IsModelValid(model)) return Error(new (ErrorCode.ModelNotValid));

        try
        {
            var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];

            var request = new RequestToRemoveReactionFromMessage()
            {
                IssuerId = this.GetIssuerId(),
                ReactionId = model.ReactionId
            };

            var updatedMessage = await _messageService.RemoveReaction(request);

            // Notifying others
            await Clients.Group(roomGuid).SendAsync("MessageLostReaction", updatedMessage);
            return Success();
        }
        catch (IssuerNotInRoomException)
        {
            return Error(new(ErrorCode.IssuerNotInTheRoom), true);
        }
        catch (RoomExpiredException)
        {
            try { return Error(new(ErrorCode.RoomExpired), true); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
        catch (NotEnoughPermissionsException)
        {
            return Error(new(ErrorCode.YouAreNotAuthorOfReaction));
        }
    }
}