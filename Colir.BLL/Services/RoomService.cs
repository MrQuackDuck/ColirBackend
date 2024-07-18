using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.Exceptions;
using DAL.Entities;
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
        // Check if the issuer exists (otherwise, an exception will be thrown)
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

    /// <summary>
    /// Creates a new room
    /// </summary>
    /// <returns>Guid of created room</returns>
    public async Task<string> CreateAsync(RequestToCreateRoom request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        if (user.UserSettings.StatisticsEnabled)
        {
            user.UserStatistics.RoomsCreated += 1;
            _unitOfWork.UserStatisticsRepository.Update(user.UserStatistics);
        }

        if (request.ExpiryDate < DateTime.Now)
        {
            throw new ArgumentException("You can't create a room with an expiry date that is earlier than now!");
        }
        
        var roomToCreate = new Room
        {
            Name = request.Name,
            Guid = new Guid().ToString(),
            OwnerId = request.IssuerId,
            ExpiryDate = request.ExpiryDate
        };
        
        await _unitOfWork.RoomRepository.AddAsync(roomToCreate);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return roomToCreate.Guid;
    }
    
    /// <summary>
    /// Renames the room
    /// </summary>
    public async Task RenameAsync(RequestToRenameRoom request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        // Check if the issuer exists (otherwise, an exception will be thrown)
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        room.Name = request.NewName;
        _unitOfWork.RoomRepository.Update(room);
        await _unitOfWork.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    /// <summary>
    /// Deletes the room
    /// </summary>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the owner of the room</exception>
    public async Task DeleteAsync(RequestToDeleteRoom request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        // Check if the issuer's is not the owner of the room
        if (request.IssuerId != room.OwnerId)
        {
            throw new NotEnoughPermissionsException();
        }
        
        _unitOfWork.RoomRepository.Delete(room);
        await _unitOfWork.SaveChangesAsync();
        
        await transaction.CommitAsync();
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