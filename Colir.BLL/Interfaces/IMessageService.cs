using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;

namespace Colir.BLL.Interfaces;

public interface IMessageService
{
    /// <summary>
    /// Gets last sent messages from certain room
    /// </summary>
    /// <param name="request">The request object</param>
    /// <param name="request.Count">Count of messages to take</param>
    /// <param name="request.Skip">Count of messages to skip</param>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<List<MessageModel>> GetLastMessagesAsync(RequestToGetLastMessages request);

    /// <summary>
    /// Gets the message by id
    /// </summary>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<MessageModel> GetMessageById(RequestToGetMessage request);

    /// <summary>
    /// Sends the message in the room
    /// + Increments the count of sent messages in user's statistics (if enabled in settings)
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the message content is empty</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<MessageModel> SendAsync(RequestToSendMessage request);

    /// <summary>
    /// Edits the sent message
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the message content is empty</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the author of the message he is trying to edit</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<MessageModel> EditAsync(RequestToEditMessage request);

    /// <summary>
    /// Deletes the message
    /// </summary>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the author of the message he is trying to delete</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task DeleteAsync(RequestToDeleteMessage request);

    /// <summary>
    /// Adds a reaction to the message
    /// + Increments the count of set reactions in user's statistics (if enabled in settings)
    /// </summary>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<MessageModel> AddReaction(RequestToAddReactionOnMessage request);

    /// <summary>
    /// Removes the reaction from the message
    /// </summary>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="NotEnoughPermissionsException">Thrown when the issuer is not the author of the reaction he is trying to remove</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<MessageModel> RemoveReaction(RequestToRemoveReactionFromMessage request);
}