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

// ✅ Register Servicesuunity
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEnvironment2DRepository, Environment2DRepository>();
builder.Services.AddScoped<IObject2DRepository, Object2DRepository>();

var app = builder.Build();
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString"); 
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString); 
app.MapGet("/", () => $"The API is up . Connection string found: {(sqlConnectionStringFound ? "" : "")}");


// ✅ Middleware for Logging API Calls
app.Use(async (context, next) =>
{
    logger.LogInformation($"🌍 Incoming Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

// ✅ Start API  
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
