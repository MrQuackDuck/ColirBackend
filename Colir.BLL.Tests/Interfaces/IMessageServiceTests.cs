﻿namespace Colir.BLL.Tests.Interfaces;

public interface IMessageServiceTests
{
	Task GetLastMessagesAsync_ReturnsLastMessages();
	Task GetLastMessagesAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
    Task GetLastMessagesAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();
    Task GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenCountLessThanZero();
    Task GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenSkipLessThanZero();
	Task GetLastMessagesAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();

    Task SendAsync_SendsMessage();
    Task SendAsync_AddsToStatistics_WhenItsEnabled();
	Task SendAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
    Task SendAsync_ThrowsMessageNotFoundException_WhenNotExistingReplyMessageIdProvided();
    Task SendAsync_ThrowsAttachmentNotFoundException_WhenNotExistingAttachmentIdProvided();
    Task SendAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();
	Task SendAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();

	Task EditAsync_EditsMessage();
    Task EditAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();
	Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage();

	Task Delete_DeletesMessage();
    Task Delete_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	Task Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();
	Task Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage();

	Task AddReaction_AddsReaction();
	Task AddReaction_AddsToStatistics_WhenItsEnabled();
	Task AddReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	Task AddReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();

	Task RemoveReaction_RemovesReaction();
	Task RemoveReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	Task RemoveReaction_ThrowsReactionNotFoundException_WhenReactionWasNotFound();
	Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();
	Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfReaction();
}