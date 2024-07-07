using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class LastTimeUserReadChatRepositoryTests : ILastTimeUserReadChatRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllTimesUsersReadChats()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetAsync_ReturnsEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetAsync_ThrowsNotFoundException_WhenEntityWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ReturnsEntity_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenEntityWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsInvalidOperationException_WhenEntryWithSameUserIdAndRoomIdAlreadyExists()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenEntityWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesEntity()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsArgumentException_WhenProvidedAnotherRoomId()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsRoomExpiredException_WhenRoomExpired()
    {
        throw new NotImplementedException();
    }
}