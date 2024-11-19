﻿using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.BLL.RequestModels.UserStatistics;
using Colir.Communication.Enums;
using Colir.Communication.RequestModels.User;
using Colir.Communication.ResponseModels;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using Colir.Hubs;
using Colir.Interfaces.Controllers;
using Colir.Misc.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class UserController : ControllerBase, IUserController
{
    private readonly IUserService _userService;
    private readonly IUserStatisticsService _userStatisticsService;
    private readonly IHubContext<ChatHub> _chatHub;

    public UserController(IUserService userService, IUserStatisticsService userStatisticsService,
        IHubContext<ChatHub> chatHub)
    {
        _userService = userService;
        _userStatisticsService = userStatisticsService;
        _chatHub = chatHub;
    }

    /// <inheritdoc cref="IUserController.GetAccountInfo"/>
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

    /// <inheritdoc cref="IUserController.GetStatistics"/>
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

    /// <inheritdoc cref="IUserController.ChangeSettings"/>
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

    /// <inheritdoc cref="IUserController.ChangeUsername"/>
    [HttpPut]
    public async Task<ActionResult> ChangeUsername(ChangeUsernameModel model)
    {
        try
        {
            var issuerId = this.GetIssuerId();
            var request = new RequestToChangeUsername
            {
                IssuerId = issuerId,
                DesiredUsername = model.NewName
            };

            var joinedRooms = (await _userService
                .GetAccountInfo(new() { IssuerId = issuerId }))
                .JoinedRooms;

            // Notifying users in the Chat hub that the user was renamed
            foreach (var room in joinedRooms)
            {
                await _chatHub.Clients.Group(room.Guid).SendAsync("UserRenamed", (issuerId, model.NewName));
            }

            return Ok(await _userService.ChangeUsernameAsync(request));
        }
        catch (StringTooShortException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooShort));
        }
        catch (StringTooLongException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.StringWasTooLong));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
    }

    /// <inheritdoc cref="IUserController.DeleteAccount"/>
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