using DAL;
using Microsoft.EntityFrameworkCore;

namespace Colir.Misc.ExtensionMethods;

public static class WebApplicationBuilderExtensions
{
    public static void AddPostgreDbContext(this WebApplicationBuilder builder)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "Colir";
        var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "USERNAME";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "USER_PASSWORD";

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!
            .Replace("{Host}", host)
            .Replace("{DbName}", dbName)
            .Replace("{DbUsername}", username)
            .Replace("{DbPassword}", password);

        builder.Services.AddDbContext<ColirDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}