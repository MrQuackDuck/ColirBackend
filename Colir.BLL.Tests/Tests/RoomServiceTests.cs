using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Room;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Interfaces;
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
        configMock.Setup(config => config["AppSettings:MinRoomNameLength"]).Returns("2");
        configMock.Setup(config => config["AppSettings:MaxRoomNameLength"]).Returns("50");
        configMock.Setup(config => config["AppSettings:MinUsernameLength"]).Returns("2");
        configMock.Setup(config => config["AppSettings:MaxUsernameLength"]).Returns("50");

        var roomCleanerMock = new Mock<IRoomCleaner>();

        var roomFileMangerMock = new Mock<IRoomFileManager>();
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object, roomFileMangerMock.Object);

        var roomCleanerFactoryMock = new Mock<IRoomCleanerFactory>();
        roomCleanerFactoryMock.Setup(factory => factory.GetRoomCleaner("cbaa8673-ea8b-43f8-b4cc-b8b0797b620e", unitOfWork, configMock.Object))
            .Returns(roomCleanerMock.Object);

        var mapper = AutomapperProfile.InitializeAutoMapper().CreateMapper();

        _roomService = new RoomService(unitOfWork, mapper, roomCleanerFactoryMock.Object, configMock.Object);

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

        var expected = (await _dbContext.Rooms
                .Include(nameof(Room.Owner))
                .FirstAsync(r => r.Guid == "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e"))
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
    public async Task GetRoomInfoAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
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
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
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
        Assert.That(await _dbContext.Rooms.CountAsync() == 3);
    }

    [Test]
    public async Task CreateAsync_ReturnsRoomModel()
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
        Assert.That(result.Guid.Length == 36);
    }

    [Test]
    public async Task CreateAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(u => u.UserId == 1);
        var request = new RequestToCreateRoom
        {
            IssuerId = 1,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        var statsAfter = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(u => u.UserId == 1);
        Assert.That(statsAfter.RoomsCreated - statsBefore.RoomsCreated == 1);
    }

    [Test]
    public async Task CreateAsync_NotAddsToStatistics_WhenItsNotEnabled()
    {
        // Arrange
        var statsBefore = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(u => u.UserId == 1);
        var request = new RequestToCreateRoom
        {
            IssuerId = 2,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        var statsAfter = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(u => u.UserId == 1);
        Assert.That(statsAfter.RoomsCreated - statsBefore.RoomsCreated == 0);
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenWrongExpiryDateWasProvided()
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
        var roomToRename = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);

        var request = new RequestToRenameRoom
        {
            IssuerId = 1,
            RoomGuid = roomToRename.Guid,
            NewName = "New name"
        };


        // Act
        await _roomService.RenameAsync(request);

        // Assert
        var roomAfter = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        Assert.That(roomAfter.Name == "New name");
    }

    [Test]
    public async Task RenameAsync_ThrowsStringTooLongException_WhenNewNameIsTooLong()
    {
        // Arrange
        var roomToRename = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var roomToRename = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
            IssuerId = 2,
            RoomGuid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e",
            NewName = "New name"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task RenameAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var roomToRename = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToRenameRoom
        {
            IssuerId = 404,
            RoomGuid = roomToRename.Guid,
            NewName = "New name"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.RenameAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task DeleteAsync_DeletesTheRoom()
    {
        // Arrange
        var roomToDelete = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToDeleteRoom
        {
            IssuerId = 1,
            RoomGuid = roomToDelete.Guid
        };

        // Act
        await _roomService.DeleteAsync(request);

        // Assert
        Assert.That(await _dbContext.Rooms.CountAsync() == 1);
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
        var roomToDelete = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var roomToDelete = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToDeleteRoom
        {
            IssuerId = 404,
            RoomGuid = roomToDelete.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.DeleteAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ReturnsLastTimeUserReadChat()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var expectedTime = (await _dbContext.LastTimeUserReadChats
            .FirstAsync(u => u.UserId == 1))
            .Timestamp;

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
    public async Task GetLastTimeUserReadChatAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToGetLastTimeUserReadChat
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToGetLastTimeUserReadChat
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.GetLastTimeUserReadChatAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task UpdateLastReadMessageByUser_UpdatesLastTimeUserReadChatWithMessageProvided()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var message = await _dbContext.Messages.FirstAsync(m => m.Id == 2);
        var request = new RequestToUpdateLastReadMessageByUser
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            MessageId = message.Id
        };

        // Act
        await _roomService.UpdateLastReadMessageByUser(request);

        var lastTimeUserReadChat =
            (await _dbContext
                .LastTimeUserReadChats
                .FirstAsync(l => l.UserId == 1 && l.RoomId == room.Id))
            .Timestamp;

        // Assert
        Assert.That(lastTimeUserReadChat == message.PostDate);
    }

    [Test]
    public async Task UpdateLastReadMessageByUser_ThrowsInvalidActionException_WhenOlderMessageWasProvided()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var message = await _dbContext.Messages.FirstAsync(m => m.Id == 1);
        var request = new RequestToUpdateLastReadMessageByUser
        {
            IssuerId = 2,
            RoomGuid = room.Guid,
            MessageId = message.Id
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastReadMessageByUser(request);

        // Assert
        Assert.ThrowsAsync<InvalidActionException>(act);
    }

    [Test]
    public async Task UpdateLastReadMessageByUser_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToUpdateLastReadMessageByUser
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            MessageId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastReadMessageByUser(request);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task UpdateLastReadMessageByUser_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToUpdateLastReadMessageByUser
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastReadMessageByUser(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task UpdateLastReadMessageByUser_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToUpdateLastReadMessageByUser
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastReadMessageByUser(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task UpdateLastReadMessageByUser_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToUpdateLastReadMessageByUser
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.UpdateLastReadMessageByUser(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task JoinMemberAsync_JoinsUserToRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.JoinedUsers))
            .FirstAsync(r => r.Id == 1);

        var request = new RequestToJoinRoom
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        await _roomService.JoinMemberAsync(request);

        // Assert
        var roomAfter = await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.JoinedUsers))
            .FirstAsync(r => r.Id == 1);

        Assert.That(roomAfter.JoinedUsers.Count() == 3);
    }

    [Test]
    public async Task JoinMemberAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(u => u.UserId == 3);
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToJoinRoom
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        await _roomService.JoinMemberAsync(request);

        // Assert
        var statsAfter = await _dbContext.UserStatistics.AsNoTracking().FirstAsync(s => s.UserId == 3);
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
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
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
        var room = await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.JoinedUsers))
            .FirstAsync(r => r.Id == 1);

        var request = new RequestToKickMember
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            TargetHexId = 0x000000
        };

        // Act
        await _roomService.KickMemberAsync(request);

        // Assert
        var roomAfter = await _dbContext.Rooms
            .AsNoTracking()
            .Include(nameof(Room.JoinedUsers))
            .FirstAsync(r => r.Id == 1);

        var kickedUser = await _dbContext.Users
            .AsNoTracking()
            .Include(nameof(User.JoinedRooms))
            .FirstAsync(u => u.HexId == 0x000000);

        Assert.That(roomAfter.JoinedUsers.Count == 1);
        Assert.That(kickedUser.JoinedRooms.Count == 1);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToKickMember
        {
            IssuerId = 1,
            RoomGuid = "404",
            TargetHexId = 0x000000
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
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 404,
            RoomGuid = room.Guid,
            TargetHexId = 0x000000
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
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 1,
            RoomGuid = room.Guid,
            TargetHexId = 0x404040
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 3,
            RoomGuid = room.Guid,
            TargetHexId = 0x000000
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToKickMember
        {
            IssuerId = 2,
            RoomGuid = room.Guid,
            TargetHexId = 0x000000
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.KickMemberAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }

    [Test]
    public async Task LeaveAsync_RemovesUserFromRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.AsNoTracking().FirstAsync(r => r.Id == 1);
        var request = new RequestToLeaveFromRoom()
        {
            IssuerId = 1,
            RoomGuid = room.Guid
        };

        // Act
        await _roomService.LeaveAsync(request);

        // Assert
        var roomAfter = await _dbContext.Rooms.AsNoTracking().Include(nameof(Room.JoinedUsers)).FirstAsync(r => r.Id == 1);
        var userAfter = await _dbContext.Users.AsNoTracking().Include((nameof(User.JoinedRooms))).FirstAsync(u => u.Id == 1);
        Assert.That(roomAfter.JoinedUsers.Count == 1);
        Assert.That(userAfter.JoinedRooms.Count == 1);
    }

    [Test]
    public async Task LeaveAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        // Arrange
        var request = new RequestToLeaveFromRoom()
        {
            IssuerId = 1,
            RoomGuid = "404"
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.LeaveAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task LeaveAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.AsNoTracking().FirstAsync(r => r.Id == 1);
        var request = new RequestToLeaveFromRoom()
        {
            IssuerId = 404,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.LeaveAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task LeaveAsync_ThrowsIssuerNotInRoomException_WhenIssuerIsNotInRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.AsNoTracking().FirstAsync(r => r.Id == 1);
        var request = new RequestToLeaveFromRoom()
        {
            IssuerId = 3,
            RoomGuid = room.Guid
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.LeaveAsync(request);

        // Assert
        Assert.ThrowsAsync<IssuerNotInRoomException>(act);
    }

    [Test]
    public async Task ClearRoomAsync_ReturnsIRoomCleanerObject()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Guid == "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e");
        var request = new RequestToClearRoom
        {
            IssuerId = 1,
            RoomGuid = room.Guid
        };

        // Act
        var result = await _roomService.ClearRoomAsync(request);

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
        AsyncTestDelegate act = async () => await _roomService.ClearRoomAsync(request);

        // Assert
        Assert.ThrowsAsync<RoomNotFoundException>(act);
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToClearRoom
        {
            IssuerId = 404,
            RoomGuid = room.Guid,
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.ClearRoomAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        // Arrange
        var room = await _dbContext.Rooms.FirstAsync(r => r.Id == 1);
        var request = new RequestToClearRoom
        {
            IssuerId = 2,
            RoomGuid = room.Guid,
        };

        // Act
        AsyncTestDelegate act = async () => await _roomService.ClearRoomAsync(request);

        // Assert
        Assert.ThrowsAsync<NotEnoughPermissionsException>(act);
    }
}