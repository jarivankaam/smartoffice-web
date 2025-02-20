using Microsoft.Data.SqlClient;
using smartoffice_web.WebApi.Repositories;
using System.Data;
using Microsoft.Extensions.Logging;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load User Secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// ✅ Get Logger for Debugging
var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();

// ✅ Fetch Connection String and Log It
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// ✅ Register Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    logger.LogInformation("🔗 Attempting to create a database connection...");
    return new SqlConnection(sqlConnectionString);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEnvironment2DRepository, Environment2DRepository>();
builder.Services.AddScoped<IObject2DRepository, Object2DRepository>();

var app = builder.Build();

// ✅ Test Database Connection on Startup
async Task TestDatabaseConnection()
{
    try
    {
        using var connection = new SqlConnection(sqlConnectionString);
        await connection.OpenAsync();

        logger.LogInformation("✅ Database connection established successfully.");

        string sql = "SELECT 1";
        int result = await connection.ExecuteScalarAsync<int>(sql);

        if (result == 1)
        {
            logger.LogInformation("✅ Database query test passed.");
        }
        else
        {
            logger.LogWarning("⚠️ Database query test returned an unexpected result.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"❌ ERROR: Unable to connect to the database. {ex.Message}");
    }
}

// ✅ Run database test before the API starts
await TestDatabaseConnection();

app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? sqlConnectionString : "No")}");

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
