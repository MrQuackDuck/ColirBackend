using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;
using Colir.BLL.RequestModels.Message;
using Colir.BLL.RequestModels.Room;
using Colir.Communication;
using Colir.Communication.RequestModels.Chat;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
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
public class ChatHub : Hub, IChatHub
{
    private readonly IRoomService _roomService;
    private readonly IMessageService _messageService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly Dictionary<string, string> ConnectionsToGroupsMapping = new();
    
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
            await SendErrorAsync(new ErrorResponse(ErrorCode.InvalidActionException));
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
            await SendErrorAsync(new (ErrorCode.RoomExpired));
            Context.Abort();
        }
        catch (RoomNotFoundException)
        {
            await SendErrorAsync(new (ErrorCode.RoomNotFound));
            Context.Abort();
        }
        catch (IssuerNotInRoomException)
        {
            await SendErrorAsync(new(ErrorCode.IssuerNotInTheRoom));
            Context.Abort();
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectionsToGroupsMapping.Remove(Context.ConnectionId);
        return Task.CompletedTask;
    }

    public async Task GetMessages(GetLastMessagesModel model)
    {
        var request = new RequestToGetLastMessages
        {
            IssuerId = this.GetIssuerId(),
            Count = model.Count,
            SkipCount = model.SkipCount,
            RoomGuid = ConnectionsToGroupsMapping[Context.ConnectionId]
        };

        try
        {
            var messages = await _messageService.GetLastMessagesAsync(request);
            await Clients.Caller.SendAsync("ReceiveMessages", messages);
        }
        catch (RoomExpiredException)
        {
            await SendErrorAsync(new(ErrorCode.RoomExpired));
            Context.Abort();
        }
    }

    public async Task SendMessage(SendMessageModel model)
    {
        // Uploading attachments
        var roomGuid = ConnectionsToGroupsMapping[Context.ConnectionId];
        var issuerId = this.GetIssuerId();
        
        var allFilesSize = model.Attachments.Sum(a => a.Length);
        var roomFreeStorage = _unitOfWork.RoomRepository.RoomFileManager.GetFreeStorageSize(roomGuid);
        
        if (allFilesSize > roomFreeStorage)
        {
            await SendErrorAsync(new(ErrorCode.NotEnoughSpace));
            return;
        }

        if (model.Content.Length == 0)
        {
            await SendErrorAsync(new(ErrorCode.EmptyMessage));
            return;
        }

        try
        {
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
                AttachmentsIds = attachmentIds
            };

            var messageModel = await _messageService.SendAsync(request);
            await Clients.Group(roomGuid).SendAsync("ReceiveMessage", messageModel);
        }
        catch (RoomExpiredException)
        {
            await SendErrorAsync(new (ErrorCode.RoomExpired));
            Context.Abort();
        }
        catch (IssuerNotInRoomException)
        {
            await SendErrorAsync(new(ErrorCode.IssuerNotInTheRoom));
            Context.Abort();
        }
    }

    public async Task<MessageModel> EditMessage(EditMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteMessage(DeleteMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> AddReactionOnMessage(AddReactionOnMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> RemoveReactionFromMessage(RemoveReactionFromMessageModel model)
    {
        throw new NotImplementedException();
    }
    
    private async Task SendErrorAsync(ErrorResponse response)
    {
        await Clients.Caller.SendAsync("Error", response);
    }
}