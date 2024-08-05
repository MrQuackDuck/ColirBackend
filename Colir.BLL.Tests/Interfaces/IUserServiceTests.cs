namespace Colir.BLL.Tests.Interfaces;

public interface IUserServiceTests
{
    Task GetAccountInfo_ReturnsUser();
    Task GetAccountInfo_ThrowsUserNotFoundException_WhenIssuerWasNotFound();

    Task AuthorizeViaGitHubAsync_CreatesUser();
    Task AuthorizeViaGitHubAsync_ReturnsCorrectData();
    Task AuthorizeViaGitHubAsync_ThrowsArgumentException_WhenHexIsNotUnique();
    Task AuthorizeViaGitHubAsync_ThrowsStringTooShortException_WhenNewUsernameTooShort();
    Task AuthorizeViaGitHubAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong();

    Task AuthorizeAsAnnoymousAsync_CreatesUser();
    Task AuthorizeAsAnnoymousAsync_ReturnsCorrectData();
    Task AuthorizeAsAnnoymousAsync_ThrowsStringTooShortException_WhenNewUsernameTooShort();
    Task AuthorizeAsAnnoymousAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong();

    Task ChangeUsernameAsync_ChangesUsername();
    Task ChangeUsernameAsync_StringTooShortException_WhenNewUsernameTooShort();
    Task ChangeUsernameAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong();

    Task ChangeSettingsAsync_UpdatesSettings();
    Task ChangeSettingsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();

    Task DeleteAccount_DeletesAccount();
    Task DeleteAccount_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
}