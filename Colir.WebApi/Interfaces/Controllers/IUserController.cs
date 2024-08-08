using Colir.BLL.Models;
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
    /// Deletes the account
    /// </summary>
    Task<ActionResult> DeleteAccount();
}