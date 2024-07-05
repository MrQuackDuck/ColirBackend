using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class UserSettingsRepositoryTests : IUserSettingsRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllUsersSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByUserHexIdAsync_ReturnsUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByUserHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ReturnsUserSettings_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserSettingsWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUserSettingsAlreadyExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserSettingsDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserSettingsWereNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenUserSettingsDoNotExist()
    {
        throw new NotImplementedException();
    }
}