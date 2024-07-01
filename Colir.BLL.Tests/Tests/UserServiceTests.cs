using Colir.BLL.Tests.Interfaces;

namespace Colir.BLL.Tests.Tests;

public class UserServiceTests : IUserServiceTests
{
    [Test]
    public async Task AuthorizeWithGitHubAsync_CreatesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ReturnsCorrectData()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenHexIsNotUnique()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenAuthTypeIsNotGithub()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AuthorizeAsAnnoymousAsync_CreatesUser()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AuthorizeAsAnnoymousAsync_ReturnsCorrectData()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ChangeUsernameAsync_ChangesUsername()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ChangeUsernameAsync_ThrowsArgumentException_WhenNewUsernameTooShort()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ChangeUsernameAsync_ThrowsArgumentException_WhenNewUsernameTooLong()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ChangeSettingsAsync_UpdatesSettings()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ChangeSettingsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAccount_DeletesAccount()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteAccount_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        throw new NotImplementedException();
    }
}