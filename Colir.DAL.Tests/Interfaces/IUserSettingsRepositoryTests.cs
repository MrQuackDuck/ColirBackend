namespace Colir.DAL.Tests.Interfaces;

public interface IUserSettingsRepositoryTests
{
    void GetAllAsync_ReturnsAllUsersSettings();

    void GetByUserHexIdAsync_ReturnsUserSettings();
    void GetByUserHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound();
    void GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();

    void GetByIdAsync_ReturnsUserSettings_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenUserSettingsWasNotFound();

    void AddAsync_AddsNewUserSettings();
    void AddAsync_ReturnsAddedUserSettings();
    void AddAsync_ThrowsArgumentException_WhenUserSettingsAlreadyExist();
    void AddAsync_ThrowsNotFoundException_WhenUserWasNotFound();

    void Delete_DeletesUserSettings();
    void Delete_ThrowsNotFoundException_WhenUserSettingsDoesNotExist();

    void DeleteByIdAsync_DeletesUserSettings();
    void Delete_ThrowsNotFoundException_WhenUserSettingsWereNotFoundById();

    void Update_UpdatesUserSettings();
    void Update_ThrowsArgumentException_WhenProvidedAnotherUserId();
    void Update_ThrowsNotFoundException_WhenUserSettingsDoNotExist();

    void SaveChanges_SavesChanges();
}