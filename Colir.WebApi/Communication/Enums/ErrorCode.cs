﻿namespace Colir.Communication.Enums;

/// <summary>
/// Lists all possible errors a client can get in an error response
/// </summary>
public enum ErrorCode
{
    AttachmentNotFound,
    EmptyMessage,
    InvalidAction,
    InvalidDate,
    IssuerNotInTheRoom,
    MessageNotFound,
    ModelNotValid,
    NotEnoughPermissions,
    NotEnoughSpace,
    NotFound,
    ReactionAlreadySet,
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
    YouAreNotConnectedToVoiceChannel,
    YouAreAlreadyConnectedToVoiceChannel,
}