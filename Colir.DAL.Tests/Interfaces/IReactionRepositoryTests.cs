namespace Colir.DAL.Tests.Interfaces;

public interface IReactionRepositoryTests
{
    void GetAllAsync_ReturnsAllReactions();

    void GetByIdAsync_ReturnsRoom_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound();

    void GetReactionsOnMessage_ReturnsAllReactionsOnMessage();
    void GetReactionsOnMessage_ThrowsNotFoundException_WhenMessageWasNotFound();

    void AddAsync_AddsNewRoom();
    void AddAsync_ReturnsAddedRoom();
    void AddAsync_AppliesJoinedUsersToRoom();
    void AddAsync_ThrowsArgumentException_WhenAuthorWasNotFound();
    void AddAsync_ThrowsArgumentException_WhenMessageWasNotFound();

    void Delete_DeletesRoom();
    void Delete_ThrowsNotFoundException_WhenRoomDoesNotExist();

    void DeleteByIdAsync_DeletesRoom();
    void Delete_ThrowsNotFoundException_WhenRoomWasNotFoundById();

    void Update_UpdatesRoom();
    void Update_ThrowsNotFoundException_WhenRoomDoesNotExist();

    void SaveChanges_SavesChanges();
}