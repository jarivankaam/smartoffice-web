using Microsoft.Data.SqlClient;
using smartoffice_web.WebApi.Repositories;
using System.Data;

using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load User Secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// ✅ Get Logger for Debugging
var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();

// ✅ Fetch Connection String and Log It
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
logger.LogInformation($"🔍 Loaded Connection String: {connectionString}");

// ✅ Register Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorldRepository, WorldRepository>();
builder.Services.AddScoped<IGameObjectRepository, GameObjectRepository>();

var app = builder.Build();

// ✅ Middleware for Logging API Calls
app.Use(async (context, next) =>
{
    logger.LogInformation($"🌍 Incoming Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});
app.MapGet("/", () => "Hello world, the API is up ");

// ✅ Start API  
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
