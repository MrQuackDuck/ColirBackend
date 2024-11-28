using Colir.Communication.Enums;
using Colir.Communication.RequestModels.Chat;
using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IChatHub
{
    /// <summary>
    /// Gets last sent messages from a certain room
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> GetMessages(GetLastMessagesModel model);

    /// <summary>
    /// Gets surrounding messages from a certain message
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.MessageNotFound"/> code is returned when the message wasn't found
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> GetSurroundingMessages(GetSurroundingMessagesModel model);

    /// <summary>
    /// Gets messages range from a certain room
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.MessageNotFound"/> code is returned when the message wasn't found
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> GetMessagesRange(GetMessagesRangeModel model);

    /// <summary>
    /// Gets unread replies to your messages since the last time the issuer read the chat
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.MessageNotFound"/> code is returned when the message wasn't found
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> GetUnreadRepliesAsync();

    /// <summary>
    /// Gets the message by its id
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.MessageNotFound"/> code is returned when the message wasn't found
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> GetMessageById(GetMessageByIdModel model);

    /// <summary>
    /// Sends a message to the room. Notifies others with the "ReceiveMessage" signal
    /// Also notifies members of the room with the "RoomSizeChanged" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.NotEnoughSpace"/> code is returned when the room has not enough space to store attachments from the request
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> SendMessage(SendMessageModel model);

    /// <summary>
    /// Edits the message in the room. Notifies others with the "MessageEdited" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.EmptyMessage"/> code is returned when the new message content is empty
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.YouAreNotAuthorOfMessage"/> code is returned when the user is not the author of the message they try to edit
    /// </summary>
    Task<SignalRHubResult> EditMessage(EditMessageModel model);

    /// <summary>
    /// Deletes the message. Notifies others with the "MessageDeleted" signal
    /// Also notifies members of the room with the "RoomSizeChanged" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.YouAreNotAuthorOfMessage"/> code is returned when the user is not the author of the message they try to delete
    /// </summary>
    Task<SignalRHubResult> DeleteMessage(DeleteMessageModel model);

    /// <summary>
    /// Adds a reaction to the message. Notifies others with the "MessageGotReaction" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> AddReactionOnMessage(AddReactionOnMessageModel model);

    /// <summary>
    /// Removes the reaction from the message. Notifies others with the "MessageLostReaction" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code is returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code is returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code is returned when the room is expired (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.YouAreNotAuthorOfReaction"/> code is returned when the user is not the author of the reaction they try to remove
    /// </summary>
    Task<SignalRHubResult> RemoveReactionFromMessage(RemoveReactionFromMessageModel model);
}