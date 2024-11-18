using System.Diagnostics;
using Colir.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Colir.HubFilters;

/// <summary>
/// Filter to log the SignalR hub signals with IP address and response size
/// </summary>
public class LoggingHubFilter(ILogger<LoggingHubFilter> logger) : IHubFilter
{
    /// <summary>
    /// List of methods that should be ignored due to their high frequency
    /// </summary>
    private static string[] MethodNameblackList = new[]
    {
        nameof(VoiceChatHub.SendVoiceSignal),
        nameof(VoiceChatHub.SendVideoSignal),
        nameof(VoiceChatHub.SendStreamSignal),
    };

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        if (MethodNameblackList.Contains(invocationContext.HubMethodName))
        {
            return await next(invocationContext);
        }

        var stopwatch = Stopwatch.StartNew();
        var result = await next(invocationContext);
        stopwatch.Stop();

        logger.LogInformation(
            "Hub: {Hub} - Method: {Method} - Elapsed time: {Duration}ms",
            invocationContext.Hub.GetType().Name,
            invocationContext.HubMethodName,
            stopwatch.ElapsedMilliseconds);

        return result;
    }
}