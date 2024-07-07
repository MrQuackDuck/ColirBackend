namespace Colir.DAL.Tests.Interfaces;

public interface IUserSettingsRepositoryTests
{
    Task GetAllAsync_ReturnsAllUsersSettings();

    Task GetByUserHexIdAsync_ReturnsUserSettings();
    Task GetByUserHexIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound();
    Task GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();

    Task GetByIdAsync_ReturnsUserSettings_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenUserSettingsWasNotFound();

    Task AddAsync_AddsNewUserSettings();
    Task AddAsync_ThrowsArgumentException_WhenUserSettingsAlreadyExist();
    Task AddAsync_ThrowsUserNotFoundException_WhenUserWasNotFound();

    Task Delete_DeletesUserSettings();
    Task Delete_ThrowsNotFoundException_WhenUserSettingsDoesNotExist();

    Task DeleteByIdAsync_DeletesUserSettings();
    Task Delete_ThrowsNotFoundException_WhenUserSettingsWereNotFoundById();

    Task Update_UpdatesUserSettings();
    Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId();
    Task Update_ThrowsNotFoundException_WhenUserSettingsDoNotExist();
}