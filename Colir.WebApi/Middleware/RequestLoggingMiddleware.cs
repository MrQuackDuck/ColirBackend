﻿using System.Diagnostics;
namespace Colir.Middleware;

/// <summary>
/// A middleware that logs the request and response of the API
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        // Clone the request body so we can read it multiple times
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

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

            logger.LogInformation(
                "Request: [{Method}] {Url} - Status: {StatusCode} - Elapsed time: {Duration}ms\n" +
                "Request Body: {RequestBody}\n" +
                "Response Size: {ResponseSize} Kb",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                requestBody.Length == 0 ? "<empty>" : requestBody,
                responseSizeKb);
        }
    }
}