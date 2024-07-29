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
    private static readonly Dictionary<string, List<int>> _hexsToOffer = new();
    
    /// <summary>
    /// Dictionary to store users' data needed for registration process
    /// The connection id is a key and the value is user's OAuth2 id
    /// </summary>
    private static readonly Dictionary<string, RegistrationUserData> _usersData = new();
    
    /// <summary>
    /// Dictionary to store chosen hex ids during registration process
    /// The connection id is a key and the value is the hex id chosen by the user
    /// </summary>
    private static readonly Dictionary<string, int> _chosenHexs = new();
    
    /// <summary>
    /// Dictionary to store chosen usernames during registration process
    /// The connection id is a key and the value is the username chosen by the user
    /// </summary>
    private static readonly Dictionary<string, string> _chosenUsernames = new();
    
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
            _usersData[Context.ConnectionId] = _registrationQueueService.ExchangeToken(queueToken!);
        }
        catch (NotFoundException)
        {
            // Abort the connection if the "queueToken" is invalid
            Context.Abort();
            return;
        }
        
        // Generate list of possible Hexs and send it to the client
        _hexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorAsyncsListAsync(5);
        await Clients.Caller.SendAsync("ReceiveHexsList", _hexsToOffer[Context.ConnectionId]);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _hexsToOffer.Remove(Context.ConnectionId);
        _usersData.Remove(Context.ConnectionId);
        _chosenHexs.Remove(Context.ConnectionId);
        _chosenUsernames.Remove(Context.ConnectionId);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IRegistrationHub.RegenerateHexs"/>
    public async Task RegenerateHexs()
    {
        _hexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorAsyncsListAsync(5);
        await Clients.Caller.SendAsync("ReceiveHexsList", _hexsToOffer[Context.ConnectionId]);
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseHex"/>
    public void ChooseHex(int hex)
    {
        if (_hexsToOffer[Context.ConnectionId].Contains(hex))
            _chosenHexs[Context.ConnectionId] = hex;
        else 
            throw new HubException("Hex is not valid");
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseUsername"/>
    public void ChooseUsername(string username)
    {
        _chosenUsernames[Context.ConnectionId] = username;
    }
    
    /// <inheritdoc cref="IRegistrationHub.FinishRegistration"/>
    public async Task<DetailedUserModel> FinishRegistration()
    {
        if (!_chosenHexs.ContainsKey(Context.ConnectionId)) throw new HubException("You haven't chosen the hex id yet!");
        if (!_chosenUsernames.ContainsKey(Context.ConnectionId)) throw new HubException("You haven't chosen the username yet!");
        
        var userOAuthId = _usersData[Context.ConnectionId].OAuth2UserId;
        var userAuthType = _usersData[Context.ConnectionId].AuthType;
        var chosenHex = _chosenHexs[Context.ConnectionId];
        var chosenUsername = _chosenUsernames[Context.ConnectionId];
        
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

        if (resultUserModel == null)
            throw new HubException("Something went wrong!");
        
        return resultUserModel;
    }
}