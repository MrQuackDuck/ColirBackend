using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class UserSettingsRepositoryTests : IUserSettingsRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllUsersSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByUserHexIdAsync_ReturnsUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByUserHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ReturnsUserSettings_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenUserSettingsWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenUserSettingsAlreadyExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenUserSettingsDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenUserSettingsWereNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenUserSettingsDoNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}