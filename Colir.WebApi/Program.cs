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
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;

var builder = WebApplication.CreateBuilder(args);

// Adding DAL services
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<ILastTimeUserReadChatRepository, LastTimeUserReadChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
builder.Services.AddScoped<IFileSystem, FileSystem>();
builder.Services.AddScoped<IRoomFileManager, RoomFileManager>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
builder.Services.AddScoped<IUserStatisticsRepository, UserStatisticsRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Adding BLL services
builder.Services.AddAutoMapper(typeof(AutomapperProfile));
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IHexColorGenerator, HexColorGenerator>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IRoomCleanerFactory, RoomCleanerFactory>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserStatisticsService, UserStatisticsService>();

builder.Services.AddDbContext<ColirDbContext>();
builder.Services.AddControllers();

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

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "API/{controller}/{action}/");

app.Run();