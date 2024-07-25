namespace Colir.Communication;

/// <summary>
/// Lists all possible errors a client can experience
/// </summary>
public enum ErrorCode
{
    StringWasTooLong,
    StringWasTooShort,
    IssuerNotInTheRoom,
    NotEnoughPermissions,
    InvalidActionException,
    NotFound,
    AttachmentNotFound,
    MessageNotFound,
    ReactionNotFound,
    RoomNotFound,
    UserNotFound
}