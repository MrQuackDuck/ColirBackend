namespace Colir.Communication;

/// <summary>
/// Lists all possible errors a client can experience
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
    UserAlreadyInRegistrationQueue,
    UserNotFound
}