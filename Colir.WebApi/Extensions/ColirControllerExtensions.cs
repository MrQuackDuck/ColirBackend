using Microsoft.AspNetCore.Mvc;

namespace Colir.Extensions;

public static class ColirControllerExtensions
{
    /// <summary>
    /// Extension method to get an id from the issuer of the request
    /// </summary>
    public static long GetIssuerId(this ControllerBase @base)
    {
        return long.Parse(@base.HttpContext.User.Claims.First(c => c.Type == "Id").Value);
    }
}