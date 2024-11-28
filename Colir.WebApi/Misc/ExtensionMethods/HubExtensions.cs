using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Misc.ExtensionMethods;

public static class HubExtensions
{
    /// <summary>
    /// Extension method to get an id from the issuer of the request
    /// Warning: Use ONLY with <see cref="AuthorizeAttribute"/> set on the method/hub!
    /// </summary>
    public static long GetIssuerId(this Hub hub)
    {
        return long.Parse(hub.Context.User!.Claims.First(c => c.Type == "Id").Value);
    }

    /// <summary>
    /// Extension method to get the hex id from the issuer of the request
    /// Warning: Use ONLY with <see cref="AuthorizeAttribute"/> set on the method/hub!
    /// </summary>
    public static int GetIssuerHexId(this Hub hub)
    {
        return int.Parse(hub.Context.User!.Claims.First(c => c.Type == "HexId").Value);
    }
}