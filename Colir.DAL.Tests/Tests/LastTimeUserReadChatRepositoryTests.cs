using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class LastTimeUserReadChatRepositoryTests : ILastTimeUserReadChatRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllTimesUsersReadChats()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetAsync_ReturnsEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetAsync_ThrowsNotFoundException_WhenEntityWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ReturnsEntity_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenEntityWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsInvalidOperationException_WhenEntryWithSameUserIdAndRoomIdAlreadyExists()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenEntityWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsArgumentException_WhenProvidedAnotherRoomId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}