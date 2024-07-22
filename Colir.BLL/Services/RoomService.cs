using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRoomCleanerFactory _roomCleanerFactory;
    
    public RoomService(IUnitOfWork unitOfWork, IMapper mapper, IRoomCleanerFactory roomCleanerFactory)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _roomCleanerFactory = roomCleanerFactory;
    }
    
    /// <summary>
    /// Gets the info about the room
    /// </summary>
    /// <exception cref="RoomExpiredException">Thrown when specified room is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when specified room wasn't found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room he is trying to get info</exception>
    // TODO: Return real amount of occupied/free room storage
    public async Task<RoomModel> GetRoomInfoAsync(RequestToGetRoomInfo request)
    {
        // Check if the issuer exists (otherwise, an exception will be thrown)
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        if (room.IsExpired()) throw new RoomExpiredException();
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }
        
        return _mapper.Map<RoomModel>(room);
    }

    /// <summary>
    /// Creates a new room
    /// + Increments the count of created rooms in user's statistics (if enabled in settings)
    /// </summary>
    /// <returns>Guid of created room</returns>
    /// <exception cref="ArgumentException">Thrown when the expiry date is not valid</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    public async Task<string> CreateAsync(RequestToCreateRoom request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        if (issuer.UserSettings.StatisticsEnabled)
        {
            issuer.UserStatistics.RoomsCreated += 1;
            _unitOfWork.UserStatisticsRepository.Update(issuer.UserStatistics);
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
    /// <exception cref="StringTooLongException">Thrown when the name for the room is too long</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room is too short</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the owner of the room</exception>
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
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the owner of the room</exception>
    public async Task DeleteAsync(RequestToDeleteRoom request)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        // Check if the issuer exists (otherwise, an exception will be thrown)
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
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

    /// <summary>
    /// Gets the last time when user read the chat
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    public async Task<DateTime> GetLastTimeUserReadChatAsync(RequestToGetLastTimeUserReadChat request)
    {
        // Check if the issuer exists (otherwise, an exception will be thrown)
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }
        
        var result = await _unitOfWork.LastTimeUserReadChatRepository.GetAsync(request.IssuerId, room.Id);
        return result.Timestamp;
    }

    /// <summary>
    /// Updates the last time user read the chat
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when issuer is not in the room</exception>
    public async Task UpdateLastTimeUserReadChatAsync(RequestToUpdateLastTimeUserReadChat request)
    {
        // Check if the issuer exists (otherwise, an exception will be thrown)
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }
        
        var transaction = _unitOfWork.BeginTransaction();

        try
        {
            var lastTimeUserReadChat =
                await _unitOfWork.LastTimeUserReadChatRepository.GetAsync(request.IssuerId, room.Id);
            lastTimeUserReadChat.Timestamp = DateTime.Now;
            _unitOfWork.LastTimeUserReadChatRepository.Update(lastTimeUserReadChat);
        }
        // If not found, create
        catch (NotFoundException)
        {
            var lastTimeUserReadChat = new LastTimeUserReadChat
            {
                UserId = request.IssuerId,
                RoomId = room.Id,
                Timestamp = DateTime.Now
            };

            await _unitOfWork.LastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChat);
        }

        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Joins a user to the room
    /// + Increments the count of joined rooms in user's statistics (if enabled in settings)
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    public async Task JoinMemberAsync(RequestToJoinRoom request)
    {
        
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var roomToJoin = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);

        if (issuer.UserSettings.StatisticsEnabled)
        {
            issuer.UserStatistics.RoomsJoined += 1;
            _unitOfWork.UserStatisticsRepository.Update(issuer.UserStatistics);
        }

        var transaction = _unitOfWork.BeginTransaction();
        
        issuer.JoinedRooms.Add(roomToJoin);
        _unitOfWork.UserRepository.Update(issuer);
        
        roomToJoin.JoinedUsers.Add(issuer);
        _unitOfWork.RoomRepository.Update(roomToJoin);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Kicks the user from the room
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when either the issuer wasn't found or the target wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when user is not the owner of the room</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    public async Task KickMemberAsync(RequestToKickMember request)
    {
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }
        
        // Check if the user is the owner of the room
        if (room.OwnerId != issuer.Id)
        {
            throw new NotEnoughPermissionsException();
        }
        
        var userToKick = await _unitOfWork.UserRepository.GetByHexIdAsync(request.TargetHexId);
        var roomToKickFrom = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        var transaction = _unitOfWork.BeginTransaction();
        
        userToKick.JoinedRooms.Remove(userToKick.JoinedRooms.First(r => r.Id == roomToKickFrom.Id));
        _unitOfWork.UserRepository.Update(userToKick);
        
        roomToKickFrom.JoinedUsers.Remove(roomToKickFrom.JoinedUsers.First(u => u.Id == userToKick.Id));
        _unitOfWork.RoomRepository.Update(roomToKickFrom);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Leaves the room
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    public async Task LeaveAsync(RequestToLeaveFromRoom request)
    {
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }
        
        var roomToKickFrom = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        var transaction = _unitOfWork.BeginTransaction();
        
        issuer.JoinedRooms.Remove(issuer.JoinedRooms.First(r => r.Id == roomToKickFrom.Id));
        _unitOfWork.UserRepository.Update(issuer);
        
        roomToKickFrom.JoinedUsers.Remove(roomToKickFrom.JoinedUsers.First(u => u.Id == issuer.Id));
        _unitOfWork.RoomRepository.Update(roomToKickFrom);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Returns an object that represents the room cleaner
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the user is not the owner of the room</exception>
    public async Task<IRoomCleaner> ClearRoomAsync(RequestToClearRoom request)
    {
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        if (room.OwnerId != issuer.Id)
        {
            throw new NotEnoughPermissionsException();
        }

        return _roomCleanerFactory.GetRoomCleaner(request.RoomGuid);
    }
}