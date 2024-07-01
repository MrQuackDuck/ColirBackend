using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class MessageRepositoryTests : IMessageRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessagesWithAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ReturnsLastMessagesWithReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ReturnsMessage_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesAttachmentsToMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesReactionsToMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenAuthorWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenRepliedMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_NotDeletesAnyOtherMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}