using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.Communication.Enums;
using Colir.Communication.RequestModels.Room;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using Colir.Hubs;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Interfaces.Controllers;
using Colir.Misc.ExtensionMethods;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class RoomController : ControllerBase, IRoomController
{
    private readonly IRoomService _roomService;
    private readonly IHubContext<ChatHub> _chatHub;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventService _eventService;

    public RoomController(IRoomService roomService, IHubContext<ChatHub> chatHub, IMapper mapper, IUnitOfWork unitOfWork, IEventService eventService)
    {
        _roomService = roomService;
        _chatHub = chatHub;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    /// <inheritdoc cref="IRoomController.GetRoomInfo"/>
    [HttpGet]
    public async Task<ActionResult<RoomModel>> GetRoomInfo([FromQuery]GetRoomInfoModel model)
    {
        try
        {
            var request = new RequestToGetRoomInfo
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid
            };

            return Ok(await _roomService.GetRoomInfoAsync(request));
        }
        catch (RoomExpiredException)
        {
            try { return BadRequest(new ErrorResponse(ErrorCode.RoomExpired)); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (IssuerNotInRoomException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.IssuerNotInTheRoom));
        }
    }

    /// <inheritdoc cref="IRoomController.CreateRoom"/>
    [HttpPost]
    public async Task<ActionResult<string>> CreateRoom(CreateRoomModel model)
    {
        try
        {
            DateTime? expiryDate = model.MinutesToLive.HasValue && model.MinutesToLive > 0
                ? DateTime.Now.AddMinutes(model.MinutesToLive.Value)
                : null;

            var request = new RequestToCreateRoom
            {
                IssuerId = this.GetIssuerId(),
                Name = model.Name,
                ExpiryDate = expiryDate
            };

            return Ok(await _roomService.CreateAsync(request));
        }
        catch (ArgumentException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.InvalidDate));
        }
        catch (StringTooLongException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooLong));
        }
        catch (StringTooShortException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooShort));
        }
    }

    /// <inheritdoc cref="IRoomController.JoinRoom"/>
    [HttpPost]
    public async Task<ActionResult<RoomModel>> JoinRoom(JoinRoomModel model)
    {
        try
        {
            var request = new RequestToJoinRoom
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid
            };

            var result = await _roomService.JoinMemberAsync(request);

            // Notifying users in the Chat hub that a new user has joined
            var user = _mapper.Map<UserModel>(await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId));
            await _chatHub.Clients.Group(model.RoomGuid).SendAsync("UserJoined", user);

            return Ok(result);
        }
        catch (InvalidActionException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserAlreadyInRoom));
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }

    /// <inheritdoc cref="IRoomController.LeaveRoom"/>
    [HttpPost]
    public async Task<ActionResult> LeaveRoom(LeaveRoomModel model)
    {
        try
        {
            var request = new RequestToLeaveFromRoom
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid
            };

            await _roomService.LeaveAsync(request);

            // Notifying users in the Chat hub that the user left
            await _chatHub.Clients.Group(request.RoomGuid).SendAsync("UserLeft", this.GetIssuerHexId());
            _eventService.OnUserLeftRoom(this.GetIssuerHexId(), request.RoomGuid);

            return Ok();
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (IssuerNotInRoomException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.IssuerNotInTheRoom));
        }
    }

    /// <inheritdoc cref="IRoomController.GetLastTimeReadChat"/>
    [HttpGet]
    public async Task<ActionResult<DateTime>> GetLastTimeReadChat([FromQuery]GetLastTimeReadChatModel model)
    {
        try
        {
            var request = new RequestToGetLastTimeUserReadChat
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid
            };

            return Ok(await _roomService.GetLastTimeUserReadChatAsync(request));
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (IssuerNotInRoomException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.IssuerNotInTheRoom));
        }
    }

    /// <inheritdoc cref="IRoomController.UpdateLastReadMessage"/>
    [HttpPut]
    public async Task<ActionResult> UpdateLastReadMessage(UpdateLastReadMessageModel model)
    {
        try
        {
            var request = new RequestToUpdateLastReadMessageByUser
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid,
                MessageId = model.MessageId
            };

            await _roomService.UpdateLastReadMessageByUser(request);

            return Ok();
        }
        catch (MessageNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.MessageNotFound));
        }
        catch (InvalidActionException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.InvalidAction));
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (IssuerNotInRoomException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.IssuerNotInTheRoom));
        }
    }

    /// <inheritdoc cref="IRoomController.KickMember"/>
    [HttpDelete]
    public async Task<ActionResult> KickMember(KickMemberModel model)
    {
        try
        {
            var request = new RequestToKickMember
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid,
                TargetHexId = model.TargetHexId
            };

            await _roomService.KickMemberAsync(request);

            try
            {
                return Ok();
            }
            finally
            {
                // Notifying users in the Chat hub that the user was kicked
                await _chatHub.Clients.Group(request.RoomGuid).SendAsync("UserKicked", request.TargetHexId);

                // Start a task to abort the connection with target user after some time
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(400));
                    _eventService.OnUserKicked(request.TargetHexId, request.RoomGuid);
                });
            }
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (IssuerNotInRoomException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.IssuerNotInTheRoom));
        }
        catch (NotEnoughPermissionsException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.NotEnoughPermissions));
        }
    }

    /// <inheritdoc cref="IRoomController.RenameRoom"/>
    [HttpPut]
    public async Task<ActionResult> RenameRoom(RenameRoomModel model)
    {
        try
        {
            var request = new RequestToRenameRoom()
            {
                IssuerId = this.GetIssuerId(),
                NewName = model.NewName,
                RoomGuid = model.RoomGuid
            };

            await _roomService.RenameAsync(request);

            try
            {
                return Ok();
            }
            finally
            {
                // Notifying users in the Chat hub that the room was renamed
                await _chatHub.Clients.Group(request.RoomGuid).SendAsync("RoomRenamed", request.NewName);
            }
        }
        catch (StringTooLongException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooLong));
        }
        catch (StringTooShortException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooShort));
        }
        catch (RoomExpiredException)
        {
            try { return BadRequest(new ErrorResponse(ErrorCode.RoomExpired)); }
            finally { await _roomService.DeleteAllExpiredAsync(); }
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (NotEnoughPermissionsException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.NotEnoughPermissions));
        }
    }

    /// <inheritdoc cref="IRoomController.DeleteRoom"/>
    [HttpDelete]
    public async Task<ActionResult> DeleteRoom(DeleteRoomModel model)
    {
        try
        {
            var request = new RequestToDeleteRoom
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid
            };

            await _roomService.DeleteAsync(request);

            try
            {
                return Ok();
            }
            finally
            {
                // Notifying users in the Chat hub that the room was deleted
                await _chatHub.Clients.Group(request.RoomGuid).SendAsync("RoomDeleted", request.RoomGuid);
            }
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (NotEnoughPermissionsException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.NotEnoughPermissions));
        }
    }
}