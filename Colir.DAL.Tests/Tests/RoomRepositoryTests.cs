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
public class RoomRepositoryTests : IRoomRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private RoomRepository _roomRepository = default!;
    
    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();
        
        // Initialize room repository
        _roomRepository = new RoomRepository(_dbContext);
        
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
    public async Task GetAllAsync_ReturnsAllRooms()
    {
        // Arrange
        List<Room> expected = _dbContext.Rooms
                                        .Include(nameof(Room.Owner))
                                        .Include(nameof(Room.JoinedUsers))
                                        .ToList();

        // Act
        var result = await _roomRepository.GetAllAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new RoomEqualityComparer()));
        
        Assert.That(result.Select(r => r.Owner).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Owner).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));
        
        Assert.That(result.SelectMany(r => r.JoinedUsers).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.JoinedUsers).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));
    }

    [Test]
    [TestCase(1)]
    public async Task GetByIdAsync_ReturnsRoom_WhenFound(long id)
    {
        // Arrange
        Room expected = _dbContext.Rooms
                                  .Include(nameof(Room.Owner))
                                  .Include(nameof(Room.JoinedUsers))
                                  .First(r => r.Id == id);
        
        // Act
        var result = await _roomRepository.GetByIdAsync(id);
        
        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new RoomEqualityComparer()));
        Assert.That(result.Owner, Is.EqualTo(expected.Owner).Using(new UserEqualityComparer()));
        Assert.That(result.JoinedUsers, Is.EqualTo(expected.JoinedUsers).Using(new UserEqualityComparer()));
    }

    [Test]
    [TestCase(3)]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound(long id)
    {
        // Act
        AsyncTestDelegate act = async () => await _roomRepository.GetByIdAsync(id);
        
        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewRoom()
    {
        // Arrange
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #3",
            OwnerId = 1,
            ExpiryDate = DateTime.Today.Add(new TimeSpan(1, 0, 0)),
        };

        // Act
        await _roomRepository.AddAsync(roomToAdd);
        _roomRepository.SaveChanges();
        
        // Assert
        Assert.That(_dbContext.Rooms.Count() == 3);
    }

    [Test]
    public async Task AddAsync_AppliesJoinedUsersToRoom()
    {
        // Arrange
        var users = _dbContext.Users.ToList();
        
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #3",
            OwnerId = 1,
            ExpiryDate = DateTime.Today.Add(new TimeSpan(1, 0, 0)),
            JoinedUsers = users,
        };

        // Act
        await _roomRepository.AddAsync(roomToAdd);
        _roomRepository.SaveChanges();

        // Assert
        var result = _dbContext.Rooms.First(r => r.Guid == roomToAdd.Guid);
        Assert.NotNull(result.JoinedUsers);
        Assert.That(result.JoinedUsers, Is.EqualTo(roomToAdd.JoinedUsers).Using(new UserEqualityComparer()));
    }

    public async Task AddAsync_ThrowsRoomExpiredException_WhenWrongExpiryDateWasProvided()
    {
        // Arrange
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #3",
            OwnerId = 1,
            ExpiryDate = new DateTime(1990, 1, 1),
        };

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.AddAsync(roomToAdd);
        _roomRepository.SaveChanges();

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    public async Task AddAsync_ThrowsUserNotFoundException_WhenOwnerWasNotFound()
    {
        // Arrange
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #3",
            OwnerId = 4,
            ExpiryDate = DateTime.Today.Add(new TimeSpan(1, 0, 0)),
        };

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.AddAsync(roomToAdd);
        _roomRepository.SaveChanges();

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task Delete_DeletesRoom()
    {
        // Arrange
        var roomCount = _dbContext.Rooms.Count();
        var roomToDelete = _dbContext.Rooms.First();
        
        // Act
        _roomRepository.Delete(roomToDelete);
        _roomRepository.SaveChanges();

        // Assert
        // Ensure that room is now couldn't be found
        var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == roomToDelete.Id);
        Assert.Null(room);
        
        // Ensure that room count was reduced
        Assert.That(_dbContext.Rooms.Count() == (roomCount - 1));
    }

    [Test]
    public async Task Delete_DeletesAllRelatedAttachments()
    {
        // Arrange
        var roomToDelete = _dbContext.Rooms.First();

        // Act
        _roomRepository.Delete(roomToDelete);
        _roomRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Attachments.Count() == 0);
    }

    [Test]
    public async Task Delete_DeletesAllRelatedMessages()
    {
        // Arrange
        var roomToDelete = _dbContext.Rooms.First();

        // Act
        _roomRepository.Delete(roomToDelete);
        _roomRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 0);
    }

    [Test]
    public async Task Delete_DeletesAllRelatedReactions()
    {
        // Arrange
        var roomToDelete = _dbContext.Rooms.First();

        // Act
        _roomRepository.Delete(roomToDelete);
        _roomRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 0);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomToDelete = new Room()
        {
            Id = 10,
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #10",
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
            OwnerId = 1
        };

        // Act
        TestDelegate act = () => _roomRepository.Delete(roomToDelete);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesRoom()
    {
        // Arrange
        var roomCount = _dbContext.Rooms.Count();
        var roomToDelete = _dbContext.Rooms.First();
        
        // Act
        _roomRepository.Delete(roomToDelete);
        _roomRepository.SaveChanges();

        // Assert
        // Ensure that room is now couldn't be found
        var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == roomToDelete.Id);
        Assert.Null(room);
        
        // Ensure that room count was reduced
        Assert.That(_dbContext.Rooms.Count() == (roomCount - 1));
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        // Arrange
        var roomCount = _dbContext.Rooms.Count();
        var roomIdToDelete = _dbContext.Rooms.First().Id;
        
        // Act
        await _roomRepository.DeleteByIdAsync(roomIdToDelete);
        _roomRepository.SaveChanges();

        // Assert
        // Ensure that room is now couldn't be found
        var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == roomIdToDelete);
        Assert.Null(room);
        
        // Ensure that room count was reduced
        Assert.That(_dbContext.Rooms.Count() == (roomCount - 1));
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedMessages()
    {
        // Arrange
        var roomIdToDelete = _dbContext.Rooms.First().Id;

        // Act
        await _roomRepository.DeleteByIdAsync(roomIdToDelete);
        _roomRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Messages.Count() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        // Arrange
        var roomIdToDelete = _dbContext.Rooms.First().Id;

        // Act
        await _roomRepository.DeleteByIdAsync(roomIdToDelete);
        _roomRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFoundById()
    {
        // Arrange
        var roomIdToDelete = 10;

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.DeleteByIdAsync(roomIdToDelete);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesRoom()
    {
        // Arrange
        var newExpiryDate = DateTime.Now.Add(new TimeSpan(2, 0, 0));
        var newOwnerId = 2;
        
        var room = new Room
        {
            Id = 1,
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #1",
            ExpiryDate = newExpiryDate,
            OwnerId = newOwnerId,
        };

        // Act
        _roomRepository.Update(room);
        _roomRepository.SaveChanges();

        // Assert
        Assert.That(room, Is.EqualTo(new Room()
        {
            Id = 1,
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #1",
            ExpiryDate = newExpiryDate,
            OwnerId = newOwnerId,
        }).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var nonExistingRoom = new Room
        {
            Id = 4,
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #69",
            ExpiryDate = DateTime.Now.Add(new TimeSpan(2, 0, 0)),
            OwnerId = 1,
        };

        // Act
        TestDelegate act = () => _roomRepository.Update(nonExistingRoom);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteAllExpiredAsync_DeletesAllExpiredRooms()
    {
        // Arrange
        var roomCount = _dbContext.Rooms.Count();
        
        // Act
        await _roomRepository.DeleteAllExpiredAsync();

        // Assert
        var newRoomCount = _dbContext.Rooms.Count();
        var expiredRoom = _roomRepository.GetByIdAsync(2);
        Assert.Null(expiredRoom);
        Assert.That(roomCount == newRoomCount);
    }

    [Test]
    public async Task DeleteAllExpiredAsync_ThrowsNotFoundException_WhenNoExpiredRoomsExist()
    {
        // Act
        await _roomRepository.DeleteAllExpiredAsync();
        AsyncTestDelegate act = async () => await _roomRepository.DeleteAllExpiredAsync();
        
        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }
}