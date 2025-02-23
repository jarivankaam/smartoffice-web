namespace smartoffice_web.WebApi.Repositories;
using smartoffice_web.WebApi.Models;
using Dapper;
using System;
using System.Threading.Tasks;

using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

public class AppUserRepository : IAppUserRepository
{
    private readonly IDbConnection _dbConnection;

    public AppUserRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<AppUser> GetByIdentityUserIdAsync(string identityUserId)
    {
        var query = "SELECT * FROM AppUsers WHERE IdentityUserId = @IdentityUserId";
        return await _dbConnection.QuerySingleOrDefaultAsync<AppUser>(query, new { IdentityUserId = identityUserId });
    }
    
    public async Task<AppUser> GetUserIdAsync(string userID)
    {
        var query = "SELECT Id FROM AppUsers WHERE IdentityUserId = @Id";
        return await _dbConnection.QuerySingleOrDefaultAsync<AppUser>(query, new { Id = userID });
    }

    public async Task<Guid> CreateAppUserAsync(AppUser user)
    {
        var query = "INSERT INTO AppUsers (Id, IdentityUserId, DisplayName) VALUES (@Id, @IdentityUserId, @DisplayName)";
        await _dbConnection.ExecuteAsync(query, user);
        return user.Id;
    }
    
    public async Task<Guid> GetUserWorlds(Guid userId)
    {
        var query = "SELECT id,name, maxHeight, maxWidth FROM Environment2D WHERE UserId = @UserId";
        return await _dbConnection.QuerySingleOrDefaultAsync<Guid>(query, new { UserId = userId});
    }
}

