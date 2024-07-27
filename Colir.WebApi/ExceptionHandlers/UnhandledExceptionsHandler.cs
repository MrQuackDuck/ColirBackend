using Microsoft.AspNetCore.Diagnostics;

namespace Colir.ExceptionHandlers;

/// <summary>
/// Handler for exceptions which weren't handled in action methods
/// </summary>
public class UnhandledExceptionsHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsync("An unhandled exception occurred!", cancellationToken);

        return true;
    }
}