using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

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

        // Initialize the room repository
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(config => config["AppSettings:MinRoomNameLength"]).Returns("2");
        configMock.Setup(config => config["AppSettings:MaxRoomNameLength"]).Returns("50");

        var roomFileManagerMock = new Mock<IRoomFileManager>();

        _roomRepository = new RoomRepository(_dbContext, configMock.Object, roomFileManagerMock.Object);

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
        List<Room> expected = await _dbContext.Rooms
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .ToListAsync();

        // Act
        var result = await _roomRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new RoomEqualityComparer()));

        Assert.That(result.Select(r => r.Owner).OrderBy(r => r!.Id),
            Is.EqualTo(expected.Select(r => r.Owner).OrderBy(r => r!.Id)).Using(new UserEqualityComparer()));

        Assert.That(result.SelectMany(r => r.JoinedUsers).OrderBy(r => r.Id),
            Is.EqualTo(expected.SelectMany(r => r.JoinedUsers).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));
    }

    [Test]
    [TestCase(1)]
    public async Task GetByIdAsync_ReturnsRoom_WhenFound(long id)
    {
        // Arrange
        Room expected = await _dbContext.Rooms
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .FirstAsync(r => r.Id == id);

        // Act
        var result = await _roomRepository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new RoomEqualityComparer()));
        Assert.That(result.Owner, Is.EqualTo(expected.Owner).Using(new UserEqualityComparer()));
        Assert.That(result.JoinedUsers, Is.EqualTo(expected.JoinedUsers).Using(new UserEqualityComparer()));
    }

    [Test]
    [TestCase(404)]
    public async Task GetByIdAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound(long id)
    {
        // Act
        AsyncTestDelegate act = async () => await _roomRepository.GetByIdAsync(id);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    [TestCase("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e")]
    public async Task GetByGuidAsync_ReturnsRoom_WhenFound(string guid)
    {
        // Arrange
        Room expected = await _dbContext.Rooms
            .Include(nameof(Room.Owner))
            .Include(nameof(Room.JoinedUsers))
            .FirstAsync(r => r.Guid == guid);

        // Act
        var result = await _roomRepository.GetByGuidAsync(guid);

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new RoomEqualityComparer()));
        Assert.That(result.Owner, Is.EqualTo(expected.Owner).Using(new UserEqualityComparer()));
        Assert.That(result.JoinedUsers, Is.EqualTo(expected.JoinedUsers).Using(new UserEqualityComparer()));
    }

    [Test]
    [TestCase("404")]
    public async Task GetByGuidAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound(string guid)
    {
        // Act
        AsyncTestDelegate act = async () => await _roomRepository.GetByGuidAsync(guid);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
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
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
        };

        // Act
        await _roomRepository.AddAsync(roomToAdd);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Rooms.CountAsync() == 3);
    }

    [Test]
    public async Task AddAsync_AppliesJoinedUsersToRoom()
    {
        // Arrange
        var users = await _dbContext.Users.ToListAsync();

        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #3",
            OwnerId = 1,
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
            JoinedUsers = users,
        };

        // Act
        await _roomRepository.AddAsync(roomToAdd);
        await _roomRepository.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Rooms.FirstAsync(r => r.Guid == roomToAdd.Guid);
        Assert.NotNull(result.JoinedUsers);
        Assert.That(result.JoinedUsers, Is.EqualTo(roomToAdd.JoinedUsers).Using(new UserEqualityComparer()));
    }

    [Test]
    public async Task AddAsync_ThrowsStringTooLongException_WhenNameTooLong()
    {
        // Arrange
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = new string('a', 51),
            OwnerId = 1,
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
        };

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.AddAsync(roomToAdd);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsStringTooShortException_WhenNameTooShort()
    {
        // Arrange
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = new string('a', 1),
            OwnerId = 1,
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
        };

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.AddAsync(roomToAdd);

        // Assert
        Assert.ThrowsAsync<StringTooShortException>(act);
    }

    [Test]
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

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsUserNotFoundException_WhenOwnerWasNotFound()
    {
        // Arrange
        var roomToAdd = new Room()
        {
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #3",
            OwnerId = 4,
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
        };

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.AddAsync(roomToAdd);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task DeleteAsync_DeletesRoom()
    {
        // Arrange
        var roomCount = await _dbContext.Rooms.CountAsync();
        var roomToDelete = await _dbContext.Rooms.AsNoTracking().FirstAsync();

        // Act
        await _roomRepository.DeleteAsync(roomToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        // Ensure that room is now couldn't be found
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomToDelete.Id);
        Assert.Null(room);

        // Ensure that room count was reduced
        Assert.That(await _dbContext.Rooms.CountAsync() == (roomCount - 1));
    }

    [Test]
    public async Task DeleteAsync_DeletesAllRelatedAttachments()
    {
        // Arrange
        var roomToDelete = await _dbContext.Rooms.AsNoTracking().FirstAsync();

        // Act
        await _roomRepository.DeleteAsync(roomToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Attachments.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteAsync_DeletesAllRelatedMessages()
    {
        // Arrange
        var roomToDelete = await _dbContext.Rooms.AsNoTracking().FirstAsync();

        // Act
        await _roomRepository.DeleteAsync(roomToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 1);
    }

    [Test]
    public async Task DeleteAsync_DeletesAllRelatedReactions()
    {
        // Arrange
        var roomToDelete = await _dbContext.Rooms.FirstAsync();

        // Act
        await _roomRepository.DeleteAsync(roomToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteAsync_ThrowsRoomNotFoundException_WhenRoomDoesNotExist()
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
        AsyncTestDelegate act = async () => await _roomRepository.DeleteAsync(roomToDelete);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesRoom()
    {
        // Arrange
        var roomCount = await _dbContext.Rooms.CountAsync();
        var roomToDelete = await _dbContext.Rooms.FirstAsync();

        // Act
        await _roomRepository.DeleteByIdAsync(roomToDelete.Id);
        await _roomRepository.SaveChangesAsync();

        // Assert
        // Ensure that room is now couldn't be found
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomToDelete.Id);
        Assert.Null(room);

        // Ensure that room count was reduced
        Assert.That(await _dbContext.Rooms.CountAsync() == (roomCount - 1));
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        // Arrange
        var roomIdToDelete = (await _dbContext.Rooms.FirstAsync()).Id;

        // Act
        await _roomRepository.DeleteByIdAsync(roomIdToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Attachments.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedMessages()
    {
        // Arrange
        var roomIdToDelete = (await _dbContext.Rooms.FirstAsync()).Id;

        // Act
        await _roomRepository.DeleteByIdAsync(roomIdToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Messages.CountAsync() == 1);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        // Arrange
        var roomIdToDelete = (await _dbContext.Rooms.FirstAsync()).Id;

        // Act
        await _roomRepository.DeleteByIdAsync(roomIdToDelete);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFoundById()
    {
        // Arrange
        var roomIdToDelete = 10;

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.DeleteByIdAsync(roomIdToDelete);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesRoom()
    {
        // Arrange
        var newExpiryDate = DateTime.Now.Add(new TimeSpan(2, 0, 0));
        var newOwnerId = 2;

        var guid = Guid.NewGuid().ToString();
        var room = new Room
        {
            Id = 1,
            Guid = guid,
            Name = "Room #1",
            ExpiryDate = newExpiryDate,
            OwnerId = newOwnerId,
        };

        // Act
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync();

        // Assert
        Assert.That(room, Is.EqualTo(new Room()
        {
            Id = 1,
            Guid = guid,
            Name = "Room #1",
            ExpiryDate = newExpiryDate,
            OwnerId = newOwnerId,
        }).Using(new RoomEqualityComparer()));
    }

    [Test]
    public async Task Update_ThrowsStringTooLongException_WhenNameTooLong()
    {
        // Arrange
        var roomToUpdate = await _dbContext.Rooms.AsNoTracking().FirstAsync(r => r.Id == 1);
        roomToUpdate.Name = new string('a', 51);

        // Act
        TestDelegate act = () => _roomRepository.Update(roomToUpdate);

        // Assert
        Assert.Throws<StringTooLongException>(act);
    }

    [Test]
    public async Task Update_ThrowsStringTooShortException_WhenNameTooShort()
    {
        // Arrange
        var roomToUpdate = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        roomToUpdate.Name = new string('a', 1);

        // Act
        TestDelegate act = () => _roomRepository.Update(roomToUpdate);

        // Assert
        Assert.Throws<StringTooShortException>(act);
    }

    [Test]
    public async Task Update_ThrowsRoomNotFoundException_WhenRoomDoesNotExist()
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
        Assert.Throws<RoomNotFoundException>(act);
    }

    [Test]
    public async Task DeleteAllExpiredAsync_DeletesAllExpiredRooms()
    {
        // Arrange
        var roomCount = await _dbContext.Rooms.CountAsync();

        // Act
        await _roomRepository.DeleteAllExpiredAsync();
        await _roomRepository.SaveChangesAsync();

        // Assert
        var newRoomCount = await _dbContext.Rooms.CountAsync();
        var expiredRoom = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == 2);
        Assert.Null(expiredRoom);
        Assert.That(roomCount != newRoomCount);
    }

    [Test]
    public async Task DeleteAllExpiredAsync_ThrowsRoomNotFoundException_WhenNoExpiredRoomsExist()
    {
        // Arrange
        await _roomRepository.DeleteAllExpiredAsync();
        await _roomRepository.SaveChangesAsync();

        // Act
        AsyncTestDelegate act = async () => await _roomRepository.DeleteAllExpiredAsync();

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }
}