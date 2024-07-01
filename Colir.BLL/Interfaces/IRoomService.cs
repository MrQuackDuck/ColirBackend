using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;

namespace Colir.BLL.Interfaces;

public interface IRoomService
{
    Task<RoomModel> GetRoomInfoAsync(RequestToGetRoomInfo request);

    Task<string> CreateAsync(RequestToCreateRoom request); // - Returns room GUID
    Task RenameAsync(RequestToRenameRoom request);
    Task DeleteAsync(RequestToDeleteRoom request);

    Task<DateTime> GetLastTimeUserReadChatAsync(RequestToGetLastTimeUserReadChat request); // - Returns last time user read chat
    Task UpdateLastTimeUserReadChatAsync(RequestToUpdateLastTimeUserReadChat request);

    Task JoinMemberAsync(RequestToJoinRoom request);
    Task KickMemberAsync(RequestToKickMember request);

    Task<IClearProcess> ClearRoom(RequestToClearRoom request);
}