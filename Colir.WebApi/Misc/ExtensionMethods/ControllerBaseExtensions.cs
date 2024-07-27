using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Misc.ExtensionMethods;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Extension method to get an id from the issuer of the request
    /// Warning: Use ONLY with <see cref="AuthorizeAttribute"/> set on action/controller!
    /// </summary>
    public static long GetIssuerId(this ControllerBase @base)
    {
        return long.Parse(@base.HttpContext.User.Claims.First(c => c.Type == "Id").Value);
    }
}