using Colir.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IAuthController
{
    ActionResult<DetailedUserModel> AnonnymousLogin(string name);
}