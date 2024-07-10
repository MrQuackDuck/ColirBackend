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

        // Initialize room service
        var configMock = new Mock<IConfiguration>();
        var hexGeneratorMock = new Mock<IHexColorGenerator>();

        hexGeneratorMock.Setup(g => g.GetUniqueHexColor()).Returns("1051b310-ed1a-42ef-9435-fe840ec44009");

        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object);
        _roomService = new RoomService(unitOfWork, hexGeneratorMock.Object);

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
        Assert.That(result == "1051b310-ed1a-42ef-9435-fe840ec44009");
    }

    [Test]
    public async Task CreateAsync_AddsToStatistics_WhenItsEnabled()
    {
        // Arrange
        var statsBefore = _dbContext.UserStatistics.First(u => u.UserId == 1);
        var request = new RequestToCreateRoom
        {
            IssuerId = 1,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        var statsAfter = _dbContext.UserStatistics.First(u => u.UserId == 1);
        Assert.That(statsAfter.RoomsCreated - statsBefore.RoomsCreated == 1);
    }

    [Test]
    public async Task CreateAsync_NotAddsToStatistics_WhenItsNotEnabled()
    {
        // Arrange
        var statsBefore = _dbContext.UserStatistics.First(u => u.UserId == 1);
        var request = new RequestToCreateRoom
        {
            IssuerId = 2,
            Name = "Room #3",
            ExpiryDate = DateTime.Now.AddDays(1)
        };

        // Act
        await _roomService.CreateAsync(request);

        // Assert
        var statsAfter = _dbContext.UserStatistics.First(u => u.UserId == 1);
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
        throw new NotImplementedException();
    }

    [Test]
    public async Task RenameAsync_ThrowsArgumentException_WhenNewNameIsTooLong()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RenameAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RenameAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task RenameAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAsync_DeletesTheRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ReturnsLastTimeUserReadChat()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsArgumentException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_UpdatesLastTimeUserReadChat()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_ThrowsArgumentException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task UpdateLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task JoinMemberAsync_JoinsUserToRoom()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task JoinMemberAsync_AddsToStatistics_WhenItsEnabled()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task JoinMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task JoinMemberAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task KickMemberAsync_KicksUserFromRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task KickMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task KickMemberAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task KickMemberAsync_ThrowsUserNotFoundException_WhenTargetWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ClearRoomAsync_ReturnsClearProcessObject()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ClearRoomAsync_ClearProcessObjectHasFilesToDeletePropertyAboveZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ClearRoomAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }
}