using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class UserStatisticsRepositoryTests : IUserStatisticsRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private UserStatisticsRepository _userStatisticsRepository = default!;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize user statistics repository
        _userStatisticsRepository = new UserStatisticsRepository(_dbContext);

        // Add entities
        UnitTestHelper.SeedData(_dbContext);
    }

    [TearDown]
    public void CleanUp()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllUsersStatistics()
    {
        // Arrange
        var expected = await _dbContext.UserStatistics
            .Include(nameof(UserStatistics.User))
            .ToListAsync();

        // Act
        var result = await _userStatisticsRepository.GetAllAsync();

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserStatisticsEqualityComparer()));

        Assert.That(result.Select(r => r.User).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.User).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByUserHexIdAsync_ReturnsUserStatistics()
    {
        // Arrange
        var expected = await _dbContext.UserStatistics
            .Include(nameof(UserStatistics.User))
            .FirstAsync(u => u.UserId == 1);

        // Act
        var result = await _userStatisticsRepository.GetByUserHexIdAsync(0xFFFFFF);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserStatisticsEqualityComparer()));
        Assert.That(result.User, Is.EqualTo(expected.User).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByUserHexIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userStatisticsRepository.GetByUserHexIdAsync(0x404040);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        // Act
        AsyncTestDelegate act = async () => await _userStatisticsRepository.GetByUserHexIdAsync(0x4040404);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsUserStatistics_WhenFound()
    {
        // Arrange
        var expected = await _dbContext.UserStatistics
            .Include(nameof(UserStatistics.User))
            .FirstAsync(us => us.Id == 1);

        // Act
        var result = await _userStatisticsRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserStatisticsEqualityComparer()));
        Assert.That(result.User, Is.EqualTo(expected.User).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userStatisticsRepository.GetByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewUserStatistics()
    {
        // Arrange
        await _userStatisticsRepository.DeleteByIdAsync(3);
        await _userStatisticsRepository.SaveChangesAsync();

        var statisticsToAdd = new UserStatistics
        {
            UserId = 3, // "Third User"
            SecondsSpentInVoice = 0,
            ReactionsSet = 1,
            MessagesSent = 2,
            RoomsJoined = 2,
            RoomsCreated = 2,
        };

        // Act
        await _userStatisticsRepository.AddAsync(statisticsToAdd);
        await _userStatisticsRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserStatistics.CountAsync() == 3);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUserStatisticsAlreadyExist()
    {
        // Arrange
        var statisticsToAdd = new UserStatistics
        {
            UserId = 2, // "Second User"
            SecondsSpentInVoice = 0,
            ReactionsSet = 1,
            MessagesSent = 2,
            RoomsJoined = 2,
            RoomsCreated = 2,
        };

        // Act
        AsyncTestDelegate act = async () => await _userStatisticsRepository.AddAsync(statisticsToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Arrange
        var statisticsToAdd = new UserStatistics
        {
            UserId = 404,
            SecondsSpentInVoice = 0,
            ReactionsSet = 1,
            MessagesSent = 2,
            RoomsJoined = 2,
            RoomsCreated = 2,
        };

        // Act
        AsyncTestDelegate act = async () => await _userStatisticsRepository.AddAsync(statisticsToAdd);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task Delete_DeletesUserStatistics()
    {
        // Arrange
        var statisticsToDelete = await _dbContext.UserStatistics.AsNoTracking().FirstAsync();

        // Act
        _userStatisticsRepository.Delete(statisticsToDelete);
        await _userStatisticsRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserStatistics.CountAsync() == 2);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist()
    {
        // Arrange
        var statisticsToAdd = new UserStatistics
        {
            Id = 404,
            UserId = 1,
            SecondsSpentInVoice = 0,
            ReactionsSet = 1,
            MessagesSent = 2,
            RoomsJoined = 2,
            RoomsCreated = 2,
        };

        // Act
        TestDelegate act = () => _userStatisticsRepository.Delete(statisticsToAdd);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUserStatistics()
    {
        // Act
        await _userStatisticsRepository.DeleteByIdAsync(1);
        await _userStatisticsRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserStatistics.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _userStatisticsRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesUserStatistics()
    {
        // Arrange
        var statsToUpdate = await _dbContext.UserStatistics.AsNoTracking().FirstAsync();
        statsToUpdate.ReactionsSet = 50;

        // Act
        _userStatisticsRepository.Update(statsToUpdate);
        await _userStatisticsRepository.SaveChangesAsync();

        // Assert
        Assert.That((await _dbContext.UserStatistics.FirstAsync()).ReactionsSet == 50);
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        // Arrange
        var statsToUpdate = await _dbContext.UserStatistics.AsNoTracking().FirstAsync();
        statsToUpdate.UserId = 3;

        // Act
        TestDelegate act = () => _userStatisticsRepository.Update(statsToUpdate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist()
    {
        // Arrange
        var statsToUpdate = new UserStatistics
        {
            Id = 404,
            UserId = 1, // "First User",
            MessagesSent = 10,
            RoomsCreated = 5,
        };

        // Act
        TestDelegate act = () => _userStatisticsRepository.Update(statsToUpdate);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }
}