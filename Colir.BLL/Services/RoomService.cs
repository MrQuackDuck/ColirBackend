using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.Exceptions;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Gets the info about the room
    /// </summary>
    /// <exception cref="RoomExpiredException">Thrown when specified room is expired</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not in the room he is trying to get info</exception>
    // TODO: Return real amount of occupied/free room storage
    public async Task<RoomModel> GetRoomInfoAsync(RequestToGetRoomInfo request)
    {
        // Check if the issuer exists (otherwise, will throw an exception)
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        if (room.ExpiryDate < DateTime.Now) throw new RoomExpiredException();
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new NotEnoughPermissionsException();
        }
        
        return _mapper.Map<RoomModel>(room);
    }

    public async Task<string> CreateAsync(RequestToCreateRoom request)
    {
        throw new NotImplementedException();
    }

    public async Task RenameAsync(RequestToRenameRoom request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(RequestToDeleteRoom request)
    {
        throw new NotImplementedException();
    }

    public async Task<DateTime> GetLastTimeUserReadChatAsync(RequestToGetLastTimeUserReadChat request)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateLastTimeUserReadChatAsync(RequestToUpdateLastTimeUserReadChat request)
    {
        throw new NotImplementedException();
    }

    public async Task JoinMemberAsync(RequestToJoinRoom request)
    {
        throw new NotImplementedException();
    }

    public async Task KickMemberAsync(RequestToKickMember request)
    {
        throw new NotImplementedException();
    }

    public async Task<IClearProcess> ClearRoom(RequestToClearRoom request)
    {
        throw new NotImplementedException();
    }
}