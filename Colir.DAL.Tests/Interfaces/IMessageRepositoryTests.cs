﻿namespace Colir.DAL.Tests.Interfaces;

public interface IMessageRepositoryTests
{
    Task GetAllAsync_ReturnsAllMessages();

    Task GetLastMessages_ReturnsLastMessages();
    Task GetLastMessages_ThrowsMessageNotFoundException_WhenRoomWasNotFound();
    Task GetLastMessages_ThrowsArgumentException_WhenCountLessThanZero();
    Task GetLastMessages_ThrowsArgumentException_WhenSkipLessThanZero();
    Task GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired();

    Task GetMessagesRange_ReturnsMessagesRange();
    Task GetMessagesRange_ThrowsMessageNotFoundException_WhenRoomWasNotFound();
    Task GetMessagesRange_ThrowsArgumentException_WhenStartIdLessThanZero();
    Task GetMessagesRange_ThrowsArgumentException_WhenEndIdLessThanZero();
    Task GetMessagesRange_ThrowsRoomExpiredException_WhenRoomExpired();

    Task GetSurroundingMessages_ThrowsArgumentException_WhenCountLessThanZero();
    Task GetSurroundingMessages_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
    Task GetSurroundingMessages_ThrowsRoomExpiredException_WhenRoomExpired();

    Task GetByIdAsync_ReturnsMessage_WhenFound();
    Task GetByIdAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound();

    Task AddAsync_AddsNewMessage();
    Task AddAsync_AppliesAttachmentsToMessage();
    Task AddAsync_AppliesReactionsToMessage();
    Task AddAsync_ThrowsUserNotFoundException_WhenAuthorWasNotFound();
    Task AddAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();
    Task AddAsync_ThrowsMessageNotFoundException_WhenRepliedMessageWasNotFound();
    Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    Task Delete_DeletesMessage();
    Task Delete_DeletesAllRelatedReactions();
    Task Delete_DeletesAllRelatedAttachments();
    Task Delete_NotDeletesAnyOtherMessages();
    Task Delete_ThrowsMessageNotFoundException_WhenMessageDoesNotExist();
    Task Delete_ThrowsRoomExpiredException_WhenRoomExpired();

    Task DeleteByIdAsync_DeletesMessage();
    Task DeleteByIdAsync_DeletesAllRelatedReactions();
    Task DeleteByIdAsync_DeletesAllRelatedAttachments();
    Task DeleteByIdAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFoundById();
    Task DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    Task Update_UpdatesMessage();
    Task Update_ThrowsMessageNotFoundException_WhenMessageDoesNotExist();
    Task Update_ThrowsRoomExpiredException_WhenRoomExpired();
}