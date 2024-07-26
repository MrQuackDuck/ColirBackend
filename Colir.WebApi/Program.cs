using System.IO.Abstractions;
using System.Text;
using Colir.BLL;
using Colir.BLL.Factories;
using Colir.BLL.Interfaces;
using Colir.BLL.Services;
using DAL;
using DAL.Interfaces;
using DAL.Repositories;
using DAL.Repositories.Related;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;

var builder = WebApplication.CreateBuilder(args);

// Addding DB context
builder.Services.AddDbContext<ColirDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding DAL services
builder.Services.AddTransient<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddTransient<ILastTimeUserReadChatRepository, LastTimeUserReadChatRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IReactionRepository, ReactionRepository>();
builder.Services.AddTransient<IFileSystem, FileSystem>();
builder.Services.AddTransient<IRoomFileManager, RoomFileManager>();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
builder.Services.AddTransient<IUserStatisticsRepository, UserStatisticsRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

// Adding BLL services
builder.Services.AddAutoMapper(typeof(AutomapperProfile));
builder.Services.AddTransient<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddTransient<IHexColorGenerator, HexColorGenerator>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IRoomCleanerFactory, RoomCleanerFactory>();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserStatisticsService, UserStatisticsService>();

builder.Services.AddControllers();

// Adding auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
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
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(defaultPolicy =>
    {
        defaultPolicy.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials();
    });
});

// Adding Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenWithConventionalRoutes(options =>
{
    options.IgnoreTemplateFunc = (template) => template.StartsWith("API/");
    options.SkipDefaults = true;
});

var app = builder.Build();

// Ensuring DB is created
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ColirDbContext>();
var dbConnected = await dbContext.Database.CanConnectAsync();
if (!dbConnected) throw new Exception("DB is not connected!");

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "API/{controller}/{action}/");

app.Run();