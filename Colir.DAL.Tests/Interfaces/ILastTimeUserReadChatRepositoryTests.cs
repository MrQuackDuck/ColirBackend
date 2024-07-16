namespace Colir.DAL.Tests.Interfaces;

public interface ILastTimeUserReadChatRepositoryTests
{
    Task GetAllAsync_ReturnsAllTimesUsersReadChats();

    Task GetAsync_ReturnsEntity();
    Task GetAsync_ThrowsUserNotFoundException_WhenUserWasNotFound();
    Task GetAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();

    Task GetByIdAsync_ReturnsEntity_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenEntityWasNotFound();

    Task AddAsync_AddsNewEntity();
    Task AddAsync_ThrowsInvalidActionException_WhenEntryWithSameUserIdAndRoomIdAlreadyExists();
    Task AddAsync_ThrowsUserNotFoundException_WhenUserWasNotFound();
    Task AddAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound();
    Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired();

    Task Delete_DeletesEntity();
    Task Delete_ThrowsNotFoundException_WhenEntityDoesNotExist();

    Task DeleteByIdAsync_DeletesEntity();
    Task Delete_ThrowsNotFoundException_WhenEntityWasNotFoundById();

    Task Update_UpdatesEntity();
    Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId();
    Task Update_ThrowsArgumentException_WhenProvidedAnotherRoomId();
    Task Update_ThrowsNotFoundException_WhenEntityDoesNotExist();
    Task Update_ThrowsRoomExpiredException_WhenRoomExpired();
}