using DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colir.WebApi.IntegrationTests;

internal class ColirWebAppFactory : WebApplicationFactory<Program>
{
    private DbContext _dbContext = default!;

    public void DeleteDatabase()
    {
        _dbContext.Database.EnsureDeleted();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ColirDbContext>));

            var connectionString = GetConnectionString(services);
            services.AddSqlServer<ColirDbContext>(connectionString);

            _dbContext = CreateDbContext(services);
        });
    }

    private static string GetConnectionString(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("IntegrationTestsConnection");
        ArgumentNullException.ThrowIfNull(connectionString);
        return connectionString;
    }

    private static ColirDbContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ColirDbContext>();
    }
}