using Dapper;
using smartoffice_web.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace smartoffice_web.WebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var sql = "SELECT * FROM Users";
            return await _dbConnection.QueryAsync<User>(sql);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var sql = "SELECT * FROM Users WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task AddUserAsync(User user)
        {
            var sql = "INSERT INTO Users (Id, Name, Email) VALUES (@Id, @Name, @Email)";
            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var sql = "UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var sql = "DELETE FROM Users WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
