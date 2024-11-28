using System.Diagnostics;
using Colir.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Colir.HubFilters;

/// <summary>
/// Filter that logs SignalR performance info about hub signal calls
/// </summary>
public class LoggingHubFilter(ILogger<LoggingHubFilter> logger) : IHubFilter
{
    /// <summary>
    /// List of methods that should be ignored due to their high frequency
    /// </summary>
    private static string[] _methodNameBlackList = new[]
    {
        nameof(VoiceChatHub.SendVoiceSignal),
        nameof(VoiceChatHub.SendVideoSignal),
        nameof(VoiceChatHub.SendStreamSignal),
    };

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        if (_methodNameBlackList.Contains(invocationContext.HubMethodName))
        {
            return await next(invocationContext);
        }

        var stopwatch = Stopwatch.StartNew();
        var result = await next(invocationContext);
        stopwatch.Stop();

        var ramUsageInMb = Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024;

        logger.LogInformation(
            "Hub: {Hub} - Method: {Method} - Elapsed time: {Duration}ms - RAM Usage: {RamUsage} Mb",
            invocationContext.Hub.GetType().Name,
            invocationContext.HubMethodName,
            stopwatch.ElapsedMilliseconds,
            ramUsageInMb);

        return result;
    }
}