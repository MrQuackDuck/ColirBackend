using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class ReactionRepositoryTests : IReactionRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllReactions()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ReturnsRoom_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenRoomWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetReactionsOnMessage_ReturnsAllReactionsOnMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetReactionsOnMessage_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AppliesJoinedUsersToRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenAuthorWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenRoomWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesRoom()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenRoomDoesNotExist()
    {
        throw new NotImplementedException();
    }
}