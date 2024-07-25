﻿using Colir.BLL.Models;
using Colir.Communication.RequestModels.Room;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IRoomController
{
    Task<ActionResult<RoomModel>> GetRoomInfo(GetRoomInfoModel model);
    Task<ActionResult<RoomModel>> CreateRoom(CreateRoomModel model);
    Task<ActionResult<RoomModel>> JoinRoom(JoinRoomModel model);
    Task<ActionResult> LeaveRoom(LeaveRoomModel model);
    Task<ActionResult<DateTime>> GetLastTimeReadChatModel(GetLastTimeReadChatModel model);
    Task<ActionResult> UpdateLastTimeReadChat(UpdateLastTimeReadChatModel model);
    Task<ActionResult> KickMember(KickMemberModel model);
    ActionResult ChangeRoomName(RenameRoomModel model);
    ActionResult DeleteRoom(DeleteRoomModel model);
}