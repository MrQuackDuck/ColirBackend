namespace Colir.DAL.Tests.Interfaces;

public interface IReactionRepositoryTests
{
    Task GetAllAsync_ReturnsAllReactions();

    Task GetByIdAsync_ReturnsRoom_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound();

    Task GetReactionsOnMessage_ReturnsAllReactionsOnMessage();
    Task GetReactionsOnMessage_ThrowsNotFoundException_WhenMessageWasNotFound();

    Task AddAsync_AddsNewRoom();
    Task AddAsync_ReturnsAddedRoom();
    Task AddAsync_AppliesJoinedUsersToRoom();
    Task AddAsync_ThrowsArgumentException_WhenAuthorWasNotFound();
    Task AddAsync_ThrowsArgumentException_WhenMessageWasNotFound();

    Task Delete_DeletesRoom();
    Task Delete_ThrowsNotFoundException_WhenRoomDoesNotExist();

    Task DeleteByIdAsync_DeletesRoom();
    Task Delete_ThrowsNotFoundException_WhenRoomWasNotFoundById();

    Task Update_UpdatesRoom();
    Task Update_ThrowsNotFoundException_WhenRoomDoesNotExist();

    Task SaveChanges_SavesChanges();
}