using Colir.Middleware;

namespace Colir.Misc.ExtensionMethods;

public static class ApplicationBuilderExtensions
{
    public static void UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
    }
}