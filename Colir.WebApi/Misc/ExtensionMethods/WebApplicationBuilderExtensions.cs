using DAL;
using Microsoft.EntityFrameworkCore;

namespace Colir.Misc.ExtensionMethods;

public static class WebApplicationBuilderExtensions
{
    public static void AddPostgreDbContext(this WebApplicationBuilder builder)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!.Replace("{host}", host);
        builder.Services.AddDbContext<ColirDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}