namespace Colir.DAL.Tests.Interfaces;

public interface IMessageRepositoryTests
{
    Task GetAllAsync_ReturnsAllMessages();

    Task GetLastMessages_ReturnsLastMessages();
    Task GetLastMessages_ReturnsLastMessagesWithAttachments();
    Task GetLastMessages_ReturnsLastMessagesWithReactions();
    Task GetLastMessages_ThrowsNotFoundException_WhenRoomWasNotFound();
    Task GetLastMessages_ThrowsArgumentExcpetion_WhenCountLessThanZero();
    Task GetLastMessages_ThrowsArgumentExcpetion_WhenSkipLessThanZero();
    Task GetLastMessages_ThrowsRoomExpiredException_WhenRoomExpired();

    Task GetByIdAsync_ReturnsMessage_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFound();

    Task AddAsync_AddsNewMessage();
    Task AddAsync_AppliesAttachmentsToMessage();
    Task AddAsync_AppliesReactionsToMessage();
    Task AddAsync_ThrowsNotFoundException_WhenAuthorWasNotFound();
    Task AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound();
    Task AddAsync_ThrowsNotFoundException_WhenRepliedMessageWasNotFound();
    Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    Task Delete_DeletesMessage();
    Task Delete_DeletesAllRelatedReactions();
    Task Delete_DeletesAllRelatedAttachments();
    Task Delete_NotDeletesAnyOtherMessages();
    Task Delete_ThrowsNotFoundException_WhenMessageDoesNotExist();
    Task Delete_ThrowsRoomExpiredException_WhenRoomExpired();

    Task DeleteByIdAsync_DeletesMessage();
    Task DeleteByIdAsync_DeletesAllRelatedReactions();
    Task DeleteByIdAsync_DeletesAllRelatedAttachments();
    Task DeleteByIdAsync_ThrowsNotFoundException_WhenMessageWasNotFoundById();
    Task DeleteByIdAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    Task Update_UpdatesMessage();
    Task Update_ThrowsNotFoundException_WhenMessageDoesNotExist();
    Task Update_ThrowsRoomExpiredException_WhenRoomExpired();
}