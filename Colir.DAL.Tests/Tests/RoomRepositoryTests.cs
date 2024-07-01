using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

public class RoomRepositoryTests : IRoomRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private RoomRepository _roomRepository = default!;
    
    [SetUp]
    public void SetUp()
    {
        // Create database options (in-memory for unit testing)
        var options = new DbContextOptionsBuilder<ColirDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        // Create database context
        _dbContext = new ColirDbContext(options);
        
        // Initialize room repository
        _roomRepository = new RoomRepository(_dbContext);
        
        // Add entities
        UnitTestHelper.SeedData(_dbContext);
    }
    
    [Test]
    public async Task GetAllAsync_ReturnsAllRooms()
    {
        // Arrange
        List<Room> expected = _dbContext.Rooms.ToList();

        // Act
        var result = await _roomRepository.GetAllAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsRoom_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesJoinedUsersToRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenWrongExpiryDateWasProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenOwnerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAllExpiredAsync_DeletesAllExpiredRooms()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAllExpiredAsync_ThrowsNotFoundException_WhenNoExpiredRoomsExist()
    {
        throw new NotImplementedException();
    }
}