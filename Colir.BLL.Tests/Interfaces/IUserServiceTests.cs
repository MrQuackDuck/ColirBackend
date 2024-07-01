namespace Colir.BLL.Tests.Interfaces;

public interface IUserServiceTests
{
    void AuthorizeWithGitHubAsync_CreatesUser();
    void AuthorizeWithGitHubAsync_ReturnsCorrectData();
    void AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenHexIsNotUnique();
    void AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenAuthTypeIsNotGithub();

    void AuthorizeAsAnnoymousAsync_CreatesUser();
    void AuthorizeAsAnnoymousAsync_ReturnsCorrectData();

    void ChangeUsernameAsync_ChangesUsername();
    void ChangeUsernameAsync_ThrowsArgumentException_WhenNewUsernameTooShort();
    void ChangeUsernameAsync_ThrowsArgumentException_WhenNewUsernameTooLong();

    void ChangeSettingsAsync_UpdatesSettings();
    void ChangeSettingsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();

    void DeleteAccount_DeletesAccount();
    void DeleteAccount_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
}