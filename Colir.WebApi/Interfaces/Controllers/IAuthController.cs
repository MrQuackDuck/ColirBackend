using Colir.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IAuthController
{
    Task<ActionResult<DetailedUserModel>> AnonnymousLogin(string name);
    Task<ActionResult> Logout();
}