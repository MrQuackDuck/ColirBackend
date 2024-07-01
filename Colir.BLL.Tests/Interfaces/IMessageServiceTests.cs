namespace Colir.BLL.Tests.Interfaces;

public interface IMessageServiceTests
{
	void GetLastMessagesAsync_ReturnsLastMessages();
	void GetLastMessagesAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
    void GetLastMessagesAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();
    void GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenCountLessThanZero();
    void GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenSkipLessThanZero();
	void GetLastMessagesAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();

    void SendAsync_SendsMessage();
	void SendAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
    void SendAsync_ThrowsMessageNotFoundException_WhenNotExistingReplyMessageIdProvided();
    void SendAsync_ThrowsAttachmentNotFoundException_WhenNotExistingAttachmentIdProvided();
    void SendAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();
	void SendAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();

	void EditAsync_EditsMessage();
    void EditAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	void EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();
	void EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage();

	void Delete_DeletesMessage();
    void Delete_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	void Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();
	void Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage();

	void AddReaction_AddsReaction();
	void AddReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	void AddReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();

	void RemoveReaction_RemovesReaction();
	void RemoveReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound();
	void RemoveReaction_ThrowsReactionNotFoundException_WhenReactionWasNotFound();
	void RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom();
	void RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfReaction();
}