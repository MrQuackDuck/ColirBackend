namespace Colir.DAL.Tests.Interfaces;

public interface IUserRepositoryTests
{
    void GetAllAsync_ReturnsAllUsers();

    void GetByIdAsync_ReturnsUser_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenUserWasNotFound();

    void GetByHexIdAsync_ReturnsUser_WhenFound();
    void GetByHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound();
    void GetByHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();

    void AddAsync_AddsNewUser();
    void AddAsync_ReturnsAddedUser();
    void AddAsync_AppliesJoinedRoomsToUser();
    void AddAsync_ThrowsArgumentException_WhenHexAlreadyExists();
    void AddAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();
    void AddAsync_ThrowsArgumentException_WhenUsernameTooShort();
    void AddAsync_ThrowsArgumentException_WhenUsernameTooLong();
    void AddAsync_ThrowsNotFound_WhenOneOfJoinedRoomsWasNotFound();
    void AddAsync_ThrowsRoomExpiredException_WhenOneOfJoinedRoomsIsExpired();

    void Delete_DeletesUser();
    void Delete_ThrowsNotFoundException_WhenUserDoesNotExist();

    void DeleteByIdAsync_DeletesUser();
    void Delete_ThrowsNotFoundException_WhenUserWasNotFoundById();

    void Update_UpdatesUser();
    void Update_ThrowsNotFoundException_WhenUserDoesNotExist();

    void SaveChanges_SavesChanges();
}