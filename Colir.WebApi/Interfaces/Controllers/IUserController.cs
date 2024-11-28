using Colir.BLL.Models;
using Colir.Communication.RequestModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IUserController
{
    /// <summary>
    /// Gets the info about the user
    /// </summary>
    Task<ActionResult<DetailedUserModel>> GetAccountInfo();

    /// <summary>
    /// Gets stats about the user
    /// </summary>
    Task<ActionResult<UserStatisticsModel>> GetStatistics();

    /// <summary>
    /// Updates the settings for the user
    /// </summary>
    /// <param name="newSettings">New settings to set</param>
    Task<ActionResult> ChangeSettings(UserSettingsModel newSettings);

    /// <summary>
    /// Changes the name of the user and notifies users in common rooms with the "UserRenamed" signal
    /// </summary>
    /// <param name="model">New name to set</param>
    Task<ActionResult> ChangeUsername(ChangeUsernameModel model);

    /// <summary>
    /// Deletes the user's account
    /// </summary>
    Task<ActionResult> DeleteAccount();
}