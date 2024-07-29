namespace Colir.Communication;

/// <summary>
/// Lists all possible errors a client can get in error response
/// </summary>
public enum ErrorCode
{
    AttachmentNotFound,
    InvalidDate,
    InvalidActionException,
    IssuerNotInTheRoom,
    MessageNotFound,
    NotEnoughPermissions,
    NotFound,
    ReactionNotFound,
    RoomExpired,
    RoomNotFound,
    StringWasTooLong,
    StringWasTooShort,
    UserAlreadyRegistered,
    UserNotFound
}