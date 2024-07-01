using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class RoomService : IRoomService
{
    private IUnitOfWork _unitOfWork;
    
    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public Task<RoomModel> GetRoomInfoAsync(RequestToGetRoomInfo request)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateAsync(RequestToCreateRoom request)
    {
        throw new NotImplementedException();
    }

    public Task RenameAsync(RequestToRenameRoom request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(RequestToDeleteRoom request)
    {
        throw new NotImplementedException();
    }

    public Task<DateTime> GetLastTimeUserReadChatAsync(RequestToGetLastTimeUserReadChat request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLastTimeUserReadChatAsync(RequestToUpdateLastTimeUserReadChat request)
    {
        throw new NotImplementedException();
    }

    public Task JoinMemberAsync(RequestToJoinRoom request)
    {
        throw new NotImplementedException();
    }

    public Task KickMemberAsync(RequestToKickMember request)
    {
        throw new NotImplementedException();
    }

    public Task<IClearProcess> ClearRoom(RequestToClearRoom request)
    {
        throw new NotImplementedException();
    }
}