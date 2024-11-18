using System.Text.Json;

namespace Colir.Middleware;

/// <summary>
/// In case of unhandled exception, returns a 500 response with a message and logs the exception
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "An unhandled exception occurred! Exception Type: {ExceptionType}\n" +
                "- Message: {Message}\n" +
                "- URL: {Url}\n" +
                "- StackTrace: {StackTrace}\n" +
                "- InnerException: {InnerException}",
                exception.GetType().FullName,
                exception.Message.Length == 0 ? "<empty>" : exception.Message,
                context.Request.Path,
                exception.StackTrace,
                exception.InnerException?.ToString()
            );

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize("An error occurred while processing your request. Please try again later.");
            await context.Response.WriteAsync(json);
        }
    }
}