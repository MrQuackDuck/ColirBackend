namespace Colir.DAL.Tests.Interfaces;

public interface IRoomRepositoryTests
{
    Task GetAllAsync_ReturnsAllRooms();

    Task GetByIdAsync_ReturnsRoom_WhenFound(long id);
    Task GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound(long id);

    Task AddAsync_AddsNewRoom();
    Task AddAsync_AppliesJoinedUsersToRoom();
    Task AddAsync_ThrowsRoomExpiredException_WhenWrongExpiryDateWasProvided();
    Task AddAsync_ThrowsUserNotFoundException_WhenOwnerWasNotFound();

    Task Delete_DeletesRoom();
    Task Delete_DeletesAllRelatedAttachments();
    Task Delete_DeletesAllRelatedMessages();
    Task Delete_DeletesAllRelatedReactions();
    Task Delete_ThrowsNotFoundException_WhenRoomDoesNotExist();

    Task DeleteByIdAsync_DeletesRoom();
    Task DeleteByIdAsync_DeletesAllRelatedAttachments();
    Task DeleteByIdAsync_DeletesAllRelatedMessages();
    Task DeleteByIdAsync_DeletesAllRelatedReactions();
    Task DeleteByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFoundById();

    Task Update_UpdatesRoom();
    Task Update_ThrowsNotFoundException_WhenRoomDoesNotExist();

    Task SaveChanges_SavesChanges();

    Task DeleteAllExpiredAsync_DeletesAllExpiredRooms();
    Task DeleteAllExpiredAsync_ThrowsNotFoundException_WhenNoExpiredRoomsExist();
}