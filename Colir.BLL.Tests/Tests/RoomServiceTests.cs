using Colir.BLL.Tests.Interfaces;

namespace Colir.BLL.Tests.Tests;

public class RoomServiceTests : IRoomServiceTests
{
    [Test]
    public void GetRoomInfoAsync_ReturnsRoomInfo()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetRoomInfoAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetRoomInfoAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetRoomInfoAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInTheRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetRoomInfoAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void CreateAsync_CreatesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void CreateAsync_ReturnsRoomGuid()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void CreateAsync_ThrowsArgumentExcpetion_WhenWrongExpiryDateWasProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void CreateAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RenameAsync_RenamesTheRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RenameAsync_ThrowsArgumentException_WhenNewNameIsTooLong()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RenameAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RenameAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void RenameAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAsync_DeletesTheRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastTimeUserReadChatAsync_ReturnsLastTimeUserReadChat()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastTimeUserReadChatAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastTimeUserReadChatAsync_ThrowsArgumentException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void UpdateLastTimeUserReadChatAsync_UpdatesLastTimeUserReadChat()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void UpdateLastTimeUserReadChatAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void UpdateLastTimeUserReadChatAsync_ThrowsArgumentException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void UpdateLastTimeUserReadChatAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void JoinMemberAsync_JoinsUserToRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void JoinMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void JoinMemberAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void KickMemberAsync_KicksUserFromRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void KickMemberAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void KickMemberAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void KickMemberAsync_ThrowsUserNotFoundException_WhenTargetWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotInRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void KickMemberAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ClearRoomAsync_ReturnsClearProcessObject()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ClearRoomAsync_ClearProcessObjectHasFilesToDeletePropertyAboveZero()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ClearRoomAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ClearRoomAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ClearRoomAsync_ThrowsNotEnoughPermissionsException_WhenIssuerIsNotOwnerOfRoom()
    {
        throw new NotImplementedException();
    }
}