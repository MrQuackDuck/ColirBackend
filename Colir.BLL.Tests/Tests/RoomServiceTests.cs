using Colir.BLL.Models;
using Colir.BLL.RequestModels.Room;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
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
        var mock = new Mock<IConfiguration>();
        
        var unitOfWork = new UnitOfWork(_dbContext, mock.Object);
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
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInTheRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetRoomInfoAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task CreateAsync_CreatesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task CreateAsync_ReturnsRoomGuid()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task CreateAsync_AddsToStatistics_WhenItsEnabled()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentExcpetion_WhenWrongExpiryDateWasProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task CreateAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
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