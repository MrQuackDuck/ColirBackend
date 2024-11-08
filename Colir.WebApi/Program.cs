using System.IO.Abstractions;
using System.Text;
using Colir.ApiRelatedServices;
using Colir.BLL;
using Colir.BLL.Factories;
using Colir.BLL.Interfaces;
using Colir.BLL.Services;
using Colir.Hubs;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Misc.ExceptionHandlers;
using DAL;
using DAL.Interfaces;
using DAL.Repositories.Related;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;

var builder = WebApplication.CreateBuilder(args);

// Configuring logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var services = builder.Services;

// Adding an exception handler
services.AddExceptionHandler<UnhandledExceptionsHandler>();
builder.Services.AddProblemDetails();

// Adding sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Addding the DB context
services.AddDbContext<ColirDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding DAL services
services.AddTransient<IFileSystem, FileSystem>();
services.AddTransient<IRoomFileManager, RoomFileManager>();
services.AddTransient<IUnitOfWork, UnitOfWork>();

// Adding BLL services
services.AddAutoMapper(typeof(AutomapperProfile));
services.AddTransient<IAttachmentService, AttachmentService>();
services.AddTransient<IHexColorGenerator, HexColorGenerator>();
services.AddTransient<IMessageService, MessageService>();
services.AddTransient<IRoomCleanerFactory, RoomCleanerFactory>();
services.AddTransient<IRoomService, RoomService>();
services.AddTransient<IUserService, UserService>();
services.AddTransient<IUserStatisticsService, UserStatisticsService>();

// Adding API-Related services
services.AddTransient<ITokenService, TokenService>();
services.AddSingleton<IGitHubOAuth2Api, GitHubOAuth2Api>();
services.AddSingleton<IGoogleOAuth2Api, GoogleOAuth2Api>();
services.AddSingleton<IOAuth2RegistrationQueueService, OAuth2RegistrationQueueService>();
services.AddSingleton<IEventService, EventService>();

services.AddControllers();

// Adding authentication
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Authentication:JwtKey").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Configure the cookie name
                context.Token = context.Request.Cookies["jwt"];

                return Task.CompletedTask;
            }
        };
    });

// Adding CORS policy
services.AddCors(options =>
{
    options.AddDefaultPolicy(defaultPolicy =>
    {
        defaultPolicy.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

// Adding Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.AddSignalRSwaggerGen(signalROptions =>
    {
        signalROptions.HubPathFunc = s => $"/API/{s}";
    });
});

services.AddSwaggerGenWithConventionalRoutes(options =>
{
    options.IgnoreTemplateFunc = (template) => template.StartsWith("API/");
    options.SkipDefaults = true;
});

services.AddSignalR(e => e.MaximumReceiveMessageSize = 1024 * 1024 * 10);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    // Ensuring the DB is connected
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ColirDbContext>();
    await dbContext.Database.MigrateAsync();
}
catch (SqlException e)
{
    logger.LogCritical(e, "An error occurred during SQL Server connection establishment!");
    return;
}

app.UseCors();

app.UseExceptionHandler();

app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/wwwroot",
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Mapping SignalR hubs
app.MapHub<ChatHub>("API/Chat");
app.MapHub<ClearRoomHub>("API/ClearRoom");
app.MapHub<RegistrationHub>("API/Registration");
app.MapHub<VoiceChatHub>("API/VoiceChat");

// Using Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        options.DocumentTitle = "Swagger";
        options.EnableDeepLinking();
    });
}

app.MapControllerRoute("default", "API/{controller}/{action}/");

await app.RunAsync();