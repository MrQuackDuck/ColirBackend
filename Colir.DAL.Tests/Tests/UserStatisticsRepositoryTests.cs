using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class UserStatisticsRepositoryTests : IUserStatisticsRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllUsersStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByUserHexIdAsync_ReturnsUserStatistics()
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
    public async Task GetByIdAsync_ReturnsUserStatistics_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUserStatisticsAlreadyExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserStatisticsWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}