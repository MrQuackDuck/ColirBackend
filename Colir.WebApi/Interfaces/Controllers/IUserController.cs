using Colir.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IUserController
{
    Task<ActionResult<DetailedUserModel>> GetAccountInfo();
    Task<ActionResult<UserStatisticsModel>> GetStatistics();
    Task<ActionResult> ChangeSettings(UserSettingsModel newSettings);
    Task<ActionResult> DeleteAccount();
}