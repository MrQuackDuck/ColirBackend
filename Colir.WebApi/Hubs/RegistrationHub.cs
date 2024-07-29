using Colir.ApiRelatedServices.Models;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.Exceptions.NotFound;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Interfaces.Hubs;
using DAL.Enums;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

/// <inheritdoc cref="IRegistrationHub"/>
[SignalRHub]
public class RegistrationHub : Hub, IRegistrationHub
{
    private readonly IUserService _userService;
    private readonly IOAuth2RegistrationQueueService _registrationQueueService;
    private readonly IHexColorGenerator _hexGenerator;
    
    /// <summary>
    /// Dictionary to store hexs that are currently offered for users to choose from
    /// The connection id is a key and the value is a list of hexs to offer
    /// </summary>
    private static readonly Dictionary<string, List<int>> HexsToOffer = new();
    
    /// <summary>
    /// Dictionary to store users' data needed for registration process
    /// The connection id is a key and the value is user's OAuth2 id
    /// </summary>
    private static readonly Dictionary<string, RegistrationUserData> UsersData = new();
    
    /// <summary>
    /// Dictionary to store chosen hex ids during registration process
    /// The connection id is a key and the value is the hex id chosen by the user
    /// </summary>
    private static readonly Dictionary<string, int> ChosenHexs = new();
    
    /// <summary>
    /// Dictionary to store chosen usernames during registration process
    /// The connection id is a key and the value is the username chosen by the user
    /// </summary>
    private static readonly Dictionary<string, string> ChosenUsernames = new();
    
    public RegistrationHub(IUserService userService, IOAuth2RegistrationQueueService registrationQueueService, 
        IHexColorGenerator hexGenerator)
    {
        _userService = userService;
        _registrationQueueService = registrationQueueService;
        _hexGenerator = hexGenerator;
    }

    public override async Task OnConnectedAsync()
    {
        var queueToken = Context.GetHttpContext()?.Request.Query["queueToken"].ToString();

        try
        {
            UsersData[Context.ConnectionId] = _registrationQueueService.ExchangeToken(queueToken!);
        }
        catch (NotFoundException)
        {
            // Abort the connection if the "queueToken" is invalid
            Context.Abort();
            return;
        }
        
        // Generate list of possible Hexs and send it to the client
        HexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorAsyncsListAsync(5);
        await Clients.Caller.SendAsync("ReceiveHexsList", HexsToOffer[Context.ConnectionId]);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        HexsToOffer.Remove(Context.ConnectionId);
        UsersData.Remove(Context.ConnectionId);
        ChosenHexs.Remove(Context.ConnectionId);
        ChosenUsernames.Remove(Context.ConnectionId);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IRegistrationHub.RegenerateHexs"/>
    public async Task RegenerateHexs()
    {
        HexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorAsyncsListAsync(5);
        await Clients.Caller.SendAsync("ReceiveHexsList", HexsToOffer[Context.ConnectionId]);
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseHex"/>
    public void ChooseHex(int hex)
    {
        if (HexsToOffer[Context.ConnectionId].Contains(hex))
            ChosenHexs[Context.ConnectionId] = hex;
        else 
            throw new HubException("Hex is not valid");
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseUsername"/>
    public void ChooseUsername(string username)
    {
        ChosenUsernames[Context.ConnectionId] = username;
    }
    
    /// <inheritdoc cref="IRegistrationHub.FinishRegistration"/>
    public async Task<DetailedUserModel> FinishRegistration()
    {
        if (!ChosenHexs.ContainsKey(Context.ConnectionId)) throw new HubException("You haven't chosen the hex id yet!");
        if (!ChosenUsernames.ContainsKey(Context.ConnectionId)) throw new HubException("You haven't chosen the username yet!");
        
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
            throw new HubException("Something went wrong!");
        
        return resultUserModel;
    }
}