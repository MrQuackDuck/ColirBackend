namespace Colir.DAL.Tests.Interfaces;

public interface IMessageRepositoryTests
{
    void GetAllAsync_ReturnsAllMessages();

    void GetLastMessages_ReturnsLastMessages();
    void GetLastMessages_ReturnsLastMessagesWithAttachments();
    void GetLastMessages_ReturnsLastMessagesWithReactions();
    void GetLastMessages_ThrowsNotFoundException_WhenRoomWasNotFound();
    void GetLastMessages_ThrowsArgumentExcpetion_WhenCountLessThanZero();
    void GetLastMessages_ThrowsArgumentExcpetion_WhenSkipLessThanZero();
    void GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired();

    void GetByIdAsync_ReturnsMessage_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFound();

    void AddAsync_AddsNewMessage();
    void AddAsync_ReturnsAddedMessage();
    void AddAsync_AppliesAttachmentsToMessage();
    void AddAsync_AppliesReactionsToMessage();
    void AddAsync_ThrowsNotFoundException_WhenAuthorWasNotFound();
    void AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound();
    void AddAsync_ThrowsNotFoundException_WhenRepliedMessageWasNotFound();
    void AddAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    void Delete_DeletesMessage();
    void Delete_DeletesAllRelatedReactions();
    void Delete_DeletesAllRelatedAttachments();
    void Delete_NotDeletesAnyOtherMessages();
    void Delete_ThrowsNotFoundException_WhenMessageDoesNotExist();
    void Delete_ThrowsRoomExpiredException_WhenRoomExpired();

    void DeleteByIdAsync_DeletesMessage();
    void DeleteByIdAsync_DeletesAllRelatedReactions();
    void DeleteByIdAsync_DeletesAllRelatedAttachments();
    void DeleteByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFoundById();
    void DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    void Update_UpdatesMessage();
    void Update_ThrowsNotFoundException_WhenMessageDoesNotExist();
    void Update_ThrowsRoomExpiredException_WhenRoomExpired();

    void SaveChanges_SavesChanges();
}