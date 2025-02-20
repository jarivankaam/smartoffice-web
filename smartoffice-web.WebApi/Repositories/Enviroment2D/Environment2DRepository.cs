using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using smartoffice_web.WebApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace smartoffice_web.WebApi.Repositories
{
    public class Environment2DRepository : IEnvironment2DRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<Environment2DRepository> _logger;

        public Environment2DRepository(IConfiguration configuration, ILogger<Environment2DRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;

            // Log connection string on startup (without revealing credentials)
            _logger.LogInformation($"🔍 Loaded Connection String: {_connectionString}");
        }

        private IDbConnection CreateConnection()
        {
            try
            {
                _logger.LogInformation("🔗 Creating new database connection...");
                var connection = new SqlConnection(_connectionString);
                _logger.LogInformation("✅ Database connection created successfully.");
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR creating database connection: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Enviroment2D>> GetAllWorldsAsync()
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation("🔍 Executing query: SELECT * FROM worlds");

                string sql = "SELECT id, name, maxHeight, maxWidth FROM worlds";
                var result = await connection.QueryAsync<Enviroment2D>(sql);

                _logger.LogInformation($"✅ Query executed successfully. Retrieved {result.AsList().Count} worlds.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in GetAllWorldsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Enviroment2D?> GetWorldByIdAsync(Guid id)
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation($"🔍 Fetching world with ID: {id}");

                string sql = "SELECT id, name, maxHeight, maxWidth FROM worlds WHERE id = @Id";
                var world = await connection.QueryFirstOrDefaultAsync<Enviroment2D>(sql, new { Id = id });

                if (world != null)
                    _logger.LogInformation("✅ World retrieved successfully.");
                else
                    _logger.LogWarning($"⚠️ No world found with ID: {id}");

                return world;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in GetWorldByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task AddWorldAsync(Enviroment2D enviroment2D)
        {
            try
            {
                using var connection = CreateConnection();
                enviroment2D.Id = enviroment2D.Id == Guid.Empty ? Guid.NewGuid() : enviroment2D.Id;

                _logger.LogInformation($"📝 Inserting new world with ID: {enviroment2D.Id}");

                string sql = "INSERT INTO worlds (id, name, maxHeight, maxWidth) VALUES (@Id, @Name, @MaxHeight, @MaxWidth)";
                await connection.ExecuteAsync(sql, enviroment2D);

                _logger.LogInformation("✅ World inserted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in AddWorldAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateWorldAsync(Enviroment2D enviroment2D)
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation($"🔄 Updating world with ID: {enviroment2D.Id}");

                string sql = "UPDATE worlds SET name = @Name, maxHeight = @MaxHeight, maxWidth = @MaxWidth WHERE id = @Id";
                await connection.ExecuteAsync(sql, enviroment2D);

                _logger.LogInformation("✅ World updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in UpdateWorldAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteWorldAsync(Guid id)
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation($"🗑 Deleting world with ID: {id}");

                string sql = "DELETE FROM worlds WHERE id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });

                _logger.LogInformation("✅ World deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in DeleteWorldAsync: {ex.Message}");
                throw;
            }
        }
    }
}
