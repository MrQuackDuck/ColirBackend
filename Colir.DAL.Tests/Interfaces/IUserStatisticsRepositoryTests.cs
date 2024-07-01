namespace Colir.DAL.Tests.Interfaces;

public interface IUserStatisticsRepositoryTests
{
    void GetAllAsync_ReturnsAllUsersStatistics();

    void GetByUserHexIdAsync_ReturnsUserStatistics();
    void GetByUserHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound();
    void GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();

    void GetByIdAsync_ReturnsUserStatistics_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFound();

    void AddAsync_AddsNewUserStatistics();
    void AddAsync_ReturnsAddedUserStatistics();
    void AddAsync_ThrowsArgumentException_WhenUserStatisticsAlreadyExist();
    void AddAsync_ThrowsNotFoundException_WhenUserWasNotFound();

    void Delete_DeletesUserStatistics();
    void Delete_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist();

    void DeleteByIdAsync_DeletesUserStatistics();
    void Delete_ThrowsNotFoundException_WhenUserStatisticsWasNotFoundById();

    void Update_UpdatesUserStatistics();
    void Update_ThrowsArgumentException_WhenProvidedAnotherUserId();
    void Update_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist();

    void SaveChanges_SavesChanges();
}