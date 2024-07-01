using Colir.BLL.Tests.Interfaces;

namespace Colir.BLL.Tests.Tests;

public class MessageServiceTests : IMessageServiceTests
{
    [Test]
    public async Task GetLastMessagesAsync_ReturnsLastMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessagesAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_SendsMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_AddsToStatistics_WhenItsEnabled()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_ThrowsMessageNotFoundException_WhenNotExistingReplyMessageIdProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_ThrowsAttachmentNotFoundException_WhenNotExistingAttachmentIdProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SendAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_EditsMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task EditAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_AddsReaction()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task AddReaction_AddsToStatistics_WhenItsEnabled()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_RemovesReaction()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsReactionNotFoundException_WhenReactionWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RemoveReaction_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotAuthorOfReaction()
    {
        throw new NotImplementedException();
    }
}