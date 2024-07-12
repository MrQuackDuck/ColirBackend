namespace Colir.BLL.Tests.Interfaces;

public interface IUserServiceTests
{
    Task AuthorizeWithGitHubAsync_CreatesUser();
    Task AuthorizeWithGitHubAsync_ReturnsCorrectData();
    Task AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenHexIsNotUnique();

    Task AuthorizeAsAnnoymousAsync_CreatesUser();
    Task AuthorizeAsAnnoymousAsync_ReturnsCorrectData();

    Task ChangeUsernameAsync_ChangesUsername();
    Task ChangeUsernameAsync_StringTooShortException_WhenNewUsernameTooShort();
    Task ChangeUsernameAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong();

    Task ChangeSettingsAsync_UpdatesSettings();
    Task ChangeSettingsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();

    Task DeleteAccount_DeletesAccount();
    Task DeleteAccount_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
}