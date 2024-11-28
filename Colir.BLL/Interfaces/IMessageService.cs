using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using Colir.BLL.RequestModels.Room;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;

namespace Colir.BLL.Interfaces;

public interface IMessageService
{
    /// <summary>
    /// Gets last sent messages from a certain room
    /// </summary>
    /// <param name="request">The request object</param>
    /// <param name="request.Count">Count of messages to take</param>
    /// <param name="request.Skip">Count of messages to skip</param>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task<List<MessageModel>> GetLastMessagesAsync(RequestToGetLastMessages request);

    /// <summary>
    /// Gets surrounding messages from a certain message
    /// </summary>
    /// <param name="request">The request object</param>
    /// <param name="request.Count">Count of messages to take</param>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found</exception>
    Task <List<MessageModel>> GetSurroundingMessagesAsync(RequestToGetSurroundingMessages request);

    /// <summary>
    /// Gets messages range from a certain room
    /// </summary>
    /// <param name="request">The request object</param>
    /// <param name="request.StartId">Id of the first message in the range</param>
    /// <param name="request.EndId">Id of the last message in the range</param>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="MessageNotFoundException">Thrown when the message wasn't found or the startId or endId is not in the room</exception>
    /// <exception cref="ArgumentException">Thrown when the startId or endId is less than zero</exception>
    Task <List<MessageModel>> GetMessagesRangeAsync(RequestToGetMessagesRange request);

    /// <summary>
    /// Gets all unread messages that have a reply to messages of the issuer
    /// Unreadness is determined by the last time the issuer read the chat
    /// </summary>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="NotFoundException">Thrown when the issuer hasn't ever read the chat</exception>
    Task<List<MessageModel>> GetUnreadRepliesAsync(RequestToGetUnreadReplies request);

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
    /// <exception cref="AttachmentNotFoundException">Thrown when the attachment wasn't found or it's not in the room the message is being sent to</exception>
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
    /// <exception cref="InvalidActionException">Thrown when the reaction with the same symbol is already set</exception>
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