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
    public class WorldRepository : IWorldRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<WorldRepository> _logger;

        public WorldRepository(IConfiguration configuration, ILogger<WorldRepository> logger)
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

        public async Task<IEnumerable<World>> GetAllWorldsAsync()
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation("🔍 Executing query: SELECT * FROM worlds");

                string sql = "SELECT id, name, maxHeight, maxWidth FROM worlds";
                var result = await connection.QueryAsync<World>(sql);

                _logger.LogInformation($"✅ Query executed successfully. Retrieved {result.AsList().Count} worlds.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in GetAllWorldsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<World?> GetWorldByIdAsync(Guid id)
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation($"🔍 Fetching world with ID: {id}");

                string sql = "SELECT id, name, maxHeight, maxWidth FROM worlds WHERE id = @Id";
                var world = await connection.QueryFirstOrDefaultAsync<World>(sql, new { Id = id });

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

        public async Task AddWorldAsync(World world)
        {
            try
            {
                using var connection = CreateConnection();
                world.Id = world.Id == Guid.Empty ? Guid.NewGuid() : world.Id;

                _logger.LogInformation($"📝 Inserting new world with ID: {world.Id}");

                string sql = "INSERT INTO worlds (id, name, maxHeight, maxWidth) VALUES (@Id, @Name, @MaxHeight, @MaxWidth)";
                await connection.ExecuteAsync(sql, world);

                _logger.LogInformation("✅ World inserted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in AddWorldAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateWorldAsync(World world)
        {
            try
            {
                using var connection = CreateConnection();
                _logger.LogInformation($"🔄 Updating world with ID: {world.Id}");

                string sql = "UPDATE worlds SET name = @Name, maxHeight = @MaxHeight, maxWidth = @MaxWidth WHERE id = @Id";
                await connection.ExecuteAsync(sql, world);

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
