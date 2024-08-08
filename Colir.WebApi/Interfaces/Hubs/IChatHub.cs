using Colir.Communication.Enums;
using Colir.Communication.RequestModels.Chat;
using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IChatHub
{
    /// <summary>
    /// Gets last sent messages from certain room
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> GetMessages(GetLastMessagesModel model);

    /// <summary>
    /// Sends a message to the room. Notifies others with "ReceiveMessage" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code returned when the model is not valid
    /// An error with <see cref="ErrorCode.NotEnoughSpace"/> code returned when the room has not enough space to store attachments from the request
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> SendMessage(SendMessageModel model);

    /// <summary>
    /// Edits the message in the room. Notifies others with "MessageEdited" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code returned when the model is not valid
    /// An error with <see cref="ErrorCode.EmptyMessage"/> code returned when the new message content is empty
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code returned when the room is expired (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.YouAreNotAuthorOfMessage"/> code returned when the user is not the author of the message he tries to edit
    /// </summary>
    Task<SignalRHubResult> EditMessage(EditMessageModel model);

    /// <summary>
    /// Deletes the message. Notifies others with "MessageDeleted" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code returned when the room is expired (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.YouAreNotAuthorOfMessage"/> code returned when the user is not the author of the message he tries to delete
    /// </summary>
    Task<SignalRHubResult> DeleteMessage(DeleteMessageModel model);

    /// <summary>
    /// Adds a reaction on the message. Notifies others with "MessageGotReaction" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code returned when the room is expired (+ disconnects from the hub)
    /// </summary>
    Task<SignalRHubResult> AddReactionOnMessage(AddReactionOnMessageModel model);

    /// <summary>
    /// Removes the reaction from the message. Notifies others with "MessageLostReaction" signal
    /// An error with <see cref="ErrorCode.ModelNotValid"/> code returned when the model is not valid
    /// An error with <see cref="ErrorCode.IssuerNotInTheRoom"/> code returned when the issuer is not in the room (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.RoomExpired"/> code returned when the room is expired (+ disconnects from the hub)
    /// An error with <see cref="ErrorCode.YouAreNotAuthorOfReaction"/> code returned when the user is not the author of the reaction he tries to remove
    /// </summary>
    Task<SignalRHubResult> RemoveReactionFromMessage(RemoveReactionFromMessageModel model);
}