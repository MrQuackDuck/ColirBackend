namespace Colir.Misc.ExtensionMethods;

public static class HttpResponseExtensions
{
    public static void ApplyJwtToken(this HttpResponse response, string jwtToken)
    {
        response.Cookies.Append("jwt", jwtToken, new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });
    }
}