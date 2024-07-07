﻿using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

public class UserRepositoryTests : IUserRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private UserRepository _userRepository = default!;

    [SetUp]
    public void SetUp()
    {
        _dbContext = UnitTestHelper.CreateDbContext();
        _userRepository = new UserRepository(_dbContext);
        UnitTestHelper.SeedData(_dbContext);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        List<User> expected = _dbContext.Users.ToList();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsUser_WhenFound()
    {
        // Arrange
        User expected = _dbContext.Users.First(u => u.Id == 1);

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByIdAsync(100);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task GetByHexIdAsync_ReturnsUser_WhenFound()
    {
        // Arrange
        var hexId = "#000001";
        User expected = new User
        {
            Id = 2,
            Username = "TestUser",
            HexId = hexId,
            JoinedRooms = new List<Room>()
        };

        _dbContext.Users.Add(expected);
        _dbContext.SaveChanges();

        // Act
        var result = await _userRepository.GetByHexIdAsync(hexId);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByHexIdAsync("#FFFFFF");

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task GetByHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.GetByHexIdAsync("invalid_hex_format");

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewUser()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>()
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Users.Count() == 2);
    }
    
    [Test]
    public async Task AddAsync_CreatesUserSettings()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>()
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.UserSettings.Count() == 1);
    }

    [Test]
    public async Task AddAsync_CreatesUserStatistics()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>()
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.UserStatistics.Count() == 1);
    }

    [Test]
    public async Task AddAsync_AppliesJoinedRoomsToUser()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>
            {
                _dbContext.Rooms.First()
            }
        };

        // Act
        await _userRepository.AddAsync(userToAdd);
        _userRepository.SaveChanges();

        // Assert
        var addedUser = _dbContext.Users.Include(u => u.JoinedRooms).First(u => u.Id == userToAdd.Id);
        Assert.That(addedUser.JoinedRooms.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenHexAlreadyExists()
    {
        // Arrange
        var existingUser = new User()
        {
            Id = 1,
            Username = "ExistingUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>()
        };

        _dbContext.Users.Add(existingUser);
        _dbContext.SaveChanges();

        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
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
            Id = 2,
            Username = "NewUser",
            HexId = "invalid_hex",
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUsernameTooShort()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "N",
            HexId = "#123456",
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenUsernameTooLong()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = new string('a', 51),
            HexId = "#123456",
            JoinedRooms = new List<Room>()
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsNotFound_WhenOneOfJoinedRoomsWasNotFound()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>
            {
                new Room { Id = 404 }
            }
        };

        // Act
        AsyncTestDelegate act = async () => await _userRepository.AddAsync(userToAdd);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenOneOfJoinedRoomsIsExpired()
    {
        // Arrange
        var userToAdd = new User()
        {
            Id = 2,
            Username = "NewUser",
            HexId = "#123456",
            JoinedRooms = new List<Room>
            {
                new Room { Id = 1, ExpiryDate = DateTime.UtcNow.AddDays(-1) }
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
        var userToDelete = _dbContext.Users.First();

        // Act
        _userRepository.Delete(userToDelete);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Users.Count() == 0);
    }

    public async Task Delete_DeletesUserSettings()
    {
        // Arrange
        var userToDelete = _dbContext.Users.First();

        // Act
        _userRepository.Delete(userToDelete);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.UserSettings.Count() == 1);
    }

    public async Task Delete_DeletesUserStatistics()
    {
        // Arrange
        var userToDelete = _dbContext.Users.First();

        // Act
        _userRepository.Delete(userToDelete);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.UserStatistics.Count() == 1);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userToDelete = new User() { Id = 404 };

        // Act
        AsyncTestDelegate act = async () => _userRepository.Delete(userToDelete);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesUser()
    {
        // Arrange
        var userToDelete = _dbContext.Users.First();

        // Act
        await _userRepository.DeleteByIdAsync(userToDelete.Id);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Users.Count() == 0);
    }

    public async Task DeleteByIdAsync_DeletesUserSettings()
    {
        // Act
        await _userRepository.DeleteByIdAsync(1);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.UserSettings.Count() == 1);
    }

    public async Task DeleteByIdAsync_DeletesUserStatistics()
    {
        // Act
        await _userRepository.DeleteByIdAsync(1);
        _userRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.UserStatistics.Count() == 1);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenUserWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _userRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesUser()
    {
        // Arrange
        var userToUpdate = _dbContext.Users.First();
        userToUpdate.Username = "UpdatedUser";

        // Act
        _userRepository.Update(userToUpdate);
        _userRepository.SaveChanges();

        // Assert
        var updatedUser = _dbContext.Users.First();
        Assert.That(updatedUser.Username, Is.EqualTo("UpdatedUser"));
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userToUpdate = new User() { Id = 404, Username = "UpdatedUser" };

        // Act
        AsyncTestDelegate act = async () => _userRepository.Update(userToUpdate);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }
}