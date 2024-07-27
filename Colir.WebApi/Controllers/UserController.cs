using Colir.BLL.Models;
using Colir.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class UserController : ControllerBase, IUserController
{
    [HttpGet]
    public async Task<ActionResult<DetailedUserModel>> GetAccountInfo()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<UserStatisticsModel>> GetStatistics()
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    public async Task<ActionResult> ChangeSettings(UserSettingsModel newSettings)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        throw new NotImplementedException();
    }
}