using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class ReactionRepositoryTests : IReactionRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllReactions()
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
    public void GetReactionsOnMessage_ReturnsAllReactionsOnMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetReactionsOnMessage_ThrowsNotFoundException_WhenMessageWasNotFound()
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
    public void AddAsync_ThrowsArgumentException_WhenAuthorWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesRoom()
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
    public void Delete_ThrowsNotFoundException_WhenRoomWasNotFoundById()
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
}