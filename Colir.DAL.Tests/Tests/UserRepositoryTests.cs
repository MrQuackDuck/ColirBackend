using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.DAL.Tests.Tests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class UserRepositoryTests : IUserRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private UserRepository _userRepository = default!;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize user repository with mocked config
        var mock = new Mock<IConfiguration>();
        mock.Setup(config => config["AppSettings:MinUsernameLength"]).Returns("2");
        mock.Setup(config => config["AppSettings:MaxUsernameLength"]).Returns("50");

        _userRepository = new UserRepository(_dbContext, mock.Object);

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
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        List<User> expected = await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .ToListAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));

        Assert.That(result.Select(r => r.UserStatistics).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.UserStatistics).OrderBy(r => r.Id))
                .Using(new UserStatisticsEqualityComparer()));

        Assert.That(result.Select(r => r.UserSettings).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.UserSettings).OrderBy(r => r.Id))
                .Using(new UserSettingsEqualityComparer()));

        Assert.That(result.SelectMany(r => r.JoinedRooms).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.JoinedRooms).OrderBy(r => r.Id)).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsUser_WhenFound()
    {
        // Arrange
        User expected = await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .FirstAsync(u => u.Id == 1);

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));
        Assert.That(result.UserStatistics,
            Is.EqualTo(expected.UserStatistics).Using(new UserStatisticsEqualityComparer()));

        Assert.That(result.UserSettings, Is.EqualTo(expected.UserSettings).Using(new UserSettingsEqualityComparer()));
        Assert.That(result.JoinedRooms, Is.EqualTo(expected.JoinedRooms).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByIdAsync(100);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetByHexIdAsync_ReturnsUser_WhenFound()
    {
        // Arrange
        User expected = await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .FirstAsync(u => u.Id == 1);

        // Act
        var result = await _userRepository.GetByHexIdAsync(0xFFFFFF);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));
        Assert.That(result.UserStatistics,
            Is.EqualTo(expected.UserStatistics).Using(new UserStatisticsEqualityComparer()));

        Assert.That(result.UserSettings, Is.EqualTo(expected.UserSettings).Using(new UserSettingsEqualityComparer()));
        Assert.That(result.JoinedRooms, Is.EqualTo(expected.JoinedRooms).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task GetByHexIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByHexIdAsync(0x404000);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetByHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByHexIdAsync(0xFFFFFFF);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetByGitHubIdAsync_ReturnsUser_WhenFound()
    {
        // Arrange
        User expected = await _dbContext.Users
            .Include(nameof(User.UserStatistics))
            .Include(nameof(User.UserSettings))
            .Include(nameof(User.JoinedRooms))
            .FirstAsync(u => u.Id == 1);

        // Act
        var result = await _userRepository.GetByGithudIdAsync("2024");

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));
        Assert.That(result.UserStatistics,
            Is.EqualTo(expected.UserStatistics).Using(new UserStatisticsEqualityComparer()));

        Assert.That(result.UserSettings, Is.EqualTo(expected.UserSettings).Using(new UserSettingsEqualityComparer()));
        Assert.That(result.JoinedRooms, Is.EqualTo(expected.JoinedRooms).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task GetByGitHubIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByGithudIdAsync("404");

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task Exists_ReturnsTrue_WhenExists()
    {
        // Act
        var result = await _userRepository.ExistsAsync(0xFFFFFF);

        // Assert
        Assert.That(result);
    }

    [Test]
    public async Task Exists_ReturnsFalse_WhenDoesNotExist()
    {
        // Act
        var result = await _userRepository.ExistsAsync(0x404040);

        // Assert
        Assert.That(!result);
    }

    [Test]
    public async Task AddAsync_AddsNewUser()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0x123456,
            JoinedRooms = new List<Room>()
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Users.CountAsync() == 4);
    }

    [Test]
    public async Task AddAsync_CreatesUserSettings()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0x123456,
            JoinedRooms = new List<Room>()
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserSettings.CountAsync() == 4);
    }

    [Test]
    public async Task AddAsync_CreatesUserStatistics()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0x123456,
            JoinedRooms = new List<Room>()
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserStatistics.CountAsync() == 4);
    }

    [Test]
    public async Task AddAsync_AppliesJoinedRoomsToUser()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0x123456,
            JoinedRooms = new List<Room>
            {
                await _dbContext.Rooms.FirstAsync()
            }
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        await _userRepository.SaveChangesAsync();

        // Assert
        var addedUser = await _dbContext.Users.Include(u => u.JoinedRooms).FirstAsync(u => u.Id == userToAdd.Id);
        Assert.That(addedUser.JoinedRooms.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenHexAlreadyExists()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0xFFFFFF,
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0xFFFFFFF,
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsStringTooShortException_WhenUsernameTooShort()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "N",
            HexId = 0x123456,
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<StringTooShortException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsStringTooLongException_WhenUsernameTooLong()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = new string('a', 51),
            HexId = 0x123456,
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomNotFoundException_WhenOneOfJoinedRoomsWasNotFound()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0x123456,
            JoinedRooms = new List<Room>
            {
                new Room { Id = 404 }
            }
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenOneOfJoinedRoomsIsExpired()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 4,
            Username = "NewUser",
            HexId = 0x123456,
            JoinedRooms = new List<Room>
            {
                new Room { ExpiryDate = DateTime.UtcNow.AddDays(-1) }
            }
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task Delete_DeletesUser()
    {
        // Arrange
        var userToDelete = await _dbContext.Users.AsNoTracking().FirstAsync();

        // Act
        _userRepository.Delete(userToDelete);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Users.CountAsync() == 2);
    }

    [Test]
    public async Task Delete_DeletesUserSettings()
    {
        // Arrange
        var userToDelete = await _dbContext.Users.FirstAsync();

        // Act
        _userRepository.Delete(userToDelete);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserSettings.CountAsync() == 2);
    }

    [Test]
    public async Task Delete_DeletesUserStatistics()
    {
        // Arrange
        var userToDelete = await _dbContext.Users.AsNoTracking().FirstAsync();

        // Act
        _userRepository.Delete(userToDelete);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserStatistics.CountAsync() == 2);
    }

    [Test]
    public async Task Delete_ThrowsUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userToDelete = new User() { Id = 404 };

        // Act
        TestDelegate act = () => _userRepository.Delete(userToDelete);

        // Assert
        Assert.Throws<UserNotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUser()
    {
        // Arrange
        var userToDelete = await _dbContext.Users.FirstAsync();

        // Act
        await _userRepository.DeleteByIdAsync(userToDelete.Id);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Users.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUserSettings()
    {
        // Act
        await _userRepository.DeleteByIdAsync(1);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserSettings.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUserStatistics()
    {
        // Act
        await _userRepository.DeleteByIdAsync(1);
        await _userRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.UserStatistics.CountAsync() == 2);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesUser()
    {
        // Arrange
        var userToUpdate = await _dbContext.Users.AsNoTracking().FirstAsync();
        userToUpdate.Username = "UpdatedUser";

        // Act
        _userRepository.Update(userToUpdate);
        await _userRepository.SaveChangesAsync();

        // Assert
        var updatedUser = await _dbContext.Users.FirstAsync();
        Assert.That(updatedUser.Username, Is.EqualTo("UpdatedUser"));
    }

    [Test]
    public async Task Update_ThrowsStringTooLongException_WhenNameTooLong()
    {
        // Arrange
        var userToUpdate = await _dbContext.Users.FirstAsync(u => u.Id == 1);
        userToUpdate.Username = new string('a', 51);

        // Act
        TestDelegate act = () => _userRepository.Update(userToUpdate);

        // Assert
        Assert.Throws<StringTooLongException>(act);
    }

    [Test]
    public async Task Update_ThrowsStringTooShortException_WhenNameTooShort()
    {
        // Arrange
        var userToUpdate = await _dbContext.Users.FirstAsync(u => u.Id == 1);
        userToUpdate.Username = new string('a', 1);

        // Act
        TestDelegate act = () => _userRepository.Update(userToUpdate);

        // Assert
        Assert.Throws<StringTooShortException>(act);
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenExistingHexIdProvided()
    {
        // Arrange
        var userToUpdate = await _dbContext.Users.AsNoTracking().FirstAsync(u => u.Id == 2);
        userToUpdate.HexId = 0xFFFFFF;

        // Act
        TestDelegate act = () => _userRepository.Update(userToUpdate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Test]
    public async Task Update_ThrowsUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userToUpdate = new User() { Id = 404, Username = "UpdatedUser" };

        // Act
        TestDelegate act = () => _userRepository.Update(userToUpdate);

        // Assert
        Assert.Throws<UserNotFoundException>(act);
    }
}