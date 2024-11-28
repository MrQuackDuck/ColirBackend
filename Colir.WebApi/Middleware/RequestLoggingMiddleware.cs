using System.Diagnostics;
namespace Colir.Middleware;

/// <summary>
/// A middleware that logs info about requests and responses of the API
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Return if WebSocket
        if (IsWebsocketRequest(context))
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        double requestSizeKb = Math.Round((context.Request.ContentLength ?? 0) / 1024.0, 2);

        // Capture the response using a memory stream
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            responseBody.Position = 0;

            // Calculate response size in KB
            double responseSizeKb = Math.Round(responseBody.Length / 1024.0, 2);

            // Copy the response to the original stream
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);

            var ramUsageInMb = Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024;

            logger.LogInformation(
                "Request: [{Method}] {Url} - Status: {StatusCode} - Elapsed time: {Duration}ms - RAM Usage: {RamUsage} Mb\n" +
                "Request Size: {RequestSize} Kb\n" +
                "Response Size: {ResponseSize} Kb",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                ramUsageInMb,
                requestSizeKb,
                responseSizeKb);
        }
    }

    private bool IsWebsocketRequest(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest) return true;

        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (path != null && (path.Contains("/api/chat") || path.Contains("/api/voicechat"))) return true;

        return false;
    }
}