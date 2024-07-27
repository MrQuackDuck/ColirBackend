using Colir.BLL.Models;
using Colir.Communication.RequestModels.Room;
using Colir.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class RoomController : ControllerBase, IRoomController
{
    [HttpGet]
    public async Task<ActionResult<RoomModel>> GetRoomInfo(GetRoomInfoModel model)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<RoomModel>> CreateRoom(CreateRoomModel model)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<RoomModel>> JoinRoom(JoinRoomModel model)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult> LeaveRoom(LeaveRoomModel model)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<DateTime>> GetLastTimeReadChatModel(GetLastTimeReadChatModel model)
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateLastTimeReadChat(UpdateLastTimeReadChatModel model)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<ActionResult> KickMember(KickMemberModel model)
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    public ActionResult ChangeRoomName(RenameRoomModel model)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public ActionResult DeleteRoom(DeleteRoomModel model)
    {
        throw new NotImplementedException();
    }
}