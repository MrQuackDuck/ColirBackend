namespace Colir.DAL.Tests.Interfaces;

public interface ILastTimeUserReadChatRepositoryTests
{
    void GetAllAsync_ReturnsAllTimesUsersReadChats();

    void GetAsync_ReturnsEntity();
    void GetAsync_ThrowsNotFoundException_WhenEntityWasNotFound();
    void GetAsync_ThrowsNotFoundException_WhenUserWasNotFound();
    void GetAsync_ThrowsNotFoundException_WhenRoomWasNotFound();

    void GetByIdAsync_ReturnsEntity_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenEntityWasNotFound();

    void AddAsync_AddsNewEntity();
    void AddAsync_ReturnsAddedEntity();
    void AddAsync_ThrowsInvalidOperationException_WhenEntryWithSameUserIdAndRoomIdAlreadyExists();
    void AddAsync_ThrowsNotFoundException_WhenUserWasNotFound();
    void AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound();
    void AddAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    void Delete_DeletesEntity();
    void Delete_ThrowsNotFoundException_WhenEntityDoesNotExist();

    void DeleteByIdAsync_DeletesEntity();
    void Delete_ThrowsNotFoundException_WhenEntityWasNotFoundById();

    void Update_UpdatesEntity();
    void Update_ThrowsArgumentException_WhenProvidedAnotherUserId();
    void Update_ThrowsArgumentException_WhenProvidedAnotherRoomId();
    void Update_ThrowsNotFoundException_WhenEntityDoesNotExist();
    void Update_ThrowsRoomExpiredException_WhenRoomExpired();

    void SaveChanges_SavesChanges();
}