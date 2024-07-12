using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class LastTimeUserReadChatRepositoryTests : ILastTimeUserReadChatRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private LastTimeUserReadChatRepository _lastTimeUserReadChatRepository;
    
    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();
        
        // Initialize repository
        _lastTimeUserReadChatRepository = new LastTimeUserReadChatRepository(_dbContext);
        
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
    public async Task GetAllAsync_ReturnsAllTimesUsersReadChats()
    {
        // Arrange
        List<LastTimeUserReadChat> expected = _dbContext.LastTimeUserReadChats
                                                        .Include(nameof(LastTimeUserReadChat.Room))
                                                        .Include(nameof(LastTimeUserReadChat.User))        
                                                        .ToList();
        
        // Act
        var result = await _lastTimeUserReadChatRepository.GetAllAsync();
        
        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new LastTimeUserReadChatEqualityComparer()));
        
        Assert.That(result.Select(r => r.Room).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Room).OrderBy(r => r.Id)).Using(new RoomEqualityComparer()));
        
        Assert.That(result.Select(r => r.User).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.User).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetAsync_ReturnsEntity()
    {
        // Arrange
        var expected = _dbContext.LastTimeUserReadChats
                                 .Include(nameof(LastTimeUserReadChat.Room))
                                 .Include(nameof(LastTimeUserReadChat.User))
                                 .First(l => l.UserId == 1 && l.RoomId == 1);
        
        // Act
        var result = await _lastTimeUserReadChatRepository.GetAsync(1, 1);
        
        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new LastTimeUserReadChatEqualityComparer()));
        Assert.That(result.Room, Is.EqualTo(expected.Room).Using(new RoomEqualityComparer()));
        Assert.That(result.User, Is.EqualTo(expected.User).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetAsync_ThrowsNotFoundException_WhenEntityWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.GetAsync(404, 404);
        
        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task GetAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.GetAsync(404, 1);
        
        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.GetAsync(1, 404);
        
        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsEntity_WhenFound()
    {
        // Arrange
        var expected = _dbContext.LastTimeUserReadChats
                                 .Include(nameof(LastTimeUserReadChat.Room))
                                 .Include(nameof(LastTimeUserReadChat.User))
                                 .First(l => l.Id == 1);
        
        // Act
        var result = await _lastTimeUserReadChatRepository.GetByIdAsync(1);
        
        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new LastTimeUserReadChatEqualityComparer()));
        Assert.That(result.Room, Is.EqualTo(expected.Room).Using(new RoomEqualityComparer()));
        Assert.That(result.User, Is.EqualTo(expected.User).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenEntityWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.GetByIdAsync(404);
        
        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewEntity()
    {
        // Arrange
        var lastTimeUserReadChatToAdd = new LastTimeUserReadChat
        {
            RoomId = 1, // "Room #1"
            UserId = 2, // "Second User"
            Timestamp = DateTime.Now
        };

        // Act
        await _lastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChatToAdd);

        // Assert
        Assert.That(_dbContext.LastTimeUserReadChats.Count() == 2);
    }

    [Test]
    public async Task AddAsync_ThrowsInvalidActionException_WhenEntryWithSameUserIdAndRoomIdAlreadyExists()
    {
        // Arrange
        var lastTimeUserReadChatToAdd = new LastTimeUserReadChat
        {
            RoomId = 1, // "Room #1"
            UserId = 1, // "First User"
            Timestamp = DateTime.Now
        };

        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChatToAdd);

        // Assert
        Assert.ThrowsAsync<InvalidActionException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsUserNotFoundException_WhenUserWasNotFound()
    {
        // Arrange
        var lastTimeUserReadChatToAdd = new LastTimeUserReadChat
        {
            RoomId = 1, // "Room #1"
            UserId = 404,
            Timestamp = DateTime.Now
        };

        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChatToAdd);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var lastTimeUserReadChatToAdd = new LastTimeUserReadChat
        {
            RoomId = 404,
            UserId = 1, // "First User"
            Timestamp = DateTime.Now
        };

        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChatToAdd);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var lastTimeUserReadChatToAdd = new LastTimeUserReadChat
        {
            RoomId = 2, // "Room #2"
            UserId = 1, // "First User"
            Timestamp = DateTime.Now
        };

        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChatToAdd);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task Delete_DeletesEntity()
    {
        // Arrange
        var entityToDelete = _dbContext.LastTimeUserReadChats.First();

        // Act
        _lastTimeUserReadChatRepository.Delete(entityToDelete);

        // Assert
        Assert.That(_dbContext.LastTimeUserReadChats.Count() == 0);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var entityToDelete = new LastTimeUserReadChat
        {
            Id = 404,
            RoomId = 1, // "Room #1"
            UserId = 1, // "First User"
            Timestamp = DateTime.Now
        };
        
        // Act
        TestDelegate act = () => _lastTimeUserReadChatRepository.Delete(entityToDelete);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesEntity()
    {
        // Act
        await _lastTimeUserReadChatRepository.DeleteByIdAsync(1);

        // Assert
        Assert.That(_dbContext.LastTimeUserReadChats.Count() == 0);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenEntityWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _lastTimeUserReadChatRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesEntity()
    {
        // Arrange
        var entityToUpdate = _dbContext.LastTimeUserReadChats.First();
        var newTimeStamp = DateTime.Now.Add(new TimeSpan(200));
        entityToUpdate.Timestamp = newTimeStamp;

        // Act
        _lastTimeUserReadChatRepository.Update(entityToUpdate);

        // Assert
        Assert.That(_dbContext.LastTimeUserReadChats.First().Timestamp == newTimeStamp);
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        // Arrange
        var entityToUpdate = _dbContext.LastTimeUserReadChats.First();
        entityToUpdate.UserId = 500;

        // Act
        TestDelegate act = () => _lastTimeUserReadChatRepository.Update(entityToUpdate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherRoomId()
    {
        // Arrange
        var entityToUpdate = _dbContext.LastTimeUserReadChats.First();
        entityToUpdate.RoomId = 500;

        // Act
        TestDelegate act = () => _lastTimeUserReadChatRepository.Update(entityToUpdate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var entityToUpdate = new LastTimeUserReadChat()
        {
            Id = 404,
            UserId = 1,
            RoomId = 1,
            Timestamp = DateTime.Now
        };

        // Act
        TestDelegate act = () => _lastTimeUserReadChatRepository.Update(entityToUpdate);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var entityToTest = new LastTimeUserReadChat()
        {
            UserId = 1,
            RoomId = 2,
            Timestamp = DateTime.Now
        };

        _dbContext.LastTimeUserReadChats.Add(entityToTest);

        // Act
        TestDelegate act = () => _lastTimeUserReadChatRepository.Update(entityToTest);

        // Assert
        Assert.Throws<RoomExpiredException>(act);
    }
}