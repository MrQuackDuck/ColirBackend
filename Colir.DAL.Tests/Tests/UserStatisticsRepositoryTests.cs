using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class UserStatisticsRepositoryTests : IUserStatisticsRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllUsersStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByUserHexIdAsync_ReturnsUserStatistics()
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
    public void GetByIdAsync_ReturnsUserStatistics_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenUserStatisticsAlreadyExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenUserStatisticsWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesUserStatistics()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}