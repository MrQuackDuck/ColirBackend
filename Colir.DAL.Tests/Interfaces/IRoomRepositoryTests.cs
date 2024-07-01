namespace Colir.DAL.Tests.Interfaces;

public interface IRoomRepositoryTests
{
    void GetAllAsync_ReturnsAllRooms();

    void GetByIdAsync_ReturnsRoom_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound();

    void AddAsync_AddsNewRoom();
    void AddAsync_ReturnsAddedRoom();
    void AddAsync_AppliesJoinedUsersToRoom();
    void AddAsync_ThrowsArgumentException_WhenWrongExpiryDateWasProvided();
    void AddAsync_ThrowsArgumentException_WhenOwnerWasNotFound();

    void Delete_DeletesRoom();
    void Delete_DeletesAllRelatedAttachments();
    void Delete_DeletesAllRelatedMessages();
    void Delete_DeletesAllRelatedReactions();
    void Delete_ThrowsNotFoundException_WhenRoomDoesNotExist();

    void DeleteByIdAsync_DeletesRoom();
    void DeleteByIdAsync_DeletesAllRelatedAttachments();
    void DeleteByIdAsync_DeletesAllRelatedMessages();
    void DeleteByIdAsync_DeletesAllRelatedReactions();
    void DeleteByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFoundById();

    void Update_UpdatesRoom();
    void Update_ThrowsNotFoundException_WhenRoomDoesNotExist();

    void SaveChanges_SavesChanges();

    void DeleteAllExpiredAsync_DeletesAllExpiredRooms();
    void DeleteAllExpiredAsync_ThrowsNotFoundException_WhenNoExpiredRoomsExist();
}