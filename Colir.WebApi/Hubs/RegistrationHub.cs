using System.Collections.Concurrent;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Communication.Enums;
using Colir.Communication.Models;
using Colir.Communication.ResponseModels;
using Colir.Exceptions.NotFound;
using Colir.Hubs.Abstract;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Interfaces.Hubs;
using DAL.Enums;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

/// <inheritdoc cref="IRegistrationHub"/>
[SignalRHub]
public class RegistrationHub : ColirHub, IRegistrationHub
{
    private readonly IUserService _userService;
    private readonly IOAuth2RegistrationQueueService _registrationQueueService;
    private readonly IHexColorGenerator _hexGenerator;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Dictionary to store hexs that are currently offered for users to choose from
    /// The connection id is a key and the value is a list of hexs to offer to specific user
    /// </summary>
    private static readonly ConcurrentDictionary<string, List<int>> HexsToOffer = new();

    /// <summary>
    /// Dictionary to store users' data needed for registration process
    /// The connection id is a key and the value is user's data
    /// </summary>
    private static readonly ConcurrentDictionary<string, RegistrationUserData> UsersData = new();

    /// <summary>
    /// Dictionary to store chosen by users hexs during a registration process
    /// The connection id is a key and the value is the hex id chosen by specific user
    /// </summary>
    private static readonly ConcurrentDictionary<string, int> ChosenHexs = new();

    /// <summary>
    /// Dictionary to store chosen usernames during registration process
    /// The connection id is a key and the value is the username chosen by specific user
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> ChosenUsernames = new();

    public RegistrationHub(IUserService userService, IOAuth2RegistrationQueueService registrationQueueService,
        IHexColorGenerator hexGenerator, ITokenService tokenService)
    {
        _userService = userService;
        _registrationQueueService = registrationQueueService;
        _hexGenerator = hexGenerator;
        _tokenService = tokenService;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var queueToken = Context.GetHttpContext()?.Request.Query["queueToken"].ToString();
            UsersData[Context.ConnectionId] = _registrationQueueService.ExchangeToken(queueToken!);
        }
        catch (NullReferenceException)
        {
            // Abort the connection if the "queueToken" wasn't provided
            Context.Abort();
            return;
        }
        catch (NotFoundException)
        {
            // Abort the connection if the "queueToken" is invalid or missing
            Context.Abort();
            return;
        }

        // Generate list of possible Hexs and send it to the client
        HexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorListAsync(5);
        await Clients.Caller.SendAsync("ReceiveHexsList", HexsToOffer[Context.ConnectionId]);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Clearning up the temporary data after the user got disconnected
        HexsToOffer.Remove(Context.ConnectionId, out _);
        UsersData.Remove(Context.ConnectionId, out _);
        ChosenHexs.Remove(Context.ConnectionId, out _);
        ChosenUsernames.Remove(Context.ConnectionId, out _);

        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IRegistrationHub.RegenerateHexs"/>
    public async Task<SignalRHubResult> RegenerateHexs()
    {
        HexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorListAsync(5);
        return Success(HexsToOffer[Context.ConnectionId]);
    }

    /// <inheritdoc cref="IRegistrationHub.ChooseHex"/>
    public SignalRHubResult ChooseHex(int hex)
    {
        if (!HexsToOffer[Context.ConnectionId].Contains(hex))
            return Error(new(ErrorCode.InvalidAction));

        ChosenHexs[Context.ConnectionId] = hex;
        return Success();
    }

    /// <inheritdoc cref="IRegistrationHub.ChooseUsername"/>
    public SignalRHubResult ChooseUsername(string username)
    {
        ChosenUsernames[Context.ConnectionId] = username;
        return Success();
    }

    /// <inheritdoc cref="IRegistrationHub.FinishRegistration"/>
    public async Task<SignalRHubResult> FinishRegistration()
    {
        if (!ChosenHexs.ContainsKey(Context.ConnectionId))
            return Error(new(ErrorCode.InvalidAction, "You haven't chosen the hex id yet!"));

        if (!ChosenUsernames.ContainsKey(Context.ConnectionId))
            return Error(new(ErrorCode.InvalidAction, "You haven't chosen the username yet!"));

        var userOAuthId = UsersData[Context.ConnectionId].OAuth2UserId;
        var userAuthType = UsersData[Context.ConnectionId].AuthType;
        var chosenHex = ChosenHexs[Context.ConnectionId];
        var chosenUsername = ChosenUsernames[Context.ConnectionId];

        DetailedUserModel? resultUserModel = default;

        if (userAuthType == UserAuthType.Github)
        {
            var request = new RequestToAuthorizeViaGitHub
            {
                HexId = chosenHex,
                Username = chosenUsername,
                GitHubId = userOAuthId
            };

            resultUserModel = await _userService.AuthorizeViaGitHubAsync(request);
        }
        else if (userAuthType == UserAuthType.Google)
        {
            var request = new RequestToAuthorizeViaGoogle()
            {
                HexId = chosenHex,
                Username = chosenUsername,
                GoogleId = userOAuthId
            };

            resultUserModel = await _userService.AuthorizeViaGoogleAsync(request);
        }

        if (resultUserModel == null)
            return Error(new ErrorResponse(ErrorCode.InvalidAction, "Something went wrong!"));

        var jwtToken = _tokenService.GenerateJwtToken(resultUserModel.Id, resultUserModel.HexId, resultUserModel.AuthType);
        var refreshToken = _tokenService.GenerateRefreshToken(jwtToken);

        // Start a task to abort the connection after some time
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(700));
            Context.Abort(); // Abort the connection
        });

        // Returning the token
        return Success(new { jwtToken, refreshToken });
    }
}