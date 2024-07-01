using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class UserRepositoryTests : IUserRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ReturnsUser_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByHexIdAsync_ReturnsUser_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_CreatesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_CreatesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesJoinedRoomsToUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenHexAlreadyExists()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUsernameTooShort()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUsernameTooLong()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFound_WhenOneOfJoinedRoomsWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenOneOfJoinedRoomsIsExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_DeletesUserSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_DeletesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}