using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class RoomRepositoryTests : IRoomRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllRooms()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ReturnsRoom_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AppliesJoinedUsersToRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenWrongExpiryDateWasProvided()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenOwnerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesAllRelatedMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesAllRelatedAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesAllRelatedMessages()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesAllRelatedReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAllExpiredAsync_DeletesAllExpiredRooms()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAllExpiredAsync_ThrowsNotFoundException_WhenNoExpiredRoomsExist()
    {
        throw new NotImplementedException();
    }
}