using Microsoft.Data.SqlClient;
using smartoffice_web.WebApi.Repositories;
using smartoffice_web.WebApi.Services;
using System.Data;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load User Secrets (in Development)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();

var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

builder.Services.AddAuthorization();

// Keep the Identity mapping as is, so default endpoints are mapped under /auth.
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = sqlConnectionString;
    });

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ✅ Register direct DB connection
builder.Services.AddScoped<IDbConnection>(sp =>
{
    logger.LogInformation("🔗 Attempting to create a database connection...");
    return new SqlConnection(sqlConnectionString);
});

// ✅ Register IdentityService and Repositories
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IEnvironment2DRepository, Environment2DRepository>();
builder.Services.AddScoped<IObject2DRepository, Object2DRepository>();

var app = builder.Build();

// Keep default Identity endpoints mapped under /auth.
app.MapGroup("/auth")
    .MapIdentityApi<IdentityUser>();

// Custom logout endpoint.
app.MapPost("/auth/logout",
    async (SignInManager<IdentityUser> signInManager, [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
.RequireAuthorization();

app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? "Yes" : "No")}");

app.Use(async (context, next) =>
{
    Console.WriteLine($"📌 Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map controllers for your custom endpoints (e.g., a custom registration endpoint in AuthController).
app.MapControllers();

app.Run();
