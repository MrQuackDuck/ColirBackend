using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Misc.ExtensionMethods;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Extension method to get the id from the issuer of the request
    /// Warning: Use ONLY with <see cref="AuthorizeAttribute"/> set on the action/controller!
    /// </summary>
    public static long GetIssuerId(this ControllerBase @base)
    {
        return long.Parse(@base.HttpContext.User.Claims.First(c => c.Type == "Id").Value);
    }

    /// <summary>
    /// Extension method to get the hex id from the issuer of the request
    /// Warning: Use ONLY with <see cref="AuthorizeAttribute"/> set on the action/controller!
    /// </summary>
    public static int GetIssuerHexId(this ControllerBase @base)
    {
        return int.Parse(@base.HttpContext.User.Claims.First(c => c.Type == "HexId").Value);
    }
}