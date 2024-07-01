using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class UserRepositoryTests : IUserRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllUsers()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ReturnsUser_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByHexIdAsync_ReturnsUser_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AppliesJoinedRoomsToUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenHexAlreadyExists()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenUsernameTooShort()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsArgumentException_WhenUsernameTooLong()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFound_WhenOneOfJoinedRoomsWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsRoomExpiredException_WhenOneOfJoinedRoomsIsExpired()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenUserWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}