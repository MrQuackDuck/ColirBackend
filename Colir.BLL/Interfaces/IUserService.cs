using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;

namespace Colir.BLL.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Gets the info about the account
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when the user wasn't found</exception>
    Task<DetailedUserModel> GetAccountInfo(RequestToGetAccountInfo request);

    /// <summary>
    /// Authorizes the user as GitHub user (i.e: returns account data of the user)
    ///
    /// If the user was found by GitHub Id, their data will be returned
    /// Otherwise, a new user with provided HexId and Username will be created
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when provided HexId is not unique</exception>
    /// <exception cref="StringTooShortException">Thrown when a username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when a username is too long</exception>
    Task<DetailedUserModel> AuthorizeViaGitHubAsync(RequestToAuthorizeViaGitHub request);

    /// <summary>
    /// Authorizes the user as Google user (i.e: returns account data of the user)
    ///
    /// If the user was found by Google Id, their data will be returned
    /// Otherwise, a new user with provided HexId and Username will be created
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when provided HexId is not unique</exception>
    /// <exception cref="StringTooShortException">Thrown when a username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when a username is too long</exception>
    Task<DetailedUserModel> AuthorizeViaGoogleAsync(RequestToAuthorizeViaGoogle request);

    /// <summary>
    /// Creates a new user with provided username and returns its data instantly
    /// </summary>
    /// <exception cref="StringTooShortException">Thrown when a username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when a username is too long</exception>
    Task<DetailedUserModel> AuthorizeAsAnnoymousAsync(RequestToAuthorizeAsAnnoymous request);

    /// <summary>
    /// Changes the username for an user
    /// </summary>
    /// <exception cref="StringTooShortException">Thrown when new username is too short</exception>
    /// <exception cref="StringTooLongException">Thrown when new username is too long</exception>
    Task<DetailedUserModel> ChangeUsernameAsync(RequestToChangeUsername request);

    /// <summary>
    /// Changes the settings for the user
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task ChangeSettingsAsync(RequestToChangeSettings request);

    /// <summary>
    /// Deletes the account of the user
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    Task DeleteAccount(RequestToDeleteAccount request);
}