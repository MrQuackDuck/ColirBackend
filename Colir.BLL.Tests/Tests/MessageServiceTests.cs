using Colir.BLL.Tests.Interfaces;

namespace Colir.BLL.Tests.Tests;

public class MessageServiceTests : IMessageServiceTests
{
    [Test]
    public void GetLastMessagesAsync_ReturnsLastMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessagesAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessagesAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessagesAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SendAsync_SendsMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SendAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SendAsync_ThrowsMessageNotFoundException_WhenNotExistingReplyMessageIdProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SendAsync_ThrowsAttachmentNotFoundException_WhenNotExistingAttachmentIdProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SendAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SendAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void EditAsync_EditsMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void EditAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddReaction_AddsReaction()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RemoveReaction_RemovesReaction()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RemoveReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RemoveReaction_ThrowsReactionNotFoundException_WhenReactionWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfReaction()
    {
        throw new NotImplementedException();
    }
}