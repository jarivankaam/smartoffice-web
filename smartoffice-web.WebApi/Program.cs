using Microsoft.Data.SqlClient;
using smartoffice_web.WebApi.Repositories;
using System.Data;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


// ✅ Load User Secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}



var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();

var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = sqlConnectionString;
    });

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

app.MapGroup("/auth")
    .MapIdentityApi<IdentityUser>();
app.MapPost("/auth/logout",
 async (SignInManager<IdentityUser> signInManager,
 [FromBody] object empty) =>
 {
     if (empty != null)
     {
         await signInManager.SignOutAsync();
         return Results.Ok();
     }
     return Results.Unauthorized();
 })
.RequireAuthorization();

app.MapControllers()
    .RequireAuthorization();




app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? "Yes" : "No")}");


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
