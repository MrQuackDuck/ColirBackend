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

    /// <inheritdoc cref="IRoomService.GetRoomInfoAsync"/>
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

        var resultModel = _mapper.Map<RoomModel>(room);

        // Apply amount of occupied/free room storage
        resultModel.FreeMemoryInBytes = _unitOfWork.RoomRepository.RoomFileManager.GetFreeStorageSize(room.Guid);
        resultModel.UsedMemoryInBytes = _unitOfWork.RoomRepository.RoomFileManager.GetOccupiedStorageSize(room.Guid);

        return resultModel;
    }

    /// <inheritdoc cref="IRoomService.CreateAsync"/>
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
            Guid = Guid.NewGuid().ToString(),
            OwnerId = request.IssuerId,
            ExpiryDate = request.ExpiryDate
        };

        await _unitOfWork.RoomRepository.AddAsync(roomToCreate);

        await _unitOfWork.SaveChangesAsync();

        // Joining the issuer to the room
        issuer.JoinedRooms.Add(roomToCreate);
        _unitOfWork.UserRepository.Update(issuer);

        roomToCreate.JoinedUsers.Add(issuer);
        _unitOfWork.RoomRepository.Update(roomToCreate);

        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return roomToCreate.Guid;
    }

    /// <inheritdoc cref="IRoomService.RenameAsync"/>
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

    /// <inheritdoc cref="IRoomService.DeleteAsync"/>
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

    /// <inheritdoc cref="IRoomService.DeleteAllExpiredAsync"/>
    public async Task DeleteAllExpiredAsync()
    {
        var transaction = _unitOfWork.BeginTransaction();

        try { _unitOfWork.RoomRepository.DeleteAllExpired(); }
        catch { /* ignored */ }

        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <inheritdoc cref="IRoomService.GetLastTimeUserReadChatAsync"/>
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

    /// <inheritdoc cref="IRoomService.UpdateLastTimeUserReadChatAsync"/>
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

    /// <inheritdoc cref="IRoomService.JoinMemberAsync"/>
    public async Task<RoomModel> JoinMemberAsync(RequestToJoinRoom request)
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

        return _mapper.Map<RoomModel>(roomToJoin);
    }

    /// <inheritdoc cref="IRoomService.KickMemberAsync"/>
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

    /// <inheritdoc cref="IRoomService.LeaveAsync"/>
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

    /// <inheritdoc cref="IRoomService.ClearRoomAsync"/>
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