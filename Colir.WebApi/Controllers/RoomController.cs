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
using Colir.Interfaces.Controllers;
using Colir.Misc.ExtensionMethods;
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

    public RoomController(IRoomService roomService, IHubContext<ChatHub> chatHub)
    {
        _roomService = roomService;
        _chatHub = chatHub;
    }

    /// <inheritdoc cref="IRoomController.GetRoomInfo"/>
    [HttpGet]
    public async Task<ActionResult<RoomModel>> GetRoomInfo(GetRoomInfoModel model)
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
            var request = new RequestToCreateRoom
            {
                IssuerId = this.GetIssuerId(),
                Name = model.Name,
                ExpiryDate = model.ExpiryDate
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

            try
            {
                return Ok(await _roomService.JoinMemberAsync(request));
            }
            finally
            {
                // Notifying users in the Chat hub that a new user has joined
                await _chatHub.Clients.Group(model.RoomGuid).SendAsync("UserJoined", this.GetIssuerHexId());
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

            try
            {
                return Ok();
            }
            finally
            {
                // Notifying users in the Chat hub that the user left
                await _chatHub.Clients.Group(request.RoomGuid).SendAsync("UserLeft", this.GetIssuerHexId());
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
    }

    /// <inheritdoc cref="IRoomController.GetLastTimeReadChatModel"/>
    [HttpGet]
    public async Task<ActionResult<DateTime>> GetLastTimeReadChatModel(GetLastTimeReadChatModel model)
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

    /// <inheritdoc cref="IRoomController.UpdateLastTimeReadChat"/>
    [HttpPut]
    public async Task<ActionResult> UpdateLastTimeReadChat(UpdateLastTimeReadChatModel model)
    {
        try
        {
            var request = new RequestToUpdateLastTimeUserReadChat
            {
                IssuerId = this.GetIssuerId(),
                RoomGuid = model.RoomGuid
            };

            await _roomService.UpdateLastTimeUserReadChatAsync(request);

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