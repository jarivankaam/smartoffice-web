using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using smartoffice_web.WebApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace smartoffice_web.WebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbConnection = new SqlConnection(connectionString);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string sql = "SELECT * FROM Users";
            return await _dbConnection.QueryAsync<User>(sql);
        }

        public async Task<User?> GetUserByIdAsync(Guid id) // Changed int to Guid
        {
            string sql = "SELECT * FROM Users WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task AddUserAsync(User user)
        {
            string sql = "INSERT INTO Users (Id, UserName, Password) VALUES (@Id, @UserName, @Password)";
            user.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id; // Ensure a GUID is assigned
            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task UpdateUserAsync(User user)
        {
            string sql = "UPDATE Users SET UserName = @UserName, Password = @Password WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task DeleteUserAsync(Guid id) // Changed int to Guid
        {
            string sql = "DELETE FROM Users WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}