using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Room;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class RoomServiceTests : IRoomServiceTests
{
    private ColirDbContext _dbContext;
    private RoomService _roomService;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object);
        _roomService = new RoomService(unitOfWork);

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
    public async Task GetRoomInfoAsync_ReturnsRoomInfo()
    {
        // Arrange
        var request = new RequestToGetRoomInfo
        {
            IssuerId = 1,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        var expected = _dbContext.Rooms
            .Include(nameof(Room.Owner))
            .First(r => r.Guid == "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e")
            .ToRoomModel();

        // Act
        var result = await _roomService.GetRoomInfoAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new RoomModelEqualityComparer()));
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        // Arrange
        var request = new RequestToGetRoomInfo
        {
            IssuerId = 1,
            RoomGuid = "12ffb712-aca7-416f-b899-8f9aaac6770f"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetRoomInfoAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomExpiredException>(act);
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToGetRoomInfo
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetRoomInfoAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInTheRoom()
    {
        // Arrange
        var request = new RequestToGetRoomInfo
        {
            IssuerId = 3,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetRoomInfoAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var request = new RequestToGetRoomInfo
        {
            IssuerId = 404,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetRoomInfoAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task CreateAsync_CreatesRoom()
    {
        // Arrange
        var request = new RequestToCreateRoom
        {
            IssuerId = 1,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        Assert.That(_dbContext.Rooms.Count() == 3);
    }

    [Test]
    public async Task CreateAsync_ReturnsRoomGuid()
    {
        // Arrange
        var request = new RequestToCreateRoom
        {
            IssuerId = 1,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        var result = await _roomService.CreateAsync(request);

        // Assert
        Assert.That(result.Length == 36);
    }

    [Test]
    public async Task CreateAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = _dbContext.UserStatistics.AsNoTracking().First(u => u.UserId == 1);
        var request = new RequestToCreateRoom
        {
            IssuerId = 1,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        var statsAfter = _dbContext.UserStatistics.AsNoTracking().First(u => u.UserId == 1);
        Assert.That(statsAfter.RoomsCreated - statsBefore.RoomsCreated == 1);
    }

    [Test]
    public async Task CreateAsync_NotAddsToStatistics_WhenItsNotEnabled()
    {
        // Arrange
        var statsBefore = _dbContext.UserStatistics.AsNoTracking().First(u => u.UserId == 1);
        var request = new RequestToCreateRoom
        {
            IssuerId = 2,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        var statsAfter = _dbContext.UserStatistics.AsNoTracking().First(u => u.UserId == 1);
        Assert.That(statsAfter.RoomsCreated - statsBefore.RoomsCreated == 0);
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentExcpetion_WhenWrongExpiryDateWasProvided()
    {
        // Arrange
        var request = new RequestToCreateRoom
        {
            IssuerId = 1,
            Name = "Room #3",
            ExpiryDate = new DateTime(1990, 1, 1)
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.CreateAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task CreateAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var request = new RequestToCreateRoom
        {
            IssuerId = 404,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.CreateAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task RenameAsync_RenamesTheRoom()
    {
        // Arrange
        var roomToRename = _dbContext.Rooms.First(r => r.Id == 1);

        var request = new RequestToRenameRoom
        {
            IssuerId = 1,
            RoomGuid = roomToRename.Guid,
            NewName = "New name"
        };


        // Act
        await _roomService.RenameAsync(request);

        // Assert
        var roomAfter = _dbContext.Rooms.First(r => r.Id == 1);
        Assert.That(roomAfter.Name == "New name");
    }

    [Test]
    public async Task RenameAsync_ThrowsStringTooLongException_WhenNewNameIsTooLong()
    {
        // Arrange
        var roomToRename = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToRenameRoom
        {
            IssuerId = 1,
            RoomGuid = roomToRename.Guid,
            NewName = new string('a', 51)
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task RenameAsync_ThrowsStringTooShortException_WhenNewNameIsTooShort()
    {
        // Arrange
        var roomToRename = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToRenameRoom
        {
            IssuerId = 1,
            RoomGuid = roomToRename.Guid,
            NewName = new string('a', 1)
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooShortException>(act);
    }

    [Test]
    public async Task RenameAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToRenameRoom
        {
            IssuerId = 1,
            RoomGuid = "404",
            NewName = "New name"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task RenameAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        // Arrange
        var request = new RequestToRenameRoom
        {
            IssuerId = 1,
            RoomGuid = "404",
            NewName = "New name"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task RenameAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var roomToRename = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToRenameRoom
        {
            IssuerId = 2,
            RoomGuid = roomToRename.Guid,
            NewName = "New name"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task DeleteAsync_DeletesTheRoom()
    {
        // Arrange
        var roomToDelete = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToDeleteRoom
        {
            IssuerId = 1,
            RoomGuid = roomToDelete.Guid
        };

        // Act
        await _roomService.DeleteAsync(request);

        // Assert
        Assert.That(_dbContext.Rooms.Count() == 1);
    }

    [Test]
    public async Task DeleteAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToDeleteRoom
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task DeleteAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        // Arrange
        var roomToDelete = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToDeleteRoom
        {
            IssuerId = 2,
            RoomGuid = roomToDelete.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task DeleteAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var roomToDelete = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToDeleteRoom
        {
            IssuerId = 404,
            RoomGuid = roomToDelete.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ReturnsLastTimeUserReadChat()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var expectedTime = _dbContext.LastTimeUserReadChats.First(u => u.UserId == 1).Timestamp;
        var request = new RequestToGetLastTimeUserReadChat
        {
            IssuerId = 1,
            RoomGuid = room.Guid
        };

        // Act
        var result = await _roomService.GetLastTimeUserReadChatAsync(request);

        // Assert
        Assert.That(result == expectedTime);
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToGetLastTimeUserReadChat
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsArgumentException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastTimeUserReadChat
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToGetLastTimeUserReadChat
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_UpdatesLastTimeUserReadChat()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUpdateLastTimeUserReadChat
        {
            IssuerId = 1,
            RoomGuid = room.Guid
        };

        // Act
        await _roomService.UpdateLastTimeUserReadChatAsync(request);

        var lastTimeUserReadChat = _dbContext.LastTimeUserReadChats.First(l => l.UserId == 1 && l.RoomId == room.Id).Timestamp;

        // Assert (approximately)
        Assert.That(DateTime.Now - lastTimeUserReadChat < TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToUpdateLastTimeUserReadChat
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_ThrowsArgumentException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUpdateLastTimeUserReadChat
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToUpdateLastTimeUserReadChat
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task JoinMemberAsync_JoinsUserToRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToJoinRoom
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        await _roomService.JoinMemberAsync(request);

        // Assert
        var roomAfter = _dbContext.Rooms.Include(nameof(Room.JoinedUsers)).First(r => r.Id == 1);
        Assert.That(roomAfter.JoinedUsers.Count() == 3);
    }

    [Test]
    public async Task JoinMemberAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = _dbContext.UserStatistics.AsNoTracking().First(u => u.UserId == 3);
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToJoinRoom
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        await _roomService.JoinMemberAsync(request);

        // Assert
        var statsAfter = _dbContext.UserStatistics.AsNoTracking().First(s => s.UserId == 3);
        Assert.That(statsAfter.RoomsJoined - statsBefore.RoomsJoined == 1);
    }

    [Test]
    public async Task JoinMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToJoinRoom
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.JoinMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task JoinMemberAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToJoinRoom
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.JoinMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task KickMemberAsync_KicksUserFromRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            TargetHexId = "#000000"
        };

        // Act
        await _roomService.KickMemberAsync(request);

        // Assert
        var roomAfter = _dbContext.Rooms.Include(nameof(Room.JoinedUsers)).First(r => r.Id == 1);
        Assert.That(roomAfter.JoinedUsers.Count == 1);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToKickMember
        {
            IssuerId = 1,
            RoomGuid = "404",
            TargetHexId = "#000000"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 404,
            RoomGuid = room.Guid,
            TargetHexId = "#000000"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsUserNotFoundException_WhenTargetWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            TargetHexId = "#404040"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 3,
            RoomGuid = room.Guid,
            TargetHexId = "#000000"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 2,
            RoomGuid = room.Guid,
            TargetHexId = "#000000"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task ClearRoomAsync_ReturnsClearProcessObject()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToClearRoom
        {
            IssuerId = 1,
            RoomGuid = room.Guid
        };

        // Act
        var result = await _roomService.ClearRoom(request);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToClearRoom
        {
            IssuerId = 1,
            RoomGuid = "404",
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.ClearRoom(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToClearRoom
        {
            IssuerId = 404,
            RoomGuid = room.Guid,
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.ClearRoom(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        // Arrange
        var room = _dbContext.Rooms.First(r => r.Id == 1);
        var request = new RequestToClearRoom
        {
            IssuerId = 2,
            RoomGuid = room.Guid,
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.ClearRoom(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }
}