using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.Communication.Enums;
using Colir.Communication.RequestModels.Room;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using Colir.Interfaces.Controllers;
using Colir.Misc.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class RoomController : ControllerBase, IRoomController
{
    private readonly IRoomService _roomService;
    
    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
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
            return BadRequest(new ErrorResponse(ErrorCode.RoomExpired));
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

            return Ok(await _roomService.JoinMemberAsync(request));
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
        catch (NotEnoughPermissionsException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.NotEnoughPermissions));
        }
    }

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

            return Ok();
        }
        catch (StringTooLongException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooLong));
        }
        catch (StringTooShortException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooShort));
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
        catch (NotEnoughPermissionsException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.NotEnoughPermissions));
        }
    }
}