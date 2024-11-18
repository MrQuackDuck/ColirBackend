﻿using Colir.BLL.Models;
using Colir.Communication.RequestModels.Room;
using Colir.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IRoomController
{
    /// <summary>
    /// Gets the info about the room
    /// </summary>
    Task<ActionResult<RoomModel>> GetRoomInfo(GetRoomInfoModel model);

    /// <summary>
    /// Creates a room
    /// </summary>
    Task<ActionResult<string>> CreateRoom(CreateRoomModel model);

    /// <summary>
    /// Joins the user in a room. Notifies users in <see cref="ChatHub"/> with the "UserJoined" signal
    /// </summary>
    Task<ActionResult<RoomModel>> JoinRoom(JoinRoomModel model);

    /// <summary>
    /// Leaves the user from the room. Notifies users in <see cref="ChatHub"/> with the "UserLeft" signal
    /// </summary>
    Task<ActionResult> LeaveRoom(LeaveRoomModel model);

    /// <summary>
    /// Gets the last time a user read chat in certain room
    /// </summary>
    Task<ActionResult<DateTime>> GetLastTimeReadChatModel(GetLastTimeReadChatModel model);

    /// <summary>
    /// Updates the last time the user read chat in certain room
    /// </summary>
    Task<ActionResult> UpdateLastTimeReadChat(UpdateLastTimeReadChatModel model);

    /// <summary>
    /// Kicks the member from the room. Notifies users in <see cref="ChatHub"/> with the "UserKicked" signal
    /// </summary>
    Task<ActionResult> KickMember(KickMemberModel model);

    /// <summary>
    /// Renames the room. Notifies users in <see cref="ChatHub"/> with the "RoomRenamed" signal
    /// </summary>
    Task<ActionResult> RenameRoom(RenameRoomModel model);

    /// <summary>
    /// Deletes the room. Notifies users in <see cref="ChatHub"/> with the "RoomDeleted" signal
    /// </summary>
    Task<ActionResult> DeleteRoom(DeleteRoomModel model);
}