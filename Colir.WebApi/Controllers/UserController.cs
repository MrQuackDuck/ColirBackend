﻿using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.BLL.RequestModels.UserStatistics;
using Colir.Communication;
using Colir.Exceptions.NotFound;
using Colir.Extensions;
using Colir.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class UserController : ControllerBase, IUserController
{
    private readonly IUserService _userService;
    private readonly IUserStatisticsService _userStatisticsService;
    
    public UserController(IUserService userService, IUserStatisticsService userStatisticsService)
    {
        _userService = userService;
        _userStatisticsService = userStatisticsService;
    }
    
    [HttpGet]
    public async Task<ActionResult<DetailedUserModel>> GetAccountInfo()
    {
        try
        {
            var request = new RequestToGetAccountInfo
            {
                IssuerId = this.GetIssuerId()
            };

            return await _userService.GetAccountInfo(request);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }

    [HttpGet]
    public async Task<ActionResult<UserStatisticsModel>> GetStatistics()
    {
        try
        {
            var request = new RequestToGetStatistics
            {
                IssuerId = this.GetIssuerId()
            };

            return Ok(await _userStatisticsService.GetStatisticsAsync(request));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }

    [HttpPut]
    public async Task<ActionResult> ChangeSettings(UserSettingsModel newSettings)
    {
        try
        {
            var request = new RequestToChangeSettings
            {
                IssuerId = this.GetIssuerId(),
                NewSettings = newSettings
            };

            await _userService.ChangeSettingsAsync(request);

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        try
        {
            var request = new RequestToDeleteAccount()
            {
                IssuerId = this.GetIssuerId()
            };

            await _userService.DeleteAccount(request);

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }
}