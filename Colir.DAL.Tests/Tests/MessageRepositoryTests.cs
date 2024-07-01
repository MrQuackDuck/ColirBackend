using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class MessageRepositoryTests : IMessageRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ReturnsLastMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ReturnsLastMessagesWithAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ReturnsLastMessagesWithReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ThrowsArgumentExcpetion_WhenCountLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ThrowsArgumentExcpetion_WhenSkipLessThanZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ReturnsMessage_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AppliesAttachmentsToMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AppliesReactionsToMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenAuthorWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenRepliedMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_NotDeletesAnyOtherMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenMessageDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}