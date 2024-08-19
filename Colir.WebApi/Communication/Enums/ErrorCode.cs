namespace Colir.Communication.Enums;

/// <summary>
/// Lists all possible errors a client can get in error response
/// </summary>
public enum ErrorCode
{
    AttachmentNotFound,
    EmptyMessage,
    InvalidActionException,
    InvalidDate,
    IssuerNotInTheRoom,
    MessageNotFound,
    ModelNotValid,
    NotEnoughPermissions,
    NotEnoughSpace,
    NotFound,
    ReactionNotFound,
    RoomExpired,
    RoomNotFound,
    StringWasTooLong,
    StringWasTooShort,
    UserAlreadyRegistered,
    UserAlreadyInRoom,
    UserNotFound,
    YouAreNotAuthorOfMessage,
    YouAreNotAuthorOfReaction,
    YouAreNotConnectedToVoiceChannel
}