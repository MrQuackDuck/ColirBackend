namespace Colir.Communication;

/// <summary>
/// Lists all possible errors a client can get in error response
/// </summary>
public enum ErrorCode
{
    AttachmentNotFound,
    EmptyMessage,
    InvalidDate,
    InvalidActionException,
    IssuerNotInTheRoom,
    MessageNotFound,
    NotEnoughPermissions,
    NotEnoughSpace,
    NotFound,
    ReactionNotFound,
    RoomExpired,
    RoomNotFound,
    StringWasTooLong,
    StringWasTooShort,
    UserAlreadyRegistered,
    UserNotFound
}