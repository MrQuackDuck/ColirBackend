using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;

namespace Colir.BLL.Interfaces;

public interface IRoomService
{
    /// <summary>
    /// Gets the info about the room
    /// </summary>
    /// <exception cref="RoomExpiredException">Thrown when specified room is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when specified room wasn't found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room he is trying to get info</exception>
    Task<RoomModel> GetRoomInfoAsync(RequestToGetRoomInfo request);

    /// <summary>
    /// Creates a new room
    /// + Increments the count of created rooms in user's statistics (if enabled in settings)
    /// </summary>
    /// <returns>Guid of created room</returns>
    /// <exception cref="ArgumentException">Thrown when the expiry date is not valid</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="StringTooLongException">Thrown when the name for the room is too long</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room is too short</exception>
    Task<string> CreateAsync(RequestToCreateRoom request);

    /// <summary>
    /// Renames the room
    /// </summary>
    /// <exception cref="StringTooLongException">Thrown when the name for the room is too long</exception>
    /// <exception cref="StringTooShortException">Thrown when the name for the room is too short</exception>
    /// <exception cref="RoomExpiredException">Thrown when specified room is expired</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the owner of the room</exception>
    Task RenameAsync(RequestToRenameRoom request);

    /// <summary>
    /// Deletes the room
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the owner of the room</exception>
    Task DeleteAsync(RequestToDeleteRoom request);

    /// <summary>
    /// Deletes all expired rooms
    /// </summary>
    Task DeleteAllExpiredAsync();

    /// <summary>
    /// Gets the last time when user read the chat
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    Task<DateTime> GetLastTimeUserReadChatAsync(RequestToGetLastTimeUserReadChat request);

    /// <summary>
    /// Updates the last time user read the chat
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when issuer is not in the room</exception>
    Task UpdateLastTimeUserReadChatAsync(RequestToUpdateLastTimeUserReadChat request);

    /// <summary>
    /// Joins a user to the room
    /// + Increments the count of joined rooms in user's statistics (if enabled in settings)
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<RoomModel> JoinMemberAsync(RequestToJoinRoom request);

    /// <summary>
    /// Kicks the user from the room
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when either the issuer wasn't found or the target wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when user is not the owner of the room</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    Task KickMemberAsync(RequestToKickMember request);

    /// <summary>
    /// Leaves the room
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    Task LeaveAsync(RequestToLeaveFromRoom request);

    /// <summary>
    /// Returns an object that represents the room cleaner
    /// </summary>
    /// <exception cref="RoomNotFoundException">Thrown when the room was not found</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the user is not the owner of the room</exception>
    Task<IRoomCleaner> ClearRoomAsync(RequestToClearRoom request);
}