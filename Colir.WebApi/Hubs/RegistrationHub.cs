using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.Exceptions.NotFound;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Interfaces.Hubs;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Colir.Hubs;

/// <inheritdoc cref="IRegistrationHub"/>
[SignalRHub]
public class RegistrationHub : Hub, IRegistrationHub
{
    private readonly IOAuth2RegistrationQueueService _registrationQueueService;
    private readonly IHexColorGenerator _hexGenerator;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private string _oAuth2UserId = default!;
    
    /// <summary>
    /// Key is connection id, value is a list of hexs to offer
    /// </summary>
    private static readonly Dictionary<string, List<int>> _hexsToOffer = new();

    private static readonly Dictionary<string, string> _userIds = new();
    
    public RegistrationHub(IOAuth2RegistrationQueueService registrationQueueService, IHexColorGenerator hexGenerator, 
        IWebHostEnvironment webHostEnvironment)
    {
        _registrationQueueService = registrationQueueService;
        _hexGenerator = hexGenerator;
        _webHostEnvironment = webHostEnvironment;
    }

    public override async Task OnConnectedAsync()
    {
        var queueToken = Context.GetHttpContext()?.Request.Query["queueToken"].ToString();

        try
        {
            // Verify the token if the application is not in development
            if (!_webHostEnvironment.IsDevelopment())
            {
                _oAuth2UserId = _registrationQueueService.ExchangeToken(queueToken!);
                _userIds[Context.ConnectionId] = _oAuth2UserId;
            }
        }
        catch (NotFoundException)
        {
            // If queueToken is invalid
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
        _userIds.Remove(Context.ConnectionId);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IRegistrationHub.RegenerateHexs"/>
    public async Task RegenerateHexs()
    {
        _hexsToOffer[Context.ConnectionId] = await _hexGenerator.GetUniqueHexColorAsyncsListAsync(5);
        await Clients.Caller.SendAsync("ReceiveHexsList", _hexsToOffer[Context.ConnectionId]);
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseHex"/>
    public void ChooseHex(int orderOfItem)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc cref="IRegistrationHub.ChooseUsername"/>
    public void ChooseUsername(string username)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc cref="IRegistrationHub.FinishRegistration"/>
    public DetailedUserModel FinishRegistration()
    {
        throw new NotImplementedException();
    }
}