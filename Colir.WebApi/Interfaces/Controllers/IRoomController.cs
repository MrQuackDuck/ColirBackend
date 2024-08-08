using Colir.BLL.Models;
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
    /// Joins user a room. Notifies users in <see cref="ChatHub"/> with "UserJoined" signal
    /// </summary>
    Task<ActionResult<RoomModel>> JoinRoom(JoinRoomModel model);

    /// <summary>
    /// Leaves the user from the room. Notifies users in <see cref="ChatHub"/> with "UserLeft" signal
    /// </summary>
    Task<ActionResult> LeaveRoom(LeaveRoomModel model);

    /// <summary>
    /// Gets the last time user read chat in certain room
    /// </summary>
    Task<ActionResult<DateTime>> GetLastTimeReadChatModel(GetLastTimeReadChatModel model);

    /// <summary>
    /// Updates the last time user read chat in certain room
    /// </summary>
    Task<ActionResult> UpdateLastTimeReadChat(UpdateLastTimeReadChatModel model);

    /// <summary>
    /// Kicks a member from the room. Notifies users in <see cref="ChatHub"/> with "UserKicked" signal
    /// </summary>
    Task<ActionResult> KickMember(KickMemberModel model);

    /// <summary>
    /// Renames the room. Notifies users in <see cref="ChatHub"/> with "RoomRenamed" signal
    /// </summary>
    Task<ActionResult> RenameRoom(RenameRoomModel model);

    /// <summary>
    /// Deletes the room. Notifies users in <see cref="ChatHub"/> with "RoomDeleted" signal
    /// </summary>
    Task<ActionResult> DeleteRoom(DeleteRoomModel model);
}