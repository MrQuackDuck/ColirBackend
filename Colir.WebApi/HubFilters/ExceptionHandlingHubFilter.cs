using Microsoft.AspNetCore.SignalR;

namespace Colir.HubFilters;

public class ExceptionHandlingHubFilter(ILogger<ExceptionHandlingHubFilter> logger) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "An unhandled exception occurred! Exception Type: {ExceptionType}\n" +
                "- Message: {Message}\n" +
                "- Hub: {Hub} - Method: {Method}" +
                "- StackTrace: {StackTrace}\n" +
                "- InnerException: {InnerException}",
                exception.GetType().FullName,
                exception.Message.Length == 0 ? "<empty>" : exception.Message,
                invocationContext.Hub.GetType().Name,
                invocationContext.HubMethodName,
                exception.StackTrace,
                exception.InnerException?.ToString()
            );

            return null;
        }
    }
}