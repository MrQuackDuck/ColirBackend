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
public class UserSettingsRepositoryTests : IUserSettingsRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private UserSettingsRepository _userSettingsRepository = default!;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize user settings repository
        _userSettingsRepository = new UserSettingsRepository(_dbContext);

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
    public async Task GetAllAsync_ReturnsAllUsersSettings()
    {
        // Arrange
        List<UserSettings> expected = await _dbContext.UserSettings
            .Include(nameof(UserSettings.User))
            .ToListAsync();

        // Act
        var result = await _userSettingsRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new UserSettingsEqualityComparer()));

        Assert.That(result.Select(r => r.User).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.User).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByUserHexIdAsync_ReturnsUserSettings()
    {
        // Arrange
        UserSettings expected = await _dbContext.UserSettings
            .Include(nameof(UserSettings.User))
            .FirstAsync(us => us.Id == 1);

        // Act
        var result = await _userSettingsRepository.GetByUserHexIdAsync(0xFFFFFF);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserSettingsEqualityComparer()));
        Assert.That(result.User, Is.EqualTo(expected.User).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByUserHexIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userSettingsRepository.GetByUserHexIdAsync(0x444444);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        // Act
        AsyncTestDelegate act = async () => await _userSettingsRepository.GetByUserHexIdAsync(0xFFFFFFF);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsUserSettings_WhenFound()
    {
        // Arrange
        UserSettings expected = await _dbContext.UserSettings
            .Include(nameof(UserSettings.User))
            .FirstAsync(us => us.Id == 1);

        // Act
        var result = await _userSettingsRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserSettingsEqualityComparer()));
        Assert.That(result.User, Is.EqualTo(expected.User).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserSettingsWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userSettingsRepository.GetByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewUserSettings()
    {
        // Arrange
        await _userSettingsRepository.DeleteByIdAsync(3);
        await _userSettingsRepository.SaveChangesAsync();

        var userSettingsToAdd = new UserSettings
        {
            UserId = 3,
            StatisticsEnabled = true
        };

        // Act
        await _userSettingsRepository.AddAsync(userSettingsToAdd);
        await _userSettingsRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserSettings.CountAsync() == 3);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUserSettingsAlreadyExist()
    {
        // Arrange
        var userSettingsToAdd = new UserSettings
        {
            Id = 3,
            UserId = 1,
            StatisticsEnabled = true
        };

        // Act
        AsyncTestDelegate act = async () => await _userSettingsRepository.AddAsync(userSettingsToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Arrange
        var userSettingsToAdd = new UserSettings
        {
            Id = 3,
            UserId = 404,
            StatisticsEnabled = true
        };

        // Act
        AsyncTestDelegate act = async () => await _userSettingsRepository.AddAsync(userSettingsToAdd);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task Delete_DeletesUserSettings()
    {
        // Arrange
        var userSettingsToDelete = await _dbContext.UserSettings.AsNoTracking().FirstAsync();

        // Act
        _userSettingsRepository.Delete(userSettingsToDelete);
        await _userSettingsRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserSettings.CountAsync() == 2);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserSettingsDoesNotExist()
    {
        // Arrange
        var userSettingsToDelete = new UserSettings
        {
            Id = 404,
            UserId = 1, // "First User"
            StatisticsEnabled = true,
        };

        // Act
        TestDelegate act = () => _userSettingsRepository.Delete(userSettingsToDelete);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUserSettings()
    {
        // Act
        await _userSettingsRepository.DeleteByIdAsync(1);
        await _userSettingsRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserSettings.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenUserSettingsWereNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _userSettingsRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesUserSettings()
    {
        // Arrange
        var userSettingsToUpdate = await _dbContext.UserSettings.AsNoTracking().FirstAsync();
        userSettingsToUpdate.StatisticsEnabled = false;

        // Act
        _userSettingsRepository.Update(userSettingsToUpdate);
        await _userSettingsRepository.SaveChangesAsync();

        // Assert
        Assert.That(!(await _dbContext.UserSettings.FirstAsync()).StatisticsEnabled);
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        // Arrange
        var userSettingsToUpdate = await _dbContext.UserSettings.AsNoTracking().FirstAsync();
        userSettingsToUpdate.UserId = 3;

        // Act
        TestDelegate act = () => _userSettingsRepository.Update(userSettingsToUpdate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenUserSettingsDoNotExist()
    {
        // Arrange
        var userSettingsToUpdate = new UserSettings
        {
            Id = 404,
            UserId = 1, // "First User"
            StatisticsEnabled = true,
        };

        // Act
        TestDelegate act = () => _userSettingsRepository.Update(userSettingsToUpdate);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }
}