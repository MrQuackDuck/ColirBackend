using Colir.BLL.Tests.Interfaces;

namespace Colir.BLL.Tests.Tests;

public class RoomServiceTests : IRoomServiceTests
{
    [Test]
    public async Task GetRoomInfoAsync_ReturnsRoomInfo()
    {
        throw new NotImplementedException();
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