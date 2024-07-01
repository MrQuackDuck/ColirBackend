using Colir.BLL.Tests.Interfaces;

namespace Colir.BLL.Tests.Tests;

public class UserServiceTests : IUserServiceTests
{
    [Test]
    public void AuthorizeWithGitHubAsync_CreatesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AuthorizeWithGitHubAsync_ReturnsCorrectData()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenHexIsNotUnique()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenAuthTypeIsNotGithub()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AuthorizeAsAnnoymousAsync_CreatesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AuthorizeAsAnnoymousAsync_ReturnsCorrectData()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ChangeUsernameAsync_ChangesUsername()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ChangeUsernameAsync_ThrowsArgumentException_WhenNewUsernameTooShort()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ChangeUsernameAsync_ThrowsArgumentException_WhenNewUsernameTooLong()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ChangeSettingsAsync_UpdatesSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ChangeSettingsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAccount_DeletesAccount()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteAccount_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }
}